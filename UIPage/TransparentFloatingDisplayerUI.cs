using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MusicBox.Music;
using MusicBox.UI;
using Terraria;

namespace MusicBox.UIPage
{
	public class TransparentFloatingDisplayerUI : FloatingUIState
	{
		private const float MIN_WIDTH = 140f;

		private float width = 230f;
		private float height = 40f;

		private float speed = 2f;
		private Vector2 titlePadding;
		private Vector2 progressPadding;

		private MusicPlayer musicPlayer { get { return MusicBox.Instance.MusicPlayer; } }

		private Vector2 TopLeft
		{
			get { return WindowPanel.GetDimensions().Center() - new Vector2(width / 2, height / 2); }
		}

		private TimeSpan playPosition;
		private TimeSpan playLength;

		protected override void Initialize(UIAdvPanel WindowPanel)
		{
			musicPlayer.OnProgressUpdate += MusicPlayer_OnProgressUpdate;
			base.Initialize(WindowPanel);
			height = MusicBox.NormalStringHeight * 2 + 40f;
			titlePadding = new Vector2(10f, 10f);
			progressPadding = titlePadding + new Vector2(0f, MusicBox.NormalStringHeight + 7f);
//			WindowPanel.MainTexture = MusicBox.ModTexturesTable["AdvInvBack1"];
			WindowPanel.SetPadding(0);
			WindowPanel.Left.Set(20f, 0f);
			WindowPanel.Top.Set(80f, 0f);
			WindowPanel.Width.Set(width, 0f);
			WindowPanel.Height.Set(height, 0f);
			WindowPanel.Color = Color.Transparent;
			WindowPanel.CornerSize = 12;
		}

		private void MusicPlayer_OnProgressUpdate(TimeSpan curPos, TimeSpan totalLen)
		{
			playPosition = curPos;
			playLength = totalLen;
		}

		protected override void DrawChildren(SpriteBatch sb)
		{
			base.DrawChildren(sb);
			DrawTitle(sb);
			DrawTime(sb);
			UpdateWidth();
		}

		private void UpdateWidth()
		{
			width = Main.fontMouseText.MeasureString(musicPlayer.NowPlaying).X;
			WindowPanel.Width.Set(MathHelper.Max(width, MIN_WIDTH), 0f);
		}

		private void DrawTitle(SpriteBatch sb)
		{
			string title = musicPlayer.NowPlaying;
			Vector2 textSize = Main.fontMouseText.MeasureString(title);
			float placeX = TopLeft.X + titlePadding.X + textSize.X;
			float placeY = TopLeft.Y + titlePadding.Y + textSize.Y;
			Terraria.Utils.DrawBorderStringFourWay(sb, Main.fontMouseText, title, placeX, placeY,
				Color.White, Color.Black, textSize);	// TODO: if too long, start rolling.
		}

		private void DrawTime(SpriteBatch sb)
		{
			string text = string.Format("{0}/{1}", playPosition.ToString(@"mm\:ss"), playLength.ToString(@"mm\:ss"));
			Vector2 textSize = Main.fontMouseText.MeasureString(text);
			float placeX = TopLeft.X + progressPadding.X + textSize.X;
			float placeY = TopLeft.Y + progressPadding.Y + textSize.Y;
			Terraria.Utils.DrawBorderStringFourWay(sb, Main.fontMouseText, text,
				placeX, placeY, Color.White, Color.Black, textSize);
		}
	}
}
