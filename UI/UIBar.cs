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
    public class UIBar : UIElement
    {


        public UIBar()
        {
			FillerColor = Color.White;
        }

		public Texture2D BarFrameTex
		{
			get;
			set;
		}


		public Vector2 BarFrameTexCornerSize
		{
			get;
			set;
		}

		public Texture2D BarFillTex
		{
			get;
			set;
		}

		public float Value
		{
			get;
			set;
		}

		public Vector2 FillerDrawOffset
		{
			get;
			set;
		}

		public Vector2 FillerSize
		{
			get;
			set;
		}

		public Color FillerColor
		{
			get;
			set;
		}

		public Vector2 ValueCenter
		{
			get
			{
				return GetInnerDimensions().Position() + FillerDrawOffset + new Vector2(FillerSize.X * Value, FillerSize.Y * 0.5f);
			}
		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);
		}

		protected override void DrawSelf(SpriteBatch sb)
		{
			base.DrawSelf(sb);
			var pos = GetInnerDimensions().Position();
			var fillpos = pos + FillerDrawOffset;
			sb.Draw(Main.magicPixel,
				new Rectangle((int)fillpos.X, (int)fillpos.Y, (int)(FillerSize.X), (int)FillerSize.Y), Color.Black);
			sb.Draw(BarFillTex,
				new Rectangle((int)fillpos.X, (int)fillpos.Y, (int)(FillerSize.X * Value), (int)FillerSize.Y), FillerColor);
			Drawing.DrawAdvBox(sb, GetInnerDimensions().ToRectangle(), Color.White, BarFrameTex, BarFrameTexCornerSize);

		}
    }
}
