using System;
using System.IO;
using System.Collections.Generic;
using Terraria;
using Terraria.UI;
using Terraria.DataStructures;
using Terraria.GameContent.UI;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.Localization;
using System.Text;
using Terraria.ModLoader;
using ReLogic.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MusicBox.Utils;
using MusicBox.UI;
using MusicBox.Music;

namespace MusicBox
{
	// MOD的主类名字，需要与文件名、MOD名完全一致，并且继承Mod类
	public class MusicBox : Mod
	{
		public static MusicBox Instance;
		/// <summary>
		/// 存储所有在"Images/"下的图片
		/// </summary>
		public static Dictionary<string, Texture2D> ModTexturesTable = new Dictionary<string, Texture2D>();

		/// <summary>
		/// 鼠标显示的Tooltip，区别于原版，是UI系统的Tooltip
		/// </summary>
		public string ShowTooltip
		{
			get;
			set;
		}

		private CDInterfaceManager InterfaceManager;

		public MusicPlayer MusicPlayer { get; private set; }

		public ConditionalInterface musicUI;
		
		public bool CanShowMusicPlayUI
		{
			get;
			set;
		} 



		public MusicBox()
		{
			Properties = new ModProperties()
			{
				Autoload = true,
				AutoloadSounds = true,
			};

		}

		private void InitUI()
		{
			InterfaceManager = new CDInterfaceManager();
			musicUI = new ConditionalInterface(() => CanShowMusicPlayUI);
			musicUI.SetState(new UIPage.MusicPlayUI());
			InterfaceManager.Add(musicUI);
		}

		public override void UpdateUI(GameTime gameTime)
		{
			InterfaceManager.Update(gameTime);
		}

		public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
		{
			int MouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
			if (MouseTextIndex != -1)
			{
				layers.Insert(MouseTextIndex, new LegacyGameInterfaceLayer(
					"MusicBox: MusicPlayerUI",
					delegate
					{
						InterfaceManager.Draw(Main.spriteBatch);
						return true;
					},
					InterfaceScaleType.UI)
				);
			}
		}

		public override void Load()
		{
			Instance = this;
			if (!Main.dedServ)
			{
				HotKeyControl.RegisterKey();
			}
		}

		public override void PostSetupContent()
		{
			if (!Main.dedServ)
			{
				ConfigLoader.LoadCondig();
				MusicPlayer = new MusicPlayer(ConfigLoader.MusicConfig.MusicPath);
				ResourceLoader.LoadAllTextures();
				InitUI();
			}

		}

		public override void Unload()
		{
			MusicPlayer.Dispose();
			MusicPlayer = null;
		}

		public override void PreSaveAndQuit()
		{
			MusicPlayer.Stop();
			ConfigLoader.SaveConfig();
		}
	}

}
