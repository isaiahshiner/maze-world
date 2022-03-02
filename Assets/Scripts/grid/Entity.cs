using System;

namespace MazeWorld
{
    /* Represents the basic functionality required for an object to exist in the Grid.
     * Includes the core methods for moving Entities around.
     */
    public abstract class Entity
    {
        public Color Color { get; set; } = Color.White;//XNA Framework Color.
        public Grid Grid { get; private set; }//Reference to the main Grid.
        public Location Location { get; private set; }//(X, Y) coordinates.

        /* Constructs an Entity with a Grid and Location.
         * The Entity will NOT put itself into the Grid automatically.
         * grid.Set(this, location) or this.PutSelfInGrid() should be called.
         * 
         * PARAMETER: l.IsValid() == true;
         * POSTCONDITION: g.Get(l).Equals(this) == true;
         */
        public Entity(Grid g, Location l)
        {
            this.Grid = g;
            this.Location = l;
        }

        //Constructs an Entity with null values. Use caution.
        public Entity() : this(null, null) { }

        /* Moves this Entity to another Location within the Grid.
         * Removes whatever was at its original position.
         * Returns the Entity that was originally occupying that Location, null if there was nothing there.
         * 
         * PRECONDITION: Grid and Location != null;
         * PARAMETER: grid.IsValid(x, y) == true;
         * POSTCONDITION: grid.Get(x, y) == this;
         * POSTCONDITION: grid.Get(oldLoc) == null;
         */
        public Entity MoveTo(int x, int y)
        {
            if (Grid.IsValid(x, y))
            {
                Grid.Remove(Location);
                this.Location = new Location(x, y);
                return Grid.Set(this, this.Location);
            }
            else
                throw new Exception("Location " + x + ", " + y + "was invalid!");
        }

        //Overloaded method to make MoveTo work with the Location class.
        public Entity MoveTo(Location l) { return this.MoveTo(l.X, l.Y); }

        //A Set method for Grid, but named to make it clear this should not be used lightly.
        public void ForceGridChange(Grid g) { this.Grid = g; }

        /* Moves this Entity into the Grid, and adjusts its Grid reference accordingly.
         * 
         * PARAMETER: g and l != null and g.IsValid() == true;
         * POSTCONDITION: this.grid == g and grid.Get(l) == this
         */
        public void PutSelfInGrid(Grid g, Location l)
        {
            this.ForceGridChange(g);
            MoveFromSideline(l);
        }

        /* Removes all references of this object so it can be garbage collected.
         * 
         * PRECONDITION: grid and location are != null;
         * POSTCONDITION: grid.Get(location) == null;
         */
        public void RemoveSelfFromGrid()
        {
            Grid.Remove(Location);
            this.ForceGridChange(null);
        }

        /* Removes this Entity from the grid without changing its Grid reference.
         * NOTE: Sideline is not a real list, its just an idea.
         * Useful for important Entities like Geners and Solvers that you need to keep instantiated.
         * 
         * PRECONDITION: Grid is not null
         * POSTCONDITION: location == null and Grid does not contain this.
         */
        public void MoveToSideline()
        {
            if (Location != null)
                Grid.Remove(Location);
        }

        /* Moves an Entity into the Grid if it's location is null.
         * 
         * PRECONDITION: grid != null;
         * PRECONDITION: this.location == null;
         * PARAMETER: loc must be valid in the Grid
         * POSTCONDITION: grid.Get(location) == this;
         */
        public void MoveFromSideline(int x, int y)
        {
            if (Grid.IsValid(x, y))
            {
                Grid.Set(this, x, y);
                this.Location = new Location(x, y);
            }
            else
                throw new ArgumentException("Given Location must be Valid!");
        }

        public void MoveFromSideline(Location l) { this.MoveFromSideline(l.X, l.Y); }

        public abstract void RandomizeColor();

        public override String ToString()
        {
            return this.GetType() + ", " + this.Color.ToString();
        }
    }
}
