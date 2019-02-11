using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace MusicBox.UI
{
	public class UIAdvPanel : UIElement
	{
		public int CornerSize
		{
			get;
			set;
		}
		public Texture2D MainTexture
		{
			get;
			set;
		}
		public Color Color = new Color(63, 82, 151) * 0.7f;

		public UIAdvPanel(Texture2D texture = null)
		{
			CornerSize = 12;
			MainTexture = texture;
			base.SetPadding(CornerSize);
		}

		private void DrawPanel(SpriteBatch spriteBatch, Texture2D texture, Color color)
		{
			if (MainTexture == null)
				return;
			CalculatedStyle dimensions = GetDimensions();
			Drawing.DrawAdvBox(spriteBatch, (int)dimensions.X, (int)dimensions.Y, (int)dimensions.Width,(int)dimensions.Height,
				Color, MainTexture, new Vector2(CornerSize, CornerSize));
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			this.DrawPanel(spriteBatch, MainTexture, Color);
		}

	}
}
