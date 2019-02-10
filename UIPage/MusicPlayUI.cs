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

namespace MusicBox.UIPage
{
	public class MusicPlayUI : AdvWindowUIState
	{   
		private const float UI_PANEL_WIDTH = 480f;
		private const float UI_PANEL_HEIGHT = 360f;

		private const float UI_BAR_WIDTH = 400f;
		private const float UI_BAR_HEIGHT = 16f;

		private MusicPlayer musicPlayer { get { return MusicBox.Instance.MusicPlayer;  } }

		private UIBar _progressBar;
		private UIPicButton _playButton;

		private TimeSpan _playPosition;
		private TimeSpan _playLength;

		private Vector2 Center
		{
			get
			{
				return WindowPanel.GetInnerDimensions().Center();
			}
		}

		protected override void Initialize(UIAdvPanel WindowPanel)
		{
			musicPlayer.OnProgressUpdate += MusicPlayer_OnProgressUpdate;
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
			_progressBar.Top.Set(100f, 0.5f);
			_progressBar.Left.Set(-UI_BAR_WIDTH / 2, 0.5f);
			_progressBar.Width.Set(UI_BAR_WIDTH, 0f);
			_progressBar.Height.Set(UI_BAR_HEIGHT, 0f);
			_progressBar.BarFrameTex = MusicBox.ModTexturesTable["BarFrame"];
			_progressBar.BarFillTex = MusicBox.ModTexturesTable["BarFiller"];
			_progressBar.BarFrameTexCornerSize = new Vector2(6, 6);
			_progressBar.FillerDrawOffset = new Vector2(6, 6);
			_progressBar.FillerSize = new Vector2(UI_BAR_WIDTH - 12, 6);
			WindowPanel.Append(_progressBar);

			_playButton = new UIPicButton();
			_playButton.Texture = MusicBox.ModTexturesTable["PlayButton"];
			_playButton.Top.Set(135f - 15f, 0.5f);
			_playButton.Left.Set(-15, 0.5f);
			_playButton.Width.Set(30, 0f);
			_playButton.Height.Set(30, 0f);
			_playButton.OnMouseHover += _playButton_OnMouseHover;
			_playButton.OnMouseOut += _playButton_OnMouseOut;
			_playButton.OnClick += _playButton_OnClick;
			WindowPanel.Append(_playButton);
		}

		private void _playButton_OnClick(UIMouseEvent evt, UIElement listeningElement)
		{
			if (musicPlayer.IsPaused)
			{
				musicPlayer.Play();
			}
			else
			{
				musicPlayer.Pause();
			}
		}

		private void _playButton_OnMouseOut(UIMouseEvent evt, UIElement listeningElement)
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
			for (int i = musicPlayer.CurrentSong; i < 6; i++)
			{
				string text = string.Format("{0}: {1}", i + 1, Path.GetFileNameWithoutExtension(musicPlayer.SongNames[i % musicPlayer.SongNames.Count]));
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
			_progressBar.Value = factor;
		}

	}
}