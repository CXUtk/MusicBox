using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria.UI;

namespace MusicBox.UI
{
	public class ConditionalInterface : UserInterface
	{
		private Func<bool> _predicate;
		public ConditionalInterface(Func<bool> predicate) : base()
		{
			_predicate = predicate;
		}

		public bool CanShow()
		{
			return _predicate.Invoke();
		}

	}
}
