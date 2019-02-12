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
	public delegate void ValueChangeEvent(float value, UIElement sender);
	public class UISliderH : UIElement
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

		public float StartX { get; set; }
		public float EndX { get; set; }

		private bool _isDragging = false;
		private Vector2 _pivotOffset;

		public UISliderH()
        {
			Value = 0f;
			Scale = 1f;
        }

		public void SetPivot()
		{
			Recalculate();

			_pivotOffset = GetInnerDimensions().Center() - Parent.GetInnerDimensions().Center();
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
				var pivot = Main.MouseScreen.X - Parent.GetInnerDimensions().X;
				if (pivot < StartX)
				{
					Left.Set(StartX - Width.Pixels * 0.5f, 0f);
				}
				else if (pivot > EndX)
				{
					Left.Set(EndX - Width.Pixels * 0.5f, 0f);
				}
				else
				{
					Left.Set(pivot - Width.Pixels * 0.5f, 0f);
				}
				Value = (Left.Pixels - StartX + Width.Pixels * 0.5f) / (EndX - StartX);
				Recalculate();
			}
			else
			{
				Left.Set(StartX + (EndX - StartX) * Value - Width.Pixels * 0.5f, 0f);
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
