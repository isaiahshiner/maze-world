using System;
using System.Collections.Generic;

namespace MazeWorld
{
    /* A utility class for defining a 2D point and easily interacting with it.
     * Includes many methods for finding other Locations near to this one.
     * 
     * TODO: Fix Grid parameter requirement. You shouldn't have to pass the grid-
     * in every method, but I don't think this object should have a Grid reference either.
     * This class absolutely needs the most work out of all the framework classes.
     */
    public class Location
    {
        public int X { get; }
        public int Y { get; }

        //A set of cardinal direction constants for Actors that use direction.
        public const int North = 0;
        public const int NorthEast = 45;
        public const int East = 90;
        public const int SouthEast = 135;
        public const int South = 180;
        public const int SouthWest = 225;
        public const int West = 270;
        public const int NorthWest = 315;

        //Constructs a new Location with Coordinates (x, y)
        public Location(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        /* Finds the the Location that is directly adjacent in any of the eight cardinal directions.
         * The main way that new Locations are created and validated, other than construction.
         * 
         * PARAMETER: Direction is a multiple of 45. Can be positive or negative.
         * Example: 3150 is 8 times around the circle, plus another 270.
         * 3150 % 360 = 270 = West
         * -2025 % 360 = -225, -225 + 360 = 135 = SouthEast
         */
        public Location GetAdjacentLocation(int direction)
        {
            Location l;
            int i = this.X;
            int j = this.Y;

            if (direction % 45 != 0)
                throw new Exception("Direction must be a multiple of 45 degrees.");

            if (direction > 315)
                direction %= 360;

            else if (direction < 0)
                direction = (direction % 360) + 360;

            switch (direction)
            {
                case North:
                    l = new Location(i, j + 1);
                    break;
                case NorthEast:
                    l = new Location(i + 1, j + 1);
                    break;
                case East:
                    l = new Location(i + 1, j);
                    break;
                case SouthEast:
                    l = new Location(i + 1, j - 1);
                    break;
                case South:
                    l = new Location(i, j - 1);
                    break;
                case SouthWest:
                    l = new Location(i - 1, j - 1);
                    break;
                case West:
                    l = new Location(i - 1, j);
                    break;
                case NorthWest:
                    l = new Location(i - 1, j + 1);
                    break;
                default:
                    throw new NotImplementedException("Default of GetAdjacentLocation was called.");
            }
            return l;
        }

        /* Private helper that acts as a highly variable for loop.
         * Checks all Locations dictated by its loop values and returns the valid ones in a list.
         */
        private List<Location> GetLocations(int initial, int final, int step, Grid g)
        {
            List<Location> locs = new List<Location>();
            for (int i = initial; i <= final; i += step)
            {
                Location adj = this.GetAdjacentLocation(i);
                if (g.IsValid(adj))
                    locs.Add(adj);
            }
            return locs;
        }

        //A series of simple one line methods to utilize the private GetLocations method.
        //NOTE: Direct =  Orthogonal
        public List<Location> GetAllLocations(Grid g) { return GetLocations(0, 315, 45, g); }
        public List<Location> GetOrthogonalLocations(Grid g) { return GetLocations(0, 270, 90, g); }
        public List<Location> getDiagonalLocations(Grid g) { return GetLocations(45, 315, 90, g); }


        /* Two methods to latch on to the GetSomethingLocations methods.
         * Removes either only the null or not null Locations
         * 
         * TODO: clean these so that they can be written on one line, or similar.
         * Somehow, these methods still feel ugly to me.
         */
        public List<Location> getEmptyLocations(Grid g, List<Location> locs)
        {
            locs.RemoveAll(l => g.Get(l) != null);
            return locs;
        }

        public List<Location> getFullLocations(Grid g, List<Location> locs)
        {
            locs.RemoveAll(l => g.Get(l) == null);
            return locs;
        }

        /* Converts a list of Locations to a list of Entities.
         * 
         * TODO: Should this method really be static? Maybe all of the methods in this class should be static?
         */
        public static List<Entity> toEntities(Grid g, List<Location> locs)
        {
            List<Entity> Entities = new List<Entity>();

            foreach (Location l in locs)
                Entities.Add(g.Get(l));

            return Entities;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Location))
                return false;
            else if (((Location)(obj)).X != this.X)
                return false;
            else if (((Location)(obj)).Y != this.Y)
                return false;
            else
                return true;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 37;

                hash = hash * 89 + X.GetHashCode();
                hash = hash * 89 + Y.GetHashCode();
                return hash;
            }
        }

        public override string ToString()
        {
            String str = "X:" + X + " Y:" + Y;
            return str;
        }
    }
}
