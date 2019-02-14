using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusicBox.Music;
using NAudio.Dsp;

namespace MusicBox.Music
{
	public class SpectrumAnalyzer
	{
		private const int SPECTRUM_BIN_SIZE = 16;
		private const int WAVE_LINE_NUMBER = 400;

		private double[] _specturmValue = new double[SPECTRUM_BIN_SIZE];
		private double _maxiumFFT;

		private LinkedList<double> _listSample = new LinkedList<double>();
		private double _movingTotal;
		private double _movingAvg;
		private double _amplitudeRatio;
		private int _windowSize;

		public double[] SpecturmValue
		{
			get
			{
				return _specturmValue;
			}
		}

		public double MaxSpectrumValue
		{
			get
			{
				return _maxiumFFT;
			}
		}

		public LinkedList<double> WaveLines
		{
			get
			{
				return _listSample;
			}
		}

		public double AmplitudeRatio
		{
			get
			{
				return _amplitudeRatio;
			}
		}


		public SpectrumAnalyzer(int windowSize = 1024)
		{
			_windowSize = windowSize;
			for(int i = 0; i < WAVE_LINE_NUMBER; i++)
			{
				_listSample.AddLast(0);
			}
		}

		private double GetYPosLog(Complex c)
		{
			double intensityDB = 10 * Math.Log(Math.Sqrt(c.X * c.X + c.Y * c.Y));
			double minDB = -96;
			if (intensityDB < minDB) intensityDB = minDB;
			double percent = intensityDB / minDB;
			return percent;
		}

		private double getAnother(Complex c)
		{
			return Math.Sqrt(c.X * c.X + c.Y * c.Y);
		}

		public void CalculateFFT(Complex[] data)
		{
			int step = _windowSize / SPECTRUM_BIN_SIZE;
			for (int i = 0; i < _specturmValue.Length; i++)
			{
				_specturmValue[i] = 0;
			}
			for (int i = 0; i < data.Length; i++)
			{
				_specturmValue[i / step] += getAnother(data[i]) / step;
			}
			_maxiumFFT = 0;
			for (int i = 0; i < _specturmValue.Length; i++)
			{
				_maxiumFFT = Math.Max(_maxiumFFT, _specturmValue[i]);
			}
		}

		public void AddAmplitude(float maxVal, float minVal)
		{
			lock (this)
			{
				_amplitudeRatio = maxVal - minVal;
				_listSample.RemoveFirst();
				_listSample.AddLast(maxVal - minVal);
			}
		}
	}
}
