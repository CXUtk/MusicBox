using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MusicBox.Utils
{
	public static class ModHelper
	{
		public static Vector2 Integerize(this Vector2 v)
		{
			return new Vector2((int)v.X, (int)v.Y);
		}
	}
}
