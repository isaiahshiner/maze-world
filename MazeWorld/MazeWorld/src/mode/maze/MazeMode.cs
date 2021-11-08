using MazeWorld.src.game;
using MazeWorld.src.mode.maze;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazeWorld.src.maze
{
    public class MazeMode : GameMode
    {
        public MazeGenerator Generator { get; }
        public MazeSolver Solver { get; }

        private int Speed = 0;
        private int HaltedFrames = 0;
        private bool Looping = true;
        private bool Auto = false;

        public bool Salting { get; set; } = false;//Whether the Gener should randomly remove some Rocks
        public float SaltFactor { get; set; } = .1f;//The chance that a Rock will be removed

        public bool FinishedWithPhase { get; set; } = false;
        private enum Phases {Generating = 1, Solving = 2}
        private Phases Phase;

        private int CellUpdateRate;

        public MazeMode(ControlsHelper c) : base(c)
        {
            Generator = new DFSgener(this, Grid, null);
            Solver = new BFSsolver(Grid, null, this, null);
        }

        public override void Update()
        {
            //Checks to see if the Maze should move to the next phase
            phaseCheck();

            if (Ctrl.KeyFalling(Keys.Right))
                Grid.Step();
            else if (Ctrl.KeyFalling(Keys.Space))
            {
                Grid.Step();
                HaltedFrames--;
            }
            else if (Ctrl.KeyDown(Keys.Space))
                Grid.Step(getStepRepeatCount());
            else if (Ctrl.KeyRising(Keys.Space))
                HaltedFrames = 0;
            else if (Ctrl.KeyFalling(Keys.R))
                Reset();
            else if (Ctrl.KeyFalling(Keys.T))
                generate();
            else if (Ctrl.KeyFalling(Keys.Y))
                solve();
            else if (Ctrl.KeyFalling(Keys.A))
                Auto = !Auto;
            else if (Auto)//If nothing else was pressed, then do the Auto check
                Grid.Step(getStepRepeatCount());

            //Changes speed
            if (Ctrl.KeyFalling(Keys.Up))
                Speed++;
            else if (Ctrl.KeyFalling(Keys.Down))
                Speed--;

            //Turns Salting on and off.
            //Salting will randomly mark some generated rocks to be removed.
            if (Ctrl.KeyFalling(Keys.S))
                Salting = true;

        }

        public override void Reset()
        {
            Generator.Finish();
            Solver.Finish();

            FinishedWithPhase = false;
            Phase = Phases.Generating;
            Looping = true;
            Auto = false;

            Grid.Reset();
            Generator.Start();
        }

        public override void Reset((int, int) maxSize)
        {
            Generator.Finish();
            Solver.Finish();

            FinishedWithPhase = false;
            Phase = Phases.Generating;
            Looping = true;
            Auto = false;

            this.Grid = new Grid(maxSize.Item1, maxSize.Item2);
            Generator.ForceGridChange(Grid);
            Solver.ForceGridChange(Grid);
            Solver.Target = new Location(Grid.MaxX - 1, Grid.MaxY - 1);
            Generator.Start();
        }

        public override string GetHeaderText()
        {
            String text;

            text = "Speed: " + Speed + " UpdateRate: " + CellUpdateRate + " Looping: " + Looping.ToString() + " Auto: " + Auto.ToString();
            if (Ctrl.MouseIsOnGrid())
            {
                Entity e = Grid.Get(Ctrl.LocationOfMouse());
                text += e == null ? "" : " Object:" + e.ToString();
            }

            return text;
        }

        public override void SlowDown(double slowFactor)
        {
            CellUpdateRate = (int)Math.Pow(slowFactor, -3);
            if (Solver is BFSsolver)
                ((BFSsolver)Solver).CellUpdateRate = CellUpdateRate;
        }

        private void phaseCheck()
        {
            if (Looping && FinishedWithPhase)
            {
                switch (Phase)
                {
                    case Phases.Generating:
                        FinishedWithPhase = false;
                        Phase = Phases.Solving;
                        Solver.Start();
                        break;
                    case Phases.Solving:
                        FinishedWithPhase = false;
                        Phase = Phases.Generating;
                        Grid.Reset();
                        Generator.Start();
                        break;
                }
            }
        }

        
        private void generate()
        {
            if (Phase != Phases.Generating)
            {
                Solver.Finish();

                Phase = Phases.Generating;
                Looping = false;
                Auto = false;

                Grid.Reset();
                Generator.Start();
            }
        }

        private void solve()
        {
            if (Phase != Phases.Solving)
            {
                Generator.Finish();

                Phase = Phases.Solving;
                Looping = false;
                Auto = false;

                Solver.Start();
            }
        }
        
        private int getStepRepeatCount()
        {
            if (HaltedFrames > 0)
            {
                HaltedFrames--;
                return 0;
            }
            else
            {
                if (Speed < 0)
                {
                    HaltedFrames = (int)Math.Pow(2, Math.Abs(Speed));
                    return 1;
                }
                else
                    return (int)Math.Pow(2, Speed);
            }
        }
    }
}
