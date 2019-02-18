using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using Terraria;
using Terraria.ID;
using Terraria.UI;
using Terraria.GameContent.UI.Elements;
using MusicBox.UI;
using MusicBox.Utils;
using MusicBox.Music;
using NAudio.Dsp;

namespace MusicBox.UIPage
{
	public class MusicPlayUI : AdvWindowUIState
	{   
		private const float UI_PANEL_WIDTH = 600f;
		private const float UI_PANEL_HEIGHT = 360f;

		private const float UI_BAR_WIDTH = 400f;
		private const float UI_BAR_HEIGHT = 16f;
		private const float UI_BAR_LEFT_OFFSET = 40f;

		private MusicPlayer musicPlayer { get { return MusicBox.Instance.MusicPlayer;  } }

		private UIBar _progressBar;
		private UIPicButton _playButton;
		private UIPicButton _forwardButton;
		private UIPicButton _backwardButton;
		private UISliderH _playSlider;
		private Texture2D _songTexture;
		private UIFixedImage _songImage;
		private UISliderV _volumeSlider;

		private TimeSpan _playPosition;
		private TimeSpan _playLength;

		private bool dontUpdatePlayPosition = false;
		private SpectrumAnalyzer _spectrumAnalyzer;

		private Vector2 Center
		{
			get
			{
				return WindowPanel.GetInnerDimensions().Center();
			}
		}

		protected override void Initialize(UIAdvPanel WindowPanel)
		{
			_spectrumAnalyzer = new SpectrumAnalyzer();
			musicPlayer.OnProgressUpdate += MusicPlayer_OnProgressUpdate;
			musicPlayer.OnSongPicLoaded += MusicPlayer_OnSongPicLoaded;
			musicPlayer.OnFFTCalculated += MusicPlayer_OnFFTCalculated;
			musicPlayer.OnMaximumCalculated += MusicPlayer_OnMaximumCalculated;
			base.Initialize(WindowPanel);
			WindowPanel.MainTexture = MusicBox.ModTexturesTable["AdvInvBack1"];
			WindowPanel.SetPadding(0);
			WindowPanel.Left.Set(Main.screenWidth / 2 - UI_PANEL_WIDTH / 2, 0f);
			WindowPanel.Top.Set(Main.screenHeight / 2 - UI_PANEL_HEIGHT / 2, 0f);
			WindowPanel.Width.Set(UI_PANEL_WIDTH, 0f);
			WindowPanel.Height.Set(UI_PANEL_HEIGHT, 0f);
			WindowPanel.Color = Color.White;
			WindowPanel.CornerSize = 12;

			_progressBar = new UIBar();
			_progressBar.SetPadding(0);
			_progressBar.Top.Set(100f, 0.5f);
			_progressBar.Left.Set(-UI_BAR_WIDTH / 2 + UI_BAR_LEFT_OFFSET, 0.5f);
			_progressBar.Width.Set(UI_BAR_WIDTH, 0f);
			_progressBar.Height.Set(UI_BAR_HEIGHT, 0f);
			_progressBar.BarFrameTex = MusicBox.ModTexturesTable["BarFrame"];
			_progressBar.BarFillTex = MusicBox.ModTexturesTable["BarFiller"];
			_progressBar.BarFrameTexCornerSize = new Vector2(6, 6);
			_progressBar.FillerDrawOffset = new Vector2(6, 6);
			_progressBar.FillerSize = new Vector2(UI_BAR_WIDTH - 12, 6);
			WindowPanel.Append(_progressBar);

			_playButton = new UIPicButton();
			_playButton.Texture = MusicBox.ModTexturesTable["PlayButtonN"];
			_playButton.Top.Set(135f - 15f , 0.5f);
			_playButton.Left.Set(-15 + UI_BAR_LEFT_OFFSET, 0.5f);
			_playButton.Width.Set(30, 0f);
			_playButton.Height.Set(30, 0f);
			_playButton.OnMouseHover += _playButton_OnMouseHover;
			_playButton.OnMouseOut += _playButton_OnMouseOut;
			_playButton.OnClick += _playButton_OnClick;
			WindowPanel.Append(_playButton);

			_playSlider = new UISliderH();
			_playSlider.Texture = MusicBox.ModTexturesTable["PlaySliderN"];
			_playSlider.Top.Set(0, 0f);
			_playSlider.Left.Set(0, 0f);
			_playSlider.Width.Set(30, 0f);
			_playSlider.Height.Set(30, 0f);
			_playSlider.StartX = 6f;
			_playSlider.EndX = UI_BAR_WIDTH - 6;
			_playSlider.Scale = 1.35f;
			_playSlider.OnValueChange += _playSlider_OnValueChange;
			_playSlider.OnMouseOver += _playSlider_OnMouseOver;
			_playSlider.OnMouseOut += _playSlider_OnMouseOut;
			_progressBar.Append(_playSlider);

			_volumeSlider = new UISliderV();
			_volumeSlider.Texture = MusicBox.ModTexturesTable["PlaySliderN"];
			_volumeSlider.Top.Set(110f, 0f);
			_volumeSlider.Left.Set(-30f, 1f);
			_volumeSlider.Width.Set(30, 0f);
			_volumeSlider.Height.Set(30, 0f);
			_volumeSlider.StartY = 110f;
			_volumeSlider.EndY = 80f;
			_volumeSlider.Scale = 1.35f;
			_volumeSlider.OnValueChange += _volumeSlider_OnValueChange;
			_volumeSlider.OnMouseOver += _volumeSlider_OnMouseOver;
			_volumeSlider.OnMouseOut += _volumeSlider_OnMouseOut;
			WindowPanel.Append(_volumeSlider);

			_forwardButton = new UIPicButton();
			_forwardButton.Texture = MusicBox.ModTexturesTable["ForwardButtonN"];
			_forwardButton.Top.Set(135f - 15f, 0.5f);
			_forwardButton.Left.Set(30f + UI_BAR_LEFT_OFFSET, 0.5f);
			_forwardButton.Width.Set(30, 0f);
			_forwardButton.Height.Set(30, 0f);
			_forwardButton.OnMouseHover += _forwardButton_OnMouseHover;
			_forwardButton.OnMouseOut += _forwardButton_OnMouseOut;
			_forwardButton.OnClick += _forwardButton_OnClick;
			WindowPanel.Append(_forwardButton);


			_backwardButton = new UIPicButton();
			_backwardButton.Texture = MusicBox.ModTexturesTable["BackwardButtonN"];
			_backwardButton.Top.Set(135f - 15f, 0.5f);
			_backwardButton.Left.Set(-60f + UI_BAR_LEFT_OFFSET, 0.5f);
			_backwardButton.Width.Set(30, 0f);
			_backwardButton.Height.Set(30, 0f);
			_backwardButton.OnMouseHover += _backwardButton_OnMouseHover;
			_backwardButton.OnMouseOut += _backwardButton_OnMouseOut;
			_backwardButton.OnClick += _backwardButton_OnClick;
			WindowPanel.Append(_backwardButton);

			_songTexture = MusicBox.ModTexturesTable["AdvInvBack1"];
			_songImage = new UIFixedImage(_songTexture);
			_songImage.Top.Set(60f, 0.5f);
			_songImage.Left.Set(-UI_BAR_WIDTH / 2 + UI_BAR_LEFT_OFFSET - 110f, 0.5f);
			_songImage.Width.Set(90f, 0f);
			_songImage.Height.Set(90f, 0f);
			WindowPanel.Append(_songImage);
		}

		private void _volumeSlider_OnMouseOut(UIMouseEvent evt, UIElement listeningElement)
		{
			_volumeSlider.Texture = MusicBox.ModTexturesTable["PlaySliderN"];
		}

		private void _volumeSlider_OnMouseOver(UIMouseEvent evt, UIElement listeningElement)
		{
			_volumeSlider.Texture = MusicBox.ModTexturesTable["PlaySlider"];
		}

		private void _volumeSlider_OnValueChange(float value, UIElement sender)
		{
			musicPlayer.Volume = value;
		}

		private void MusicPlayer_OnMaximumCalculated(object sender, MaxSampleEventArgs e)
		{
			_spectrumAnalyzer.AddAmplitude(e.MaxSample, e.MinSample);
		}

		
		private bool drawfft = false;
		private void MusicPlayer_OnFFTCalculated(object sender, FftEventArgs e)
		{
			_spectrumAnalyzer.CalculateFFT(e.Result);
		}

		private void MusicPlayer_OnSongPicLoaded(byte[] data)
		{
			if (data == null)
			{
				_songImage.Texture = _songTexture = MusicBox.ModTexturesTable["AdvInvBack1"];
				return;
			}
			using (MemoryStream ms = new MemoryStream(data))
			{
				_songTexture = Texture2D.FromStream(Main.instance.GraphicsDevice, ms);
			}
			_songImage.Texture = _songTexture;

		}

		private void _playSlider_OnMouseOut(UIMouseEvent evt, UIElement listeningElement)
		{
			_playSlider.Texture = MusicBox.ModTexturesTable["PlaySliderN"];
		}

		private void _playSlider_OnMouseOver(UIMouseEvent evt, UIElement listeningElement)
		{
			_playSlider.Texture = MusicBox.ModTexturesTable["PlaySlider"];
		}

		private void _playSlider_OnValueChange(float value, UIElement sender)
		{
			musicPlayer.PlayFrom(value);
			dontUpdatePlayPosition = true;
		}

		private void _backwardButton_OnClick(UIMouseEvent evt, UIElement listeningElement)
		{
			musicPlayer.SwitchPrevSong();
			UpdatePlayButton();
		}

		private void _backwardButton_OnMouseOut(UIMouseEvent evt, UIElement listeningElement)
		{
			_backwardButton.Texture = MusicBox.ModTexturesTable["BackwardButtonN"];
		}

		private void _backwardButton_OnMouseHover(UIElement target, Vector2 mousePosition)
		{
			_backwardButton.Texture = MusicBox.ModTexturesTable["BackwardButton"];
		}

		private void _forwardButton_OnClick(UIMouseEvent evt, UIElement listeningElement)
		{
			musicPlayer.SwitchNextSong();
			UpdatePlayButton();
		}

		private void _forwardButton_OnMouseOut(UIMouseEvent evt, UIElement listeningElement)
		{
			_forwardButton.Texture = MusicBox.ModTexturesTable["ForwardButtonN"];
		}

		private void _forwardButton_OnMouseHover(UIElement target, Vector2 mousePosition)
		{
			_forwardButton.Texture = MusicBox.ModTexturesTable["ForwardButton"];
		}

		private void _playButton_OnClick(UIMouseEvent evt, UIElement listeningElement)
		{
			if (musicPlayer.IsPaused)
			{
				musicPlayer.Play();
				_playButton.Texture = MusicBox.ModTexturesTable["PauseButton"];
			}
			else
			{
				musicPlayer.Pause();
				_playButton.Texture = MusicBox.ModTexturesTable["PlayButton"];
			}
		}

		private void _playButton_OnMouseOut(UIMouseEvent evt, UIElement listeningElement)
		{
			UpdatePlayButton();
		}

		private void UpdatePlayButton()
		{
			if (musicPlayer.IsPaused)
			{
				_playButton.Texture = MusicBox.ModTexturesTable["PlayButtonN"];
			}
			else
			{
				_playButton.Texture = MusicBox.ModTexturesTable["PauseButtonN"];
			}
		}

		private void _playButton_OnMouseHover(UIElement target, Vector2 mousePosition)
		{
			if (musicPlayer.IsPaused)
			{
				_playButton.Texture = MusicBox.ModTexturesTable["PlayButton"];
			}
			else
			{
				_playButton.Texture = MusicBox.ModTexturesTable["PauseButton"];
			}
		}

		private void MusicPlayer_OnProgressUpdate(TimeSpan curPos, TimeSpan totalLen)
		{
			dontUpdatePlayPosition = false;
			_playPosition = curPos;
			_playLength = totalLen;
		}

		protected override void OnClose(UIMouseEvent evt, UIElement listeningElement)
		{
			MusicBox.Instance.CanShowMusicPlayUI = false;
		}

		protected override void DrawChildren(SpriteBatch sb)
		{

			base.DrawChildren(sb);
			DrawProgressBar(sb);
			Rectangle coverbox = _songImage.GetOuterDimensions().ToRectangle();
			Drawing.DrawAdvBox(sb, new Rectangle(coverbox.X - 2, coverbox.Y - 2, coverbox.Width + 4, coverbox.Height + 4),
				Color.White, MusicBox.ModTexturesTable["CoverFrame"], new Vector2(10, 10));
			Vector2 drawPos = _progressBar.GetDimensions().Position() - new Vector2(0, 50);
			sb.Draw(Main.magicPixel, new Rectangle((int)(drawPos.X), (int)drawPos.Y, (int)UI_BAR_WIDTH, 3), Color.Gray);


			lock (_spectrumAnalyzer)
			{
				int width = (int)(UI_BAR_WIDTH / 16);
				for (int i = 0; i < _spectrumAnalyzer.SpecturmValue.Length; i++)
				{
					int height = (int)(_spectrumAnalyzer.SpecturmValue[i] / _spectrumAnalyzer.MaxSpectrumValue * 100f);
					sb.Draw(Main.magicPixel, new Rectangle((int)(drawPos.X) + width * i,
						(int)(drawPos.Y) - height, width, height), Color.White);
				}


				int cnt = 0;
				foreach (var h in _spectrumAnalyzer.WaveLines)
				{
					int height = (int)(h * 50);

					sb.Draw(Main.magicPixel, new Rectangle((int)(drawPos.X + cnt),
							(int)(drawPos.Y) - 50 - height, 1, height * 2), Color.Yellow * 0.75f);
					cnt++;
				}
				int y = (int)(_spectrumAnalyzer.AmplitudeRatio * 100);
				Vector2 ampDrawPos = drawPos - new Vector2(80f, 0f);
				sb.Draw(Main.magicPixel, new Rectangle((int)ampDrawPos.X,
					(int)drawPos.Y - y, 30, y), Color.Cyan * 0.9f);

			}
			// DrawSongList(sb);
		}

		private void DrawProgressBar(SpriteBatch sb)
		{
			Vector2 barCenter = _progressBar.GetInnerDimensions().Center();

			string text = string.Format("{0}/{1}", _playPosition.ToString(@"mm\:ss"), _playLength.ToString(@"mm\:ss"));
			Vector2 textSize = Main.fontMouseText.MeasureString(text);
			Terraria.Utils.DrawBorderStringFourWay(sb, Main.fontMouseText, text,
				barCenter.X, barCenter.Y - 12, Color.White, Color.Black, textSize * 0.5f);
		}

		private void DrawSongList(SpriteBatch sb)
		{
			Vector2 start = Center - new Vector2(UI_PANEL_WIDTH / 2 - 80, UI_PANEL_HEIGHT / 2 - 80);
			Vector2 offset = Vector2.Zero;
			Vector2 textSize = Main.fontMouseText.MeasureString("hi");
			int current = musicPlayer.CurrentSong;
			for (int i = 1; i < 6; i++)
			{
				string text = string.Format("{0}: {1}", i, Path.GetFileNameWithoutExtension(musicPlayer.SongFiles[(current + i - 1) % musicPlayer.SongFiles.Count]));
				Vector2 pos = start + offset;
				Terraria.Utils.DrawBorderStringFourWay(sb, Main.fontMouseText, text, pos.X, pos.Y, Color.White, Color.Black, textSize * 0.5f);
				offset += new Vector2(0f, textSize.Y + 2f);
			}
		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);
			float factor = _playPosition.Ticks / (float)_playLength.Ticks;
			if (double.IsNaN(factor))
				factor = 0;
			if (!_playSlider.Dragging && !dontUpdatePlayPosition)
			{
				_playSlider.Value = factor;
			}
			_progressBar.Value = _playSlider.Value;
			_volumeSlider.Value = musicPlayer.Volume;
		}

	}
}