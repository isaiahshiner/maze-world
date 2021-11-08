using System;

namespace MazeWorld
{
    /* The breadcrumb data class for the Depth First Search maze generator.
     * Has four states that can change throughout the generation process.
     * Also will Salt certain Rocks to be later removed, if grid.Salting == true.
     */
    public class DFScell : Rock
    {
        public int State { get; private set; }
        public bool Salted { get; set; } = false;
        public DFSgener Master { get; set; }

        //The Gener has never been here. It will always move to these places first.
        public const int Untouched = 0;

        //The Gener has been here at some point, but it is not yet ready to change to a path
        public const int Touched = 1;

        //These are placed anywhere that an Untouched block is adjacent to this and a Touched block
        public const int Rock = 2;

        //Created when a Touched cell is adjacent to only Rocks, other Paths, or this
        public const int Path = 3;

        public Color UntouchedColor { get; set; } = Color.Black;
        public Color TouchedColor { get; set; } = Color.White;
        public Color RockColor { get; set; } = Color.Black;
        public Color PathColor { get; set; } = Color.Blue;
        public Color SaltedColor { get; set; } = Color.DarkGreen;

        public DFScell(Grid g, Location l, int i, DFSgener m)
        {
            PutSelfInGrid(g, l);
            this.SetState(i);
            Master = m;
        }

        public DFScell(Grid g, Location l, DFSgener m) : this(g, l, DFScell.Untouched, m) { }

        /* Two things are given: 
         * 
         * The Gener is adjacent to this, and
         * This is an Untouched cell, which the Gener might try to moveTo.
         * 
         * Therefore, if any of the neighbors of this cell are Touched,
         * this must become a rock, so the Gener can't move here.
         * 
         * If the Gener did move here, it would get confused because it would
         * be adjacent to more than one Touched cell at a time.
         */
        public void UntouchedUpdate()
        {
            foreach (Entity e in Location.toEntities(Grid, Location.GetOrthogonalLocations(Grid)))
                if (e is DFScell)
                    if (((DFScell)(e)).State == DFScell.Touched)
                    {
                        this.SetState(Rock);
                        return;
                    }        
        }

        /* A Touched cell becomes a Path cell when a branch is closed.
         * This only happens when a Touched cell is surrounded by only Rocks, existing Paths or the Gener.
         */
        public void TouchedUpdate()
        {
            bool canBePath = true;
            foreach (Entity e in Location.toEntities(Grid, Location.GetOrthogonalLocations(Grid)))
                if(e is DFScell)
                    if ((((DFScell)(e)).State != DFScell.Rock && ((DFScell)(e)).State != DFScell.Path))
                        canBePath = false;

            if (canBePath)
                this.SetState(DFScell.Path);
        }

        //Changes state, Color, and randomly makes rocks salted.
        public void SetState(int s)
        {
            switch (s)
            {
                case 0:
                    this.Color = UntouchedColor;
                    break;
                case 1:
                    this.Color = TouchedColor;
                    break;
                case 2:
                    this.Color = RockColor;
                    if (Master.Maze.Salting && (int) (Grid.Rand.NextDouble() * (int)(1/Master.Maze.SaltFactor)) == 0) // (1/.25) = 4, 
                    {
                        Salted = true;
                        Color = SaltedColor;
                    }
                    break;
                case 3:
                    this.Color = PathColor;
                    break;
                default:
                    throw new NotImplementedException("Default of SetState was called");

            }
            this.State = s;
        }

        public override string ToString()
        {
            return base.ToString() + " State: " + this.State.ToString();
        }
    }
}
