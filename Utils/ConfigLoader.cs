using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusicBox.Config;
using System.IO;
using Newtonsoft.Json;
using Terraria;

namespace MusicBox.Utils
{
	public static class ConfigLoader
	{
		public static MusicConfig MusicConfig
		{
			get;
			set;
		}

		public static bool FirstTimeUse
		{
			get;
			set;
		}

		private const string CONFIG_FILE_NAME = "MusicBoxConfig.json";

		public static void LoadCondig()
		{
			MusicConfig = new MusicConfig();
			if (!Directory.Exists(Main.SavePath))
			{
				Directory.CreateDirectory(Main.SavePath);
			}
			string path = string.Concat(new object[]
			{
				Main.SavePath,
				Path.DirectorySeparatorChar,
				CONFIG_FILE_NAME
			});
			FirstTimeUse = false;
			if (File.Exists(path))
			{
				using (StreamReader r = new StreamReader(path))
				{
					string json = r.ReadToEnd();
					MusicConfig = JsonConvert.DeserializeObject<MusicConfig>(json);
				}
			}
			else
			{
				FirstTimeUse = true;
			}
		}

		public static void SaveConfig()
		{
			if (!Directory.Exists(Main.SavePath))
			{
				Directory.CreateDirectory(Main.SavePath);
			}
			string path = string.Concat(new object[]
			{
				Main.SavePath,
				Path.DirectorySeparatorChar,
				CONFIG_FILE_NAME
			});
			string json = JsonConvert.SerializeObject(MusicConfig, Formatting.Indented);
			File.WriteAllText(path, json);
		}
	}
}
