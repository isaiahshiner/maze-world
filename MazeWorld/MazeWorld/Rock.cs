﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazeWorld
{
    /* The wall tiles of MazeWorld. They will just sit there and do nothing.
     * They cannot Act, and simply serve as building blocks.
     */
    public class Rock : Entity
    {
        public Rock() { color = Color.DarkRed; }

        public Rock(Color c, Grid g, Location l) : base(g, l) { this.color = c; }

        //Randomizes colors, with a push towards being dark red.
        public override void RandomizeColor()
        {
            int r1 = (int)(grid.Rand.NextDouble() * 128), 
                r2 = (int)(grid.Rand.NextDouble() * 32), 
                r3 = (int)(grid.Rand.NextDouble() * 32);
            this.color = new Color(r1, r2, r3);
        }

    }
}
