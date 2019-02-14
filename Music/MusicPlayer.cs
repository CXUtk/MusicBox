using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using NAudio.Wave;
using Terraria;
using System.Windows.Forms;

namespace MusicBox.Music
{
	public delegate void SongTextureEventHandler(byte[] data);
	public class MusicPlayer
	{
		private bool isMusicEnd;
		private bool isStopped;
		private Thread playSongThread;
		private IWavePlayer outputDevice;
		private AudioFileReader audioFile; 
		private TagLib.Tag currentSongFileDescriptor;

		public event EventHandler OnMusicEnd;
		public event EventHandler<FftEventArgs> OnFFTCalculated;
		public event EventHandler<MaxSampleEventArgs> OnMaximumCalculated;
		public event UpdateProgressEventHandler OnProgressUpdate;
		public event SongTextureEventHandler OnSongPicLoaded;

		private string playSrc;
		public string PlaySrc
		{
			get { return playSrc; }
			set { ResetSrc(value); }
		}

		public Version Version { get; }
		public List<string> SongFiles { get; private set; }
		public int CurrentSong { get; private set; }

		private float volume;
		public float Volume
		{
			get { return volume; }
			set
			{
				volume = value;
				outputDevice.Volume = volume;
			}
		}

		public string NowPlaying
		{
			get
			{
				return Path.GetFileNameWithoutExtension(SongFiles[CurrentSong]);
			}
		}

		public bool IsPaused { get; private set; }

		public MusicPlayer()
		{
			Version = typeof(Program).Assembly.GetName().Version;
			isMusicEnd = false;
			isStopped = true;
			IsPaused = true;
			CurrentSong = 0;
			outputDevice = new WaveOut
			{
				Volume = 0.2f,
				DesiredLatency = 200,
			};
		}

		public void ResetSrc(string src)
		{
			Stop();
			playSrc = src;
			SongFiles = new List<string>(Directory.EnumerateFiles(src));
			audioFile = null;
		}

		private void playSong()
		{
			while (!isStopped)
			{
				isMusicEnd = false;
				TagLib.File songFile = TagLib.File.Create(SongFiles[CurrentSong]);
				currentSongFileDescriptor = songFile.Tag;
				if (currentSongFileDescriptor.Pictures.Length > 0)
				{
					foreach (var pic in currentSongFileDescriptor.Pictures)
					{
						OnSongPicLoaded?.Invoke(pic.Data.Data);
						break;
					}
				}
				else
				{
					OnSongPicLoaded?.Invoke(null);
				}
				songFile.Dispose();



				// 这个bug是当前第一优先级
				try
				{

					audioFile = new AudioFileReader(SongFiles[CurrentSong]);
				}
				catch (Exception ex)
				{
				}
				SampleAggregator aggregator = new SampleAggregator(audioFile)
				{
					NotificationCount = audioFile.WaveFormat.SampleRate / 1024,
					PerformFFT = true,
				};
				aggregator.FFTCalculated += Aggregator_FFTCalculated;
				aggregator.MaximumCalculated += Aggregator_MaximumCalculated;
				playAction?.Invoke();
				outputDevice.Stop();
				outputDevice.Init(aggregator);
				outputDevice.Play();
				while (!Main.gameMenu && !isMusicEnd && (outputDevice.PlaybackState == PlaybackState.Playing ||
						outputDevice.PlaybackState == PlaybackState.Paused))
				{
					Thread.Sleep(16);
					OnProgressUpdate(audioFile.CurrentTime, audioFile.TotalTime);
				}
				if (Main.gameMenu || isStopped)
					break;
				CurrentSong = (CurrentSong + 1) % SongFiles.Count;
				playAction = null;
			}
		}

		private void Aggregator_MaximumCalculated(object sender, MaxSampleEventArgs e)
		{
			OnMaximumCalculated?.Invoke(sender, e);
		}

		private void Aggregator_FFTCalculated(object sender, FftEventArgs e)
		{
			OnFFTCalculated?.Invoke(sender, e);
		}

		private void playNew()
		{
			isMusicEnd = false;
			isStopped = false;
			IsPaused = false;
			playSongThread = new Thread(playSong);
			playSongThread.Start();

		}

		private void resume()
		{
			IsPaused = false;
			outputDevice.Play();
		}

		/// <summary>
		/// Interrupt and play the next song.
		/// </summary>
		public void SwitchNextSong()
		{
			playAction = null;
			if (!isMusicEnd && playSongThread != null)
			{
				isMusicEnd = true;
			}
			//CurrentSong = (CurrentSong + 1) % SongFiles.Count;
			//playNew();

		}

		/// <summary>
		/// Interrupt and play the previous song.
		/// </summary>
		public void SwitchPrevSong()
		{
			playAction = null;
			isMusicEnd = true;
			CurrentSong -= 2;
			if(CurrentSong < 0) { CurrentSong += SongFiles.Count; }
		}

		/// <summary>
		/// Stop current song entirely.
		/// </summary>
		public void Stop()
		{
			IsPaused = true;
			isMusicEnd = true;
			isStopped = true;
			playSongThread?.Join();
			outputDevice?.Stop();
			currentSongFileDescriptor = null;
		}

		/// <summary>
		/// Start playing the song from the beginning.
		/// </summary>
		public void Play()
		{
			playAction = null;
			if (isStopped || isMusicEnd)
			{
				playNew();
				return;
			}
			if (IsPaused)
			{
				resume();
			}
		}

		private Action playAction = null;
		/// <summary>
		/// Start playing the song from the beginning.
		/// </summary>
		public void PlayFrom(double percent)
		{
			if (isStopped || isMusicEnd)
			{
				playAction = new Action(() => SetTime(percent));
				playNew();
				return;
			}
			if (IsPaused)
			{
				resume();
			}
			SetTime(percent);
		}

		public void PlayRandom()
		{
			playAction = null;
			playSongThread.Abort();
			CurrentSong = Main.rand.Next(0, SongFiles.Count);
			playNew();
		}

		public void Pause()
		{
			IsPaused = true;
			outputDevice.Pause();
		}

		/// <summary>
		/// Sets the time for the current song. Time must be valid.
		/// </summary>
		/// <param name="time">The time to set</param>
		/// <returns>If success.</returns>
		public void SetTime(TimeSpan time)
		{
			if (time > audioFile.TotalTime)
				throw new ArgumentException("设置了非法的时间");
			double percent = time.TotalMilliseconds / audioFile.TotalTime.TotalMilliseconds;
			SetTime(percent);
		}

		/// <summary>
		/// Sets the time for the current song. Percentage must be between 0 and 1.
		/// </summary>
		/// <param name="percent"></param>
		/// <returns></returns>
		public void SetTime(double percent)
		{
			if(percent < 0 || percent > 1)
				throw new ArgumentException("设置了非法的时间");
			audioFile.Position = (long)(percent * (audioFile.Length - 1));
		}

		public void Dispose()
		{
			Stop();
			outputDevice?.Dispose();
			audioFile?.Dispose();
			playSongThread?.Abort();
			playSongThread = null;
			outputDevice = null;
			audioFile = null;
			isStopped = true;
		}

		/// <summary>
		/// Gets the tag of the song (with all information)
		/// </summary>
		/// <returns>The metadata of the song</returns>
		public TagLib.Tag GetCurrentSongDescription()
		{
			if (isStopped || currentSongFileDescriptor == null)
				return null;
			return currentSongFileDescriptor;
		}
	}
}
