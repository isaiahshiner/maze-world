using System.Collections.Generic;

namespace MazeWorld
{
    public class BFSminion : Rock
    {

        public BFSminion(Grid g, Location l)
        {
            Color = Color.Red;
            PutSelfInGrid(g, l);
        }

        public List<Location> masterAct()
        {
            List<Location> locs = new List<Location>();
            foreach (Location l in Location.GetOrthogonalLocations(Grid))
                if ((Grid.Get(l) == null))
                    locs.Add(l);

            return locs;
        }
    }
}
