using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazeWorld.src.game
{
    public abstract class GameMode
    {
        public Grid Grid { get; set; }
        public ControlsHelper Ctrl { get; set; }

        public GameMode(ControlsHelper ctrl)
        {
            Ctrl = ctrl;
        }

        public abstract void Update();//Update Loop

        public abstract void Reset();//Set to default
        public abstract void Reset((int, int) maxSize);//Set to default and change the size of the Grid.

        public abstract String GetHeaderText();

        public abstract void SlowDown(double slowFactor);
    }
}
