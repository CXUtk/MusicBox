using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using System.IO;

namespace MusicBox.Utils
{
    public static class ResourceLoader
    {
		public static void LoadAllTextures()
		{
			MusicBox.ModTexturesTable.Clear();
			LoadTextures();
			Drawing.Box1 = MusicBox.ModTexturesTable["Box"];
			Drawing.Box2 = MusicBox.ModTexturesTable["Box2"];
			Drawing.Bar1 = MusicBox.ModTexturesTable["Bar"];

		}
        private static void LoadTexture(string name)
        {
			MusicBox.ModTexturesTable.Add(name.Substring(7), MusicBox.Instance.GetTexture(name));
		}
		//private static void LoadEffect(string name)
		//{
		//	MusicBox.ModEffectsTable.Add(name.Substring(8), MusicBox.Instance.GetEffect(name));
		//}
        private static void LoadTextures()
        {
			// 反射获取MOD的Textrues属性，用以获取图片
			IDictionary<string, Texture2D> textures = (IDictionary<string, Texture2D>)(typeof(Mod).GetField("textures", 
				System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).GetValue(MusicBox.Instance));
			var names = textures.Keys.Where((name) =>
			{
				return name.StartsWith("Images/");
			});
			foreach (var name in names)
                LoadTexture(name);
        }
		//private static void LoadEffects()
		//{
		//	IDictionary<string, Effect> textures = (IDictionary<string, Effect>)(typeof(Mod).GetField("effects",
		//		System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).GetValue(FallenStar49.Instance));
		//	var names = textures.Keys.Where((name) =>
		//	{
		//		return name.StartsWith("Effects/");
		//	});

		//	foreach (var name in names)
		//		LoadEffect(name);
		//}
		//private static void LoadGlowMasks(string[] names)
  //      {
  //          int oldMax = Main.glowMaskTexture.Length;
  //          Array.Resize(ref Main.glowMaskTexture, oldMax + names.Length);
  //          for(int i = 0; i < names.Length; i++)
  //          {
  //              Main.glowMaskTexture[i] = FallenStar49.Instance.GetTexture("Images/" + names[i]);
  //              FallenStar49.ModGlowTexTable[names[i]] = (short)i;
  //          }
  //      }        

    }
}
