using MazeWorld.src.game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazeWorld.src.mode.labyrinth
{
    public class LabyrinthMode : GameMode
    {
        public LabyrinthMode(ControlsHelper ctrl) : base(ctrl) { }

        public override void Update()
        {
            throw new NotImplementedException();
        }

        public override void Reset()
        {
            throw new NotImplementedException();
        }

        public override void Reset((int, int) maxSize)
        {
            throw new NotImplementedException();
        }

        public override string GetHeaderText()
        {
            throw new NotImplementedException();
        }

        public override void SlowDown(double slowFactor)
        {
            throw new NotImplementedException();
        }
    }
}
