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
	public class UISliderV : UIElement
    {
		public new event UIElement.MouseEvent OnMouseDown;
		public new event UIElement.MouseEvent OnMouseUp;
		public event ValueChangeEvent OnValueChange;

		/// <summary>
		/// 滑块贴图
		/// </summary>
		public Texture2D Texture { get; set; }


		public float Value { get; set; }
		public float Scale { get; set; }
		public bool Dragging { get { return _isDragging; } }
		public bool DragSync { get; set; }

		public float StartY { get; set; }
		public float EndY { get; set; }

		private bool _isDragging = false;

		public UISliderV()
        {
			Value = 0f;
			Scale = 1f;
        }

		public override void MouseDown(UIMouseEvent evt)
		{
			_isDragging = true;
			OnMouseDown?.Invoke(evt, this);
		}

		public override void MouseUp(UIMouseEvent evt)
		{
			if (_isDragging)
			{
				_isDragging = false;
				OnValueChange?.Invoke(Value, this);
			}
			OnMouseUp?.Invoke(evt, this);
		}


		public override void MouseOver(UIMouseEvent evt)
		{
			Main.PlaySound(12, -1, -1, 1, 1f, 0f);
			base.MouseOver(evt);
		}


		public override void Update(GameTime gameTime)
		{
			if (_isDragging)
			{
				var pivot = Main.MouseScreen.Y - Parent.GetInnerDimensions().Y;
				if (pivot > StartY)
				{
					Top.Set(StartY - Height.Pixels * 0.5f, 0f);
				}
				else if (pivot < EndY)
				{
					Top.Set(EndY - Height.Pixels * 0.5f, 0f);
				}
				else
				{
					Top.Set(pivot - Height.Pixels * 0.5f, 0f);
				}
				Value = (Top.Pixels - EndY + Height.Pixels * 0.5f) / (StartY - EndY);
				Recalculate();
				if (DragSync)
				{
					OnValueChange?.Invoke(Value, this);
				}
			}
			else
			{
				Top.Set(StartY + (StartY - EndY) * Value - Height.Pixels * 0.5f, 0f);
			}
			base.Update(gameTime);
		}

		protected override void DrawSelf(SpriteBatch sb)
		{
			base.DrawSelf(sb);
			var center = GetInnerDimensions().Center();
			sb.Draw(Texture, center, null, Color.White, 0, Texture.Size() * 0.5f, Scale, SpriteEffects.None, 0f);
		}
    }
}
