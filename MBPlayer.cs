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
using System.Windows.Forms;
using System.Threading;

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
			if (ConfigLoader.FirstTimeUse)
			{
				DialogResult InvokeResult = DialogResult.None;
				string selectedPath = string.Empty;
				var InvokeThread = new Thread(new ThreadStart(() =>
				{
					var f2 = new Form() { TopMost = true, Visible = false };
					FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog()
					{
						Description = "请选择音乐文件夹"
					};
					InvokeResult = folderBrowserDialog.ShowDialog(f2);
					if (InvokeResult == DialogResult.OK)
					{
						selectedPath = folderBrowserDialog.SelectedPath;
					}
				}));
				InvokeThread.SetApartmentState(ApartmentState.STA);
				InvokeThread.Start();
				InvokeThread.Join();
				// 让玩家选择路径
				if (InvokeResult == DialogResult.OK)
				{
					MusicBox.Instance.MusicPlayer.ResetSrc(selectedPath);
				}
			}
			base.OnEnterWorld(player);
		}


		public override void PostUpdate()
		{
			if (MusicBox.Instance.CanShowMusicPlayUI)
			{
				if (!MusicBox.Instance.IsRunning)
				{
					MusicBox.Instance.SetNewMusicPlayer();
				}
				MusicBox.Instance.MusicPlayer.Play();
			}
		}
	}
}
