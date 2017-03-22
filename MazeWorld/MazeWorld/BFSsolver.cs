using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazeWorld
{
    /* This Actor will find the shortest path from its inital placement to the given target.
     * It can do this in any grid, with any kind of obstacles.
     * It cannot handle if there is no path to the target, and will remove itself if this occurs.
     * 
     * TODO: This does not behave like a normal Actor. Either the idea of a "typical" Actor is silly,
     * or a new Controller class should be made, which is much similar to how this Actor really works.
     * As it stands, this class is largely poorly organized, and should be refactored to follow
     * whatever the new formula is, whether being more like an Actor, or actually being a Controller.
     * 
     * TODO: Make this able to handle grids with no valid path to target.
     */
    public class BFSsolver : Actor
    {
        public Location Target { get; set; }
        public int Phase { get; set; }
        public int Steps { get; set; }
        private List<BFSminion> CurrentMinions;
        private List<BFSminion> RecruitMinions;
        private List<BFScell> Cells;

        public BFSsolver(Grid grid, Location location, Location target) : base(Color.Gray, grid, location)
        {
            this.Target = target;
            this.Phase = 1;
            this.Steps = 0;
            this.CurrentMinions = new List<BFSminion>();
            this.RecruitMinions = new List<BFSminion>();
            this.Cells = new List<BFScell>();
        }

        public BFSsolver() : this(null, null, null) { }

        public override void Act()
        {
            switch (this.Phase)
            {
                case 1:
                    this.InitialStepWithNoMinions();
                    break;
                case 2:
                    this.StepWithMinions();
                    break;
                case 3:
                    this.ShowShortestPath();
                    break;
            }
        }

        private void InitialStepWithNoMinions()
        {
            foreach (Location l in location.GetDirectLocations(grid))
            {
                if ((grid.Get(l) == null))
                {
                    BFSminion m = new BFSminion(grid, l);
                    grid.Set(m, l);
                    this.CurrentMinions.Add(m);
                }

            }

            this.Phase++;
            this.Steps++;
        }

        /* At each step, all CurrentMinions spawn new Minions in all the open spots adjacent to them.
         * These new RecruitMinions, then replace the CurrentMinions, and CurrentMinions become Cells
         * All cells also update their color pattern to reflect distance traveled.
         */
        private void StepWithMinions()
        {
            foreach (BFSminion m in this.CurrentMinions)
            {
                foreach (Location l in m.masterAct())
                    this.addMinion(l);

                Location minionLoc = m.location;
                BFScell bCell = new BFScell(grid, minionLoc, this);
                Cells.Add(bCell);
            }

            foreach (BFScell b in Cells)
                b.UpdateColors();

            this.killMinions();
            this.Steps++;

            if (this.CurrentMinions.Count() == 0)
            {
                if(!(grid.Get(Target) is BFSminion || grid.Get(Target) is BFScell))
                {
                    grid.Reset();
                    this.MoveToSideline();
                    return;
                }
                this.Phase++;
                grid.Remove(location);
                MoveTo(Target);
            }
        }

        private void ShowShortestPath()
        {
            int min = Int32.MaxValue;
            Location next = null;
            Location stored = location;
            foreach (Entity e in Location.toEntities(grid, location.GetDirectLocations(grid)))
            {
                if ((e is BFScell))
                    if ((((BFScell)(e)).Steps < min))
                    {
                        min = ((BFScell)(e)).Steps;
                        next = e.location;
                    }
                if (e == null)
                    next = null;
            }

            if (next != null)
            {
                MoveTo(next);
                grid.Set(new BFScell(grid, stored, this), stored);
            }
            else
            {
                Location finalLoc = location.getEmptyLocations(grid, location.GetAllLocations(grid)).ElementAt(0);
                grid.Set(new BFScell(grid, location, this), location);
                grid.Set(new BFScell(grid, finalLoc, this), finalLoc);
                this.Finish();
            }
        }

        private void addMinion(Location l)
        {
            BFSminion m = new BFSminion(grid, l);
            grid.Set(m, l);
            this.RecruitMinions.Add(m);
        }

        private void killMinions()
        {
            this.CurrentMinions.Clear();
            this.CurrentMinions.AddRange(this.RecruitMinions);
            this.RecruitMinions.Clear();
        }

        public void Finish()
        {
            CurrentMinions.Clear();
            RecruitMinions.Clear();
            Cells.Clear();
            Phase = 1;
            Steps = 0;
            grid.FinishedWithStep = true;
            Location stored = location;
            this.MoveToSideline();
            grid.Set(new BFScell(grid, stored, this), stored);
        }

        public override String ToString()
        {
            String str = base.ToString() + " Phase: " + Phase + " Steps: " + Steps + " Total Cells: " + Cells.Count();
            return str;
        }
    }
}
