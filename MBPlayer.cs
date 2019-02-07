using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria.ModLoader;
using ReLogic.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MusicBox.Utils;
using MusicBox.UI;
using Terraria.GameInput;
using Terraria;

namespace MusicBox
{
	public class MBPlayer : ModPlayer
	{
		public override void ProcessTriggers(TriggersSet triggersSet)
		{
			HotKeyControl.PressKey(triggersSet);
		}

		public override void OnEnterWorld(Player player)
		{
			base.OnEnterWorld(player);
		}

		public override void PostUpdate()
		{
			if (MusicBox.Instance.CanShowMusicPlayUI)
			{
				MusicBox.Instance.MusicPlayer.Play();
			}
		}
	}
}
