﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicBox.Config
{
	public class MusicConfig
	{
		public string MusicPath;
		public bool MuteVanillaMusic;
		public int Volume;

		public MusicConfig()
		{
			Volume = 70;
		}
	}
}
