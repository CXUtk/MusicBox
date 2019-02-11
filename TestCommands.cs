using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace MusicBox
{
	public class StopCommand : CommandTemplate
	{
		public StopCommand()
		{
			name = "mbstop";
			argstr = "";
			desc = "STOP";
		}

		public override void Action(CommandCaller caller, string input, string[] args)
		{
			MusicBox.Instance.MusicPlayer.Stop();
		}
	}

	public class PlayCommand : CommandTemplate
	{
		public PlayCommand()
		{
			name = "mbplay";
			argstr = "";
			desc = "PLAY";
		}

		public override void Action(CommandCaller caller, string input, string[] args)
		{
			MusicBox.Instance.MusicPlayer.Play();
		}
	}

	public class PauseCommand : CommandTemplate
	{
		public PauseCommand()
		{
			name = "mbpause";
			argstr = "";
			desc = "PAUSE";
		}

		public override void Action(CommandCaller caller, string input, string[] args)
		{
			MusicBox.Instance.MusicPlayer.Pause();
		}
	}

	public class NextSongCommand : CommandTemplate
	{
		public NextSongCommand()
		{
			name = "mbnext";
			argstr = "";
			desc = "NEXT SONG";
		}

		public override void Action(CommandCaller caller, string input, string[] args)
		{
			MusicBox.Instance.MusicPlayer.SwitchNextSong();
		}
	}

	public class PrevSongCommand : CommandTemplate
	{
		public PrevSongCommand()
		{
			name = "mbprev";
			argstr = "";
			desc = "PREVIOUS SONG";
		}

		public override void Action(CommandCaller caller, string input, string[] args)
		{
			MusicBox.Instance.MusicPlayer.SwitchPrevSong();
		}
	}

	public class RandomSongCommand : CommandTemplate
	{
		public RandomSongCommand()
		{
			name = "mbrand";
			argstr = "";
			desc = "RANDOM SONG";
		}

		public override void Action(CommandCaller caller, string input, string[] args)
		{
			MusicBox.Instance.MusicPlayer.PlayRandom();
		}
	}

	public abstract class CommandTemplate : ModCommand
	{
		public string name, argstr, desc;

		public override CommandType Type => CommandType.Chat;

		public override string Command => name;

		public override string Usage => string.Format("/{0} {1}", name, argstr);

		public override string Description => string.Format("{0}", desc);
	}
}
