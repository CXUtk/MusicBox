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
	// TODO: What's wrong with this ui??? I don't get this
	public class TransparentFloatingDisplayerUI : FloatingUIState
	{
		private float width = 200f;
		private float height = 40f;

		private float speed = 2f;
		private Vector2 titlePadding;
		private Vector2 progressPadding;

		private MusicPlayer musicPlayer { get { return MusicBox.Instance.MusicPlayer; } }

		private Vector2 Center
		{
			get { return WindowPanel.GetDimensions().Center(); }
		}

		private TimeSpan playPosition;
		private TimeSpan playLength;

		protected override void Initialize(UIAdvPanel WindowPanel)
		{
			musicPlayer.OnProgressUpdate += MusicPlayer_OnProgressUpdate;
			base.Initialize(WindowPanel);
			height = MusicBox.NormalStringHeight * 2 + 20f;
			titlePadding = new Vector2(5f, 5f);
			progressPadding = titlePadding + new Vector2(0f, MusicBox.NormalStringHeight + 5f);
			WindowPanel.MainTexture = MusicBox.ModTexturesTable["AdvInvBack1"];	//TODO: DELETE THIS
			WindowPanel.SetPadding(0);
			WindowPanel.Left.Set(10f, 0f);
			WindowPanel.Top.Set(10f, 0f);
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
		}

		private void DrawTitle(SpriteBatch sb)
		{
			string title = Path.GetFileNameWithoutExtension(musicPlayer.SongNames[musicPlayer.CurrentSong]);
			Vector2 textSize = Main.fontMouseText.MeasureString(title);
			Vector2 place = Center - new Vector2(width / 2 - 5f, height / 2 - 5f);
			if (textSize.X <= width)
			{
				Terraria.Utils.DrawBorderStringFourWay(sb, Main.fontMouseText, title, place.X, place.Y,
					Color.White, Color.Black, textSize);
			}
			else
			{
				// TODO: rolling effect
				Terraria.Utils.DrawBorderStringFourWay(sb, Main.fontMouseText, title, place.X, place.Y,
					Color.White, Color.Black, textSize);
			}
		}

		private void DrawTime(SpriteBatch sb)
		{
			string text = string.Format("{0}/{1}", playPosition.ToString(@"mm\:ss"), playLength.ToString(@"mm\:ss"));
			Vector2 textSize = Main.fontMouseText.MeasureString(text);
			Vector2 place = Center - new Vector2(width / 2 - 5f, height / 2 - (5f + progressPadding.Y));
			Terraria.Utils.DrawBorderStringFourWay(sb, Main.fontMouseText, text,
				place.X, place.Y, Color.White, Color.Black, textSize);
		}
	}
}
