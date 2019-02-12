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
	public class MusicPlayer
	{
		private bool isMusicEnd;
		private bool isStopped;
		private Thread playSongThread;
		private IWavePlayer outputDevice;
		private AudioFileReader audioFile;      // 暂时使用AudioFileReader
		private TagLib.Tag currentSongFileDescriptor;

		public event EventHandler OnMusicEnd;
		public event EventHandler<FftEventArgs> OnFFTCalculated;
		public event UpdateProgressEventHandler OnProgressUpdate;

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
			outputDevice = new WaveOutEvent
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
			// 性能关键点，考虑用cache
			// Dispose 不要乱用， 容易引发异常且未观测到任何性能提升
			TagLib.File songFile = TagLib.File.Create(SongFiles[CurrentSong]);
			currentSongFileDescriptor = songFile.Tag;
			songFile.Dispose();
			
			audioFile = new AudioFileReader(SongFiles[CurrentSong]);
			SampleAggregator aggregator = new SampleAggregator(audioFile)
			{
				NotificationCount = audioFile.WaveFormat.SampleRate / 10000,
				PerformFFT = true,
			};
			aggregator.FFTCalculated += Aggregator_FFTCalculated;
			playAction?.Invoke();
			outputDevice.Stop();
			outputDevice.Init(aggregator);
			outputDevice.Play();
			while (outputDevice.PlaybackState == PlaybackState.Playing ||
					outputDevice.PlaybackState == PlaybackState.Paused)
			{
				Thread.Sleep(16);
				OnProgressUpdate(audioFile.CurrentTime, audioFile.TotalTime);
			}
			isMusicEnd = true;
			// 事件驱动
			OnMusicEnd(this, new EventArgs());
		}

		private void Aggregator_FFTCalculated(object sender, FftEventArgs e)
		{
			OnFFTCalculated?.Invoke(sender, e);
		}

		private void playNew()
		{
			playSongThread = new Thread(playSong);
			playSongThread.Start();
			isMusicEnd = false;
			isStopped = false;
			IsPaused = false;
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
			if (!isMusicEnd)
			{
				playSongThread.Abort();
				isMusicEnd = true;
			}
			CurrentSong = (CurrentSong + 1) % SongFiles.Count;
			playNew();
		}

		/// <summary>
		/// Interrupt and play the previous song.
		/// </summary>
		public void SwitchPrevSong()
		{
			playAction = null;
			playSongThread.Abort();
			CurrentSong--;
			if (CurrentSong < 0)
				CurrentSong += SongFiles.Count;
			playNew();
		}

		/// <summary>
		/// Stop current song entirely.
		/// </summary>
		public void Stop()
		{
			if (isStopped)
				return;
			outputDevice.Stop();
			playSongThread.Abort();
			playSongThread = null;
			currentSongFileDescriptor = null;
			isStopped = true;
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
