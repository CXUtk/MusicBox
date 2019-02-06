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

namespace MusicBox
{
	public class TurretConfigUI : AdvWindowUIState
	{   
		private const float UI_PANEL_WIDTH = 480f;
		private const float UI_PANEL_HEIGHT = 360f;

		protected override void Initialize(UIAdvPanel WindowPanel)
		{
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

		protected override void OnClose(UIMouseEvent evt, UIElement listeningElement)
		{
			MusicBox.Instance.CanShowMusicPlayUI = false;
		}
		protected override void DrawChildren(SpriteBatch sb)
		{
			base.DrawChildren(sb);
		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);
		}

		//public override void OnInitialize()
		//{

		//	Texture2D closeTex = FallenStar49.ModTexturesTable["CloseButton"];
		//	UIButton dclose = new UIButton(closeTex, false);
		//	dclose.Left.Set(-30f, 1f);
		//	dclose.Top.Set(10f, 0f);
		//	dclose.Width.Set(20f, 0f);
		//	dclose.Height.Set(20f, 0f);
		//	dclose.ButtonDefaultColor = Color.White;
		//	dclose.ButtonChangeColor = Color.White * 0f;
		//	dclose.OnClick += Close_OnClick;
		//	dclose.Tooltip = "关闭";
		//	WindowPanel.Append(dclose);

		//	base.Append(WindowPanel);
		//}

	}
}