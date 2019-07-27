using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazeWorld
{
    public class BFScell : Rock
    {

        public int Steps { get; set; }

        public BFSsolver Master { get; }

        public BFScell(Grid g, Location l, BFSsolver m)
        {
            PutSelfInGrid(g, l);
            this.Steps = m.Steps;
            this.Master = m;
            this.Color = Color.Magenta;
        }

        public void UpdateColors()
        {
            //Will only update colors every 10 steps.
               if (this.Steps == Master.Steps || ((Master.Steps + this.Steps) % Master.CellUpdateRate == 0))
                this.Color = DynamicColorCalculator(this.Steps, Master.Steps);
            
        }

        /* I have to confess, I did not write this code, I don't know where I originally found it,
         * and I can't find it again to link to it. I also do not really know how it works, so I
         * don't know how efficient it is. This may be the only way to do what I'm looking for,
         * but I'm not sure.
         * 
         * For now, it works fine and only causes lag on very large Grids at high speeds,
         * So it is NOT a priority to change.
         */
        private static Color DynamicColorCalculator(int d, int t)
        {
            int wavelength = (int)((205 * ((double)d / t)) + 440);
            double red, green, blue, factor;

            const int INTENSITY_MAX = 255;
            const double GAMMA = 1.00;

            if (wavelength >= 350 && wavelength <= 439)
            {
                red = -(wavelength - 440d) / (440d - 350d);
                green = 0.0;
                blue = 1.0;
            }
            else if (wavelength >= 440 && wavelength <= 489)
            {
                red = 0.0;
                green = (wavelength - 440d) / (490d - 440d);
                blue = 1.0;
            }
            else if (wavelength >= 490 && wavelength <= 509)
            {
                red = 0.0;
                green = 1.0;
                blue = -(wavelength - 510d) / (510d - 490d);
            }
            else if (wavelength >= 510 && wavelength <= 579)
            {
                red = (wavelength - 510d) / (580d - 510d);
                green = 1.0;
                blue = 0.0;
            }
            else if (wavelength >= 580 && wavelength <= 644)
            {
                red = 1.0;
                green = -(wavelength - 645d) / (645d - 580d);
                blue = 0.0;
            }
            else if (wavelength >= 645 && wavelength <= 780)
            {
                red = 1.0;
                green = 0.0;
                blue = 0.0;
            }
            else
            {
                red = 0.0;
                green = 0.0;
                blue = 0.0;
            }

            if (wavelength >= 350 && wavelength <= 419)
            {
                factor = 0.3 + 0.7 * (wavelength - 350d) / (420d - 350d);
            }
            else if (wavelength >= 420 && wavelength <= 700)
            {
                factor = 1.0;
            }
            else if (wavelength >= 701 && wavelength <= 780)
            {
                factor = 0.3 + 0.7 * (780d - wavelength) / (780d - 700d);
            }
            else
            {
                factor = 0.0;
            }

            int redVal = (red == 0.0) ? 0 : (int)Math.Round(INTENSITY_MAX * Math.Pow(red * factor, GAMMA));
            int greenVal = (green == 0.0) ? 0 : (int)Math.Round(INTENSITY_MAX * Math.Pow(green * factor, GAMMA));
            int blueVal = (blue == 0.0) ? 0 : (int)Math.Round(INTENSITY_MAX * Math.Pow(blue * factor, GAMMA));

            return new Color(redVal, greenVal, blueVal);
        }

        public override String ToString()
        {
            String str = base.ToString() + " Steps: " + Steps;
            return str;
        }
    }
}
