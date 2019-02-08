using System;
using System.Collections.Generic;
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

		private MusicPlayer musicPlayer { get { return MusicBox.Instance.MusicPlayer;  } }

		private long _playPosition;
		private long _playLength;

		private Vector2 Center
		{
			get
			{
				return WindowPanel.GetDimensions().Center();
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
		}

		private void MusicPlayer_OnProgressUpdate(long pos, long length)
		{
			_playPosition = pos;
			_playLength = length;
		}

		protected override void OnClose(UIMouseEvent evt, UIElement listeningElement)
		{
			MusicBox.Instance.CanShowMusicPlayUI = false;
		}
		protected override void DrawChildren(SpriteBatch sb)
		{
			base.DrawChildren(sb);
			DrawProgressBar(sb);
		}

		private void DrawProgressBar(SpriteBatch sb)
		{
			Point barCenter = (Center + new Vector2(0, 100)).ToPoint();
			double factor = _playPosition / (double)_playLength;
			if (double.IsNaN(factor))
				factor = 0;
			sb.Draw(Main.magicPixel, new Rectangle(barCenter.X - 203, barCenter.Y - 12, 406, 24), Color.Gray);
			sb.Draw(Main.magicPixel, new Rectangle(barCenter.X - 200, barCenter.Y - 10, (int)(400 * factor), 20), Color.White);

			string text = string.Format("{0:N2}%", factor);
			Vector2 textSize = Main.fontMouseText.MeasureString(text);
			Terraria.Utils.DrawBorderStringFourWay(sb, Main.fontMouseText, text,
				barCenter.X - textSize.X * 0.5f, barCenter.Y + 6, Color.White, Color.Black, textSize * 0.5f);
		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);
		}

	}
}