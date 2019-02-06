using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria;
using Terraria.ModLoader;

namespace MusicBox
{
	public class MusicBoxPlayer : ModPlayer
	{
		public override void OnEnterWorld(Player player)
		{
			MusicBox.Instance.MusicPlayer.Run();
		}
	}
}
