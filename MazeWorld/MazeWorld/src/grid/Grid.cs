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

        /* Constructs a new Grid of size (X, Y) with 2 preinstantiated Geners and Solvers
         * PARAMETER: X and Y are positive.
         */
        public Grid(int maxX, int maxY)
        {
            if (maxX <= 0 || maxY <= 0)
                throw new ArgumentOutOfRangeException("Grid size must be positive.");

            grid = new Entity[maxX, maxY];
            registry = new List<Actor>();
            this.MaxX = maxX;
            this.MaxY = maxY;
        }

        public Grid((int, int) maxSize) : this(maxSize.Item1, maxSize.Item2) { }

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

        /* Removes all Entities from the Grid, leaving all Locations null.
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
                        e.RemoveSelfFromGrid();
                }
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

            //This is what doesn't work
            //registry.ForEach(actor => actor.Act());
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
