using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicBox.Music
{
	public delegate void UpdateProgressEventHandler(TimeSpan curPos, TimeSpan totalLen);
}
