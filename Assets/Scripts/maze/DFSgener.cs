using MazeWorld.src.maze;
using MazeWorld.src.mode.maze;
using System.Collections.Generic;
using System.Linq;

namespace MazeWorld
{
    /* This actor constructs a maze using randomized Depth First Search.
     * Using DFScells as breadcrumbs, the Actor will walk randomly around the Grid.
     * When it reaches a dead end, either by running into itself or the wall,
     * it will walk back along that path and then look for somewhere else it hasn't been,
     * until it ends all the way back where it started.
     */
    public class DFSgener : MazeGenerator
    {
        private bool First = true;//Used to fill the Grid on first step.

        public DFSgener(MazeMode m, Grid g, Location l) : base(m, g, l) { }
        public DFSgener() : this(null, null, null) { }

        /* Calls on all the methods required to generate the maze.
         * The most significant logic occurs in this.GetMoveLocations() and DFScell.Update()
         * PopulateGrid() fills the Grid with State 1 DFScells
         * Move and Process are in reversed order to make the way it renders look better,
         * but the logic is actually fully reversable.
         * Move then Process, or Process then Move, both work.
         */
        public override void Act()
        {
            if (this.First)
            {
                this.PopulateGrid();
                this.First = false;
            }
            this.MakeMove(this.ChooseMoveLocation(this.GetMoveLocations()));
            this.ProcessEntities(this.GetEntities());
        }

        //Standard Location methods, Orthogonal Entities only.
        public override List<Entity> GetEntities()
        {
            return Location.toEntities(Grid, Location.GetOrthogonalLocations(Grid));
        }

        protected override void DoCustomStartWork()
        {
            this.MoveFromSideline(Grid.MaxX - 1, Grid.MaxY - 1);
        }

        protected override void DoCustomFinishWork()
        {
            this.RemoveJunkFromGrid();
            First = true;
        }

        /* Calls Update on surrounding cells.
         * Update is called on Untouched or Touched cells, 
         * but never on Rock or Path blocks, which are already final
         * and do not need to Update.
         * 
         * NOTE: This foreachs over all 4 directly adjacent Entities,
         * and then Update() foreachs over that cells 4 Entities, one of them being this.
         * That means this will ultimately check up to 2 Entities in each direction, 
         * plus diagonal Entities.
         */
        public override void ProcessEntities(List<Entity> Entities)
        {
            foreach (Entity e in Entities)
                if ((e is DFScell))
                {
                    DFScell d = (DFScell)e;
                    if (d.State == DFScell.Untouched)
                        d.UntouchedUpdate();
                    else if (d.State == DFScell.Touched)
                        d.TouchedUpdate();
                }
        }

        /* Takes the Location list for Direct Locations, and splits it into 2 lists.
         * So long as there are any Untouched cells nearby, we should go there first.
         * If there aren't, that means we've hit a dead end and need to turn around,
         * which means choosing the single Touched cell we just came in on. As we
         * leave, DFScell.Update() will change these Touched cells to Path cells.
         * A null list is ONLY returned when the maze is finished.
         */
        public override List<Location> GetMoveLocations()
        {
            List<Location> untouchedLocs = new List<Location>();
            List<Location> touchedLocs = new List<Location>();
            foreach (Entity e in Location.toEntities(Grid, Location.GetOrthogonalLocations(Grid)))
                if ((e is DFScell))
                    if ((((DFScell)(e)).State == DFScell.Untouched))
                        untouchedLocs.Add(e.Location);
                    else if ((((DFScell)(e)).State == DFScell.Touched))
                        touchedLocs.Add(e.Location);

            if ((untouchedLocs.Count() != 0))
                return untouchedLocs;
            else if ((touchedLocs.Count() != 0))
                return touchedLocs;
            else
                return null;

        }

        //ChooseMoveLocations is inherited from Actor

        /* Moves to a new Location, leaving a Touched cell in the old Location.
         * If l == null, then the maze is finished.
         */
        public override bool MakeMove(Location l)
        {
            if ((l == null))
            {
                this.Finish();
                return true;
            }

            Location storedLoc = this.Location;// save current location
            Grid.Get(l).RemoveSelfFromGrid();// remove rock in new location
            this.MoveTo(l);// move to new location
            Grid.Set(new DFScell(Grid, storedLoc, 1, this), storedLoc);// place new rock in old location

            return true;
        }

        //Fills the Grid with Untouched Cells
        private void PopulateGrid()
        {
            for (int i = 0; (i < Grid.MaxX); i++)
                for (int j = 0; (j < Grid.MaxY); j++)
                {
                    Location l = new Location(i, j);
                    if ((Grid.Get(l) != this))
                        Grid.Set(new DFScell(Grid, l, this), l);
                }
        }

        

        //Seaches entire Grid, first changing trapped Untouched cells to Rocks,
        //Then removes all remaining cells that are not Rocks.
        private void RemoveJunkFromGrid()
        {
            for (int i = 0; i < Grid.MaxX; i++)
                for (int j = 0; j < Grid.MaxY; j++)
                {
                    Entity e = Grid.Get(new Location(i, j));
                    if ((e is DFScell))
                    {
                        this.RemoveUntouchedCellsInsideRocks(e);
                        if (((DFScell)(e)).State != DFScell.Rock || ((DFScell)(e)).Salted)
                            e.RemoveSelfFromGrid();
                    }
                }
        }

        private void RemoveUntouchedCellsInsideRocks(Entity e)
        {
            bool flag = true;
            if ((((DFScell)(e)).State == DFScell.Untouched))
            {
                foreach (Entity f in Location.toEntities(e.Grid, e.Location.GetOrthogonalLocations(Grid)))
                    if (f is DFScell)
                        if (((DFScell)(f)).State != DFScell.Rock && ((DFScell)(f)).State != DFScell.Untouched)
                            flag = false;

                if (flag)
                    ((DFScell)(e)).SetState(DFScell.Rock);

            }
        }
    }
}
    
