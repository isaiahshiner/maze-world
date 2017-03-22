﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazeWorld
{
    /* Actors are how a Grid changes over time.
     * When grid.Step() is called, all Actors will Act().
     * They can process other Entites and make them do things.
     * They can move themselves around the Grid however they want.
     * 
     * This class, though really a framework class, is an example of how
     * Actors should look, and can be used as a test class, since it walks
     * around randomly and will change the colors of the Entities it bumps
     * into to show that it is processing them.
     */
    public class Actor : Entity
    {
        public Actor() : base() { this.color = Color.Cyan; }

        public Actor(Color c, Grid g, Location l) : base(g, l) { this.color = c; }

        /* Determines how an Actor should Act.
         * Should be kept short by using the other standard Act methods.
         * The other methods shown are examples of how an Actor should work,
         * 
         * This implementation shows the methods being used in a very basic way.
         * The methods are very safe, and rely on Grid or Location methods heavily.
         */
        public virtual void Act()
        {
            ProcessEntities(GetEntities());
            MakeMove(ChooseMoveLocation(GetMoveLocations()));
        }

        /* Determines what Entities will be processed by ProcessEntities().
         * This implementation uses Location methods to get all the Entities-
         * around in all 8 cardinal directions.
         */
        public virtual List<Entity> GetEntities()
        {
            return Location.toEntities(grid, location.getFullLocations(grid, location.GetAllLocations(grid)));
        }

        /* Determines how Entites will be changed.
         * This implementation will randomize the colors of the Entites around it.
         */
        public virtual void ProcessEntities(List<Entity> ents)
        {
            foreach (Entity e in ents)
                e.RandomizeColor();
        }

        /* Determines all the Locations that this Actor might try to move to.
         * This implementation uses Location methods to get the orthogonally adjacent Locations.
         * It also only gets Locations where there are no other Entities.
         */
        public virtual List<Location> GetMoveLocations()
        {
            return location.getEmptyLocations(grid, location.GetDirectLocations(grid));
        }

        /* Determines which Location out of the list will be chosen.
         * This simply chooses one at random. If the list is emtpy, returns null.
         */
        public virtual Location ChooseMoveLocation(List<Location> locs)
        {
            if (locs != null && locs.Count > 0)
                return locs[(int)(grid.Rand.NextDouble() * locs.Count)];
            else
                return null;
        }

        /* The system for actually making an Actor move into another Location.
         * Since there is nothing at grid.Get(loc), we can just move there,
         * as long as loc != null.
         * 
         * PRECONDITION: grid.Get(loc) == null; 
         */
        public virtual bool MakeMove(Location loc)
        {
            if (loc == null)
                return false;
            else
                MoveTo(loc);

            return true;
        }

        //Randomizes colors, with a push towards being cyan.
        public override void RandomizeColor()
        {
            int r1 = (int)(grid.Rand.NextDouble() * 32), 
                r2 = (int)(grid.Rand.NextDouble() * 128) + 128, 
                r3 = (int)(grid.Rand.NextDouble() * 128) + 128;
            this.color = new Color(r1, r2, r3);
        }
    }
}
