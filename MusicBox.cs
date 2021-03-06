﻿using System;
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
using MusicBox.UIPage;

namespace MusicBox
{
	// MOD的主类名字，需要与文件名、MOD名完全一致，并且继承Mod类
	public class MusicBox : Mod
	{
		public static MusicBox Instance;

		public static float NormalStringHeight;

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
		public ConditionalInterface floatingDisplayUI;
		
		public bool CanShowMusicPlayUI
		{
			get;
			set;
		}

		public bool IsRunning { get; private set; }



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
			musicUI.SetState(new MusicPlayUI());

			floatingDisplayUI = new ConditionalInterface(() => true);
			floatingDisplayUI.SetState(new TransparentFloatingDisplayerUI());

			InterfaceManager.Add(musicUI);
			// InterfaceManager.Add(floatingDisplayUI);
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
			IsRunning = false;
			if (!Main.dedServ)
			{
				NormalStringHeight = Main.fontMouseText.MeasureString("hi").Y;
				HotKeyControl.RegisterKey();
			}
		}

		public override void PostSetupContent()
		{
			if (!Main.dedServ)
			{
				SetNewMusicPlayer();
				ResourceLoader.LoadAllTextures();
				InitUI();
			}

		}

		public void SetNewMusicPlayer()
		{
			ConfigLoader.LoadConfig();
			MusicPlayer?.Dispose();
			MusicPlayer = new MusicPlayer
			{
				Volume = ConfigLoader.MusicConfig.Volume / 100f
			};
			MusicPlayer.OnMusicEnd += (sender, args) => (sender as MusicPlayer).SwitchNextSong();
			if (!ConfigLoader.FirstTimeUse)
			{
				MusicPlayer.ResetSrc(ConfigLoader.MusicConfig.MusicPath);
			}
			IsRunning = true;
			
		}

		public override void Unload()
		{
			// MessageBox.Show("Unload");
			MusicPlayer.Dispose();
			IsRunning = false;
		}

		public override void PreSaveAndQuit()
		{
			MusicPlayer.Stop();
			ConfigLoader.SaveConfig();
		}
	}

}
