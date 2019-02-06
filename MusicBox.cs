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
using MusicBox.Music;


namespace MusicBox
{
	// MOD的主类名字，需要与文件名、MOD名完全一致，并且继承Mod类
	public class MusicBox : Mod
	{
		public const string TEST_MUSIC_FOLDER = @"D:\CloudMusic";

		public static MusicBox Instance;

		public MusicPlayHandler MusicPlayer { get; private set; }
		
		public MusicBox()
		{
			Properties = new ModProperties()
			{

				Autoload = true,
				AutoloadSounds = true,
			};

		}

		public override void Load()
		{
			Instance = this;
			MusicPlayer = new MusicPlayHandler(TEST_MUSIC_FOLDER);

			if (!Main.dedServ)
			{
				
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
		}
	}

}
