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
	public delegate void UIMouseEventHandler(UIElement target, Vector2 mousePosition);
	public class UIPicButton : UIElement
	{

		public Color ButtonDefaultColor { get; set; }
		/// <summary>
		/// 按钮的内部贴图
		/// </summary>
		public Texture2D Texture { get; set; }
		/// <summary>
		/// 鼠标移动到按钮上后显示的文本
		/// </summary>
		public string Tooltip { get; set; }

		public Rectangle? SourceRect
		{
			get;
			set;
		}

		public event UIMouseEventHandler OnMouseHover;

		public UIPicButton()
		{
			ButtonDefaultColor = Color.White;
			SourceRect = null;
		}


		public override void MouseOver(UIMouseEvent evt)
		{
			Main.PlaySound(12, -1, -1, 1, 1f, 0f);
			base.MouseOver(evt);
			OnMouseHover(evt.Target, evt.MousePosition);
		}


		public override void Update(GameTime gameTime)
		{
			if (Tooltip != "" && ContainsPoint(Main.MouseScreen))
			{
				MusicBox.Instance.ShowTooltip = Tooltip;
			}
			base.Update(gameTime);
		}


		protected override void DrawSelf(SpriteBatch sb)
		{
			CalculatedStyle innerDimension = GetInnerDimensions();

			if (Texture != null)
				sb.Draw(Texture, innerDimension.Center(), SourceRect, 
					ButtonDefaultColor, 0f, Texture.Size() * 0.5f, 1f, SpriteEffects.None, 0f);
		}
	}
}
