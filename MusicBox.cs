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

		private CDInterfaceManager CDInterfaceManager;

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
			CDInterfaceManager = new CDInterfaceManager();
		}

		public override void Load()
		{
			Instance = this;
			if (!Main.dedServ)
			{
				ResourceLoader.LoadAllTextures();
				InitUI();
				HotKeyControl.RegisterKey();
			}
		}

		
	}

}
