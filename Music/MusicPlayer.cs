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
		public List<string> SongNames { get; private set; }
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

		public bool IsPaused { get; private set; }

		public MusicPlayer()
		{
			Version = typeof(Program).Assembly.GetName().Version;
			isMusicEnd = false;
			isStopped = true;
			IsPaused = false;
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
			SongNames = new List<string>(Directory.EnumerateFiles(src));
			audioFile = null;
		}

		private void playSong()
		{
			// 性能关键点，考虑用cache
			// Dispose 不要乱用， 容易引发异常且未观测到任何性能提升
			audioFile = new AudioFileReader(SongNames[CurrentSong]);
			SampleAggregator aggregator = new SampleAggregator(audioFile)
			{
				NotificationCount = audioFile.WaveFormat.SampleRate / 10000,
				PerformFFT = true,
			};
			aggregator.FFTCalculated += Aggregator_FFTCalculated;
			outputDevice.Stop();
			outputDevice.Init(aggregator);
			outputDevice.Play();
			while (outputDevice.PlaybackState == PlaybackState.Playing ||
					outputDevice.PlaybackState == PlaybackState.Paused)
			{
				OnProgressUpdate(audioFile.CurrentTime, audioFile.TotalTime);
				Thread.Sleep(1000);
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
			if (!isMusicEnd)
			{
				playSongThread.Abort();
				isMusicEnd = true;
			}
			CurrentSong = (CurrentSong + 1) % SongNames.Count;
			playNew();
		}

		/// <summary>
		/// Interrupt and play the previous song.
		/// </summary>
		public void SwitchPrevSong()
		{
			playSongThread.Abort();
			CurrentSong--;
			if (CurrentSong < 0)
				CurrentSong += SongNames.Count;
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
			isStopped = true;
		}

		/// <summary>
		/// Start playing the song from the beginning.
		/// </summary>
		public void Play()
		{
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

		public void Pause()
		{
			IsPaused = true;
			outputDevice.Pause();
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
	}
}
