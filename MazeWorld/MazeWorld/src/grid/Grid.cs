using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazeWorld
{
    /*
     * Represents a Grid of Entities using a nested 2D array.
     * Includes methods for generating and solving itself.
     * Works as the relay between all Entities in itself-
     * and the controlling Game class.
     */
    public class Grid
    {
        private Entity[,] grid;//Standard primitive 2D object array.

        public int MaxX { get; }//Total number of rows in the Grid
        public int MaxY { get; }//Total number of columns in the Grid

        //Contains only the Actors in the Grid. Updated by Accesor methods.
        //Prevents need to search the entire Grid for Actors on every "FStep" call.
        private List<Actor> registry;

        //Central Random object used by all Entities.
        //Seed can be changed by grid.Rand = new Random(SEED);
        public Random Rand { get; set; } = new Random(); 

        /* The standard Geners and Solvers used by this Grid.
         * 
         * TODO: change to a generic system that does not require an instance
         * of the object to be created by the Game class and then held indefinetley.
         * This will be needed when other Geners and Solvers are added.
         */
        public Actor MazeGener { get; }
        public Actor MazeSolver { get; }

        public bool Salting { get; set; } = false;//Whether the Gener should randomly remove some Rocks
        public float SaltFactor { get; set; } = .1f;//The chance that a Rock will be removed

        public bool FinishedWithStep { get; set; } = true;//A messanger variable to allow the Geners and Solvers
                                                          //to communicate with the Game object.

        /* Constructs a new Grid of size (X, Y) with 2 preinstantiated Geners and Solvers
         * PARAMETER: X and Y are positive.
         * PARAMETER: Gener and Solver are != null;
         * POSTCONDITION: Gener and Solver Grid reference = this;
         * POSTCONDITION: Gener and Solver Location reference = null;
         */
        public Grid(int x, int y, Actor gener, Actor solver)
        {
            if (x <= 0 || y <= 0)
                throw new ArgumentOutOfRangeException("Grid size must be positive.");
            if (gener == null || solver == null)
                throw new ArgumentNullException("Geners and solvers must be preinstantiated.");

            grid = new Entity[x, y];
            registry = new List<Actor>();
            this.MaxX = x;
            this.MaxY = y;

            this.MazeGener = gener;
            this.MazeSolver = solver;

            MazeSolver.ForceGridChange(this);//Changes their Grid reference to this
            MazeGener.ForceGridChange(this);

            MazeSolver.MoveToSideline();//Changes their Location reference to null
            MazeGener.MoveToSideline();
        }

        /* Returns the object in the Grid at (x, y). Returns null if there is nothing there.
         * PRECONDITION: IsValid(x, y) == true;
         */
        public Entity Get(int x, int y)
        {
            return grid[x, y];
        }

        /* Assigns (x, y) to be equal to "e". "e" can be null.
         * Returns the object in the Grid at (x, y). Returns null if there is nothing there.
         * Will also change Registry accordingly
         * 
         * PRECONDITION: IsValid(x, y) == true;
         * POSTCONDITION: this.Get(x, y).equals(e) == true;
         * POSTCONDITION: if(removed is Actor) -> Registry.Contains(removed) == false;
         * POSTCONDITION: if(e is Actor) -> Registry.Contains(e) == true;
         */
        public Entity Set(Entity e, int x, int y)
        {
            Entity removed = grid[x, y];
            if (removed is Actor)
                registry.Remove((Actor)(removed));
            grid[x, y] = e;
            if (e is Actor)
                registry.Add(((Actor)(e)));
            return removed;
        }

        /* Assigns (x, y) to be null
         * Will also change Registry accordingly
         * 
         * PRECONDITION: IsValid(x, y) == true;
         * POSTCONDITION: this.get(x, y) == null;
         * POSTCONDITION: if(e is Actor) -> Registry.Contains(e) == false;
         */
        public Entity Remove(int x, int y)
        {
            Entity e = this.Get(x, y);
            grid[x, y] = null;
            if (e is Actor)
                registry.Remove(((Actor)(e)));
            return e;
        }

        //Returns true if (x, y) can be used with this Grid without throwing IndexOutOfRangeException
        public bool IsValid(int x, int y)
        {
            bool flag = true;
            if (x < 0 || x >= this.MaxX)
                flag = false;
            if (y < 0 || y >= this.MaxY)
                flag = false;

            return flag;
        }

        //A set of overloaded versions of the accesor methods to allow use with the Location class.
        public Entity Get(Location l) { return grid[l.X, l.Y]; }
        public Entity Set(Entity e, Location l) { return this.Set(e, l.X, l.Y); }
        public Entity Remove(Location l) { return this.Remove(l.X, l.Y); }
        public bool IsValid(Location l)
        {
            if (l != null)
                return this.IsValid(l.X, l.Y);
            else
                return false;
        }

        /* Moves the Gener into place so that it can begin generating the maze.
         * Currently, the Gener is in the bottom right corner of the Grid. 
         * This can be changed later, but is largely inconsequential.
         * 
         * PRECONDITION: this.Reset() was just called, or equivalent. Grid should be clear of objects.
         * PRECONDITION: MazeGener.Finish() was just called, or equivalent. Gener should not think its still generating.
         * POSTCONDITION: grid.Get(grid.MaxX - 1, grid.MaxY - 1).Equals(MazeGener) == true;
         */
        public void Scramble()
        {
            MazeGener.MoveFromSideline(this.MaxX - 1, this.MaxY - 1);
        }

        /* Finds the first empty spot in the Grid starting from the top left and places the solver there.
         * Also contains non-generic code to set the target of the BFSsolver.
         * This approach should be changed to something more generic, perhaps an abstract MazeSolver class?
         * 
         * PRECONDITION: MazeSolver.Finish() was just called, or equivalent. Solver should not think its still solving.
         * PRECONDITION: Grid should have at least one path to (MaxX - 1, MaxY - 1).
         * POSTCONDITION: a MazeSolver should exist somewhere in the Grid.
         * 
         * TODO: Remove valid path requirement. Make it so that Solvers can fail.
         */
        public void Solve()
        {
            if (MazeSolver is BFSsolver)
                ((BFSsolver)(MazeSolver)).Target = new Location(MaxX - 1, MaxY - 1);


            for (int i = 0; i < MaxX; i++)
                for(int j = 0; j < MaxY; j++)
                    if (this.Get(i, j) == null)
                    {
                        MazeSolver.MoveFromSideline(i, j);
                        return;
                    }
        }

        /* Removes all Entities from the Grid, leaving all Locations null.
         * Saves the MazeGener and MazeSolver and moves them to the sideline instead.
         * 
         * POSTCONDITION: this.Get(any, any) == null;
         */
        public void Reset()
        {
            for (int i = 0; i < MaxX; i++)
                for (int j = 0; j < MaxY; j++)
                {
                    Entity e = this.Get(i, j);
                    if(e != null)
                        if (!(e == MazeGener || e == MazeSolver))
                            e.RemoveSelfFromGrid();
                }
            MazeGener.MoveToSideline();
            MazeSolver.MoveToSideline();
        }

        /* Calls Act on all Actors in the Grid. 
         * Uses Registry to reference Actors without searching through the Grid.
         * 
         * TODO: Research a proper Collections solution of iterating over the Registry-
         * without needing a secondary array. I don't actually know if that exists or not.
         */ 
        public void Step()
        {
            Actor[] actors = new Actor[registry.Count];
            registry.CopyTo(actors);

            for (int i = 0; i < actors.Length; i++)
                actors[i].Act();
        }

        /* Overloaded method to call Step repeatedly.
         * 
         * WARNING: A high number (greater than ~1000) of Step calls will cause lag.
         */
        public void Step(int times)
        {
            for (int i = 0; i < times; i++)
                this.Step();
        }

        /* Searches the Grid and resets the Registry to accurately reflect what is in it.
         * Currently no known use case, may later be removed.
         */
        public void FixRegistry()
        {
            List<Actor> actors = new List<Actor>();

            for (int i = 0; i < MaxX; i++)
                for (int j = 0; j < MaxY; j++)
                    if (this.Get(i, j) != null)
                        if (this.Get(i, j) is Actor)
                            actors.Add((Actor)this.Get(i, j));

            registry = actors;
        }
        

        //Standard ToString. WARNING: Very long String. Will cause Lag if printed to Debug.
        public override string ToString()
        {
            String str = "Grid size: " + MaxX + " rows " + MaxY + "Columns\n";
            for(int i = 0; i<MaxX; i++)
                for(int j =0; j<MaxY; j++)
                {
                    if(this.Get(i, j) == null)
                        str += "Row: " + i + " Column: " + j + " null\n";
                    else
                        str += "Row: " + i + " Column: " + j + " " + this.Get(i, j).ToString() + "\n";
                }
            str += "\nRegistry:\n";
            foreach (Actor a in registry)
                str += a.ToString() + "\n";
            return str;
        }
    }
}
