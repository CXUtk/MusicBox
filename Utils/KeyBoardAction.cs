using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace FallenStar49
{
    public struct KeyBoardAction
    {
        public Microsoft.Xna.Framework.Input.Keys pressKey;
        /// <summary>
        /// Function called when key is pressed
        /// </summary>
        public Action keyPressAction;
        public KeyBoardAction(Microsoft.Xna.Framework.Input.Keys key, Action pressAction)
        {
            pressKey = key;
            keyPressAction = pressAction;
        }
    }
}
