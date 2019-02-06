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


namespace MusicBox
{
	// MOD的主类名字，需要与文件名、MOD名完全一致，并且继承Mod类
	public class MusicBox : Mod
	{
		public static MusicBox Instance;

		
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
			if (!Main.dedServ)
			{
				
			}
		}

		
	}

}
