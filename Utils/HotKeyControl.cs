using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.Localization;
using Terraria.ID;
using System.IO;
using Terraria.GameInput;

namespace MusicBox.Utils
{
    public static class HotKeyControl
    {
		private static ModHotKey ShowMusicUIs;
		public static void RegisterKey()
        {
			ShowMusicUIs = MusicBox.Instance.RegisterHotKey("打开音乐播放界面", "Z");
        }

		public static void PressKey(TriggersSet triggersSet)
		{
			if (ShowMusicUIs.JustPressed)
			{
				MusicBox.Instance.CanShowMusicPlayUI ^= true;
			}
		}
    }
}
