using MazeWorld.src.maze;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazeWorld.src.mode.maze
{
    public abstract class MazeMachine : Actor
    {
        public MazeMode Maze { get; set; }
        public bool Running { get; set; } = false;


        public MazeMachine(MazeMode m, Grid g, Location l) : base(Color.Yellow, g, l) { Maze = m; }
        public MazeMachine() : base() { }

        public void Start()
        {
            if(!Running)
                DoCustomStartWork();
            Running = true;
        }

        public void Finish()
        { 
            if (Running)
            {
                DoCustomFinishWork();

                this.MoveToSideline();
                Maze.FinishedWithPhase = true;
            }
            Running = false;
        }

        protected abstract void DoCustomStartWork();
        protected abstract void DoCustomFinishWork();
    }

    public abstract class MazeSolver : MazeMachine
    {
        public Location Target { get; set; }
        public MazeSolver(MazeMode m, Grid g, Location l) : base(m, g, l) { }
        public MazeSolver() : base() { }
    }

    public abstract class MazeGenerator : MazeMachine
    {
        public MazeGenerator(MazeMode m, Grid g, Location l) : base(m, g, l) { }
        public MazeGenerator() : base() { }
    }
}
