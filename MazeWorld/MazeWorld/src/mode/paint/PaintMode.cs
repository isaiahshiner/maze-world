using MazeWorld.src.game;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazeWorld.src.mode.paint
{
    public class PaintMode : GameMode
    {

        //Color and Block placement
        public Color SelectedColor { get; set; } = Color.Red;
        public int SelectedBlock { get; set; } = 0;

        public override void Update()
        {
            //Changes the Color to be used when drawing.
            if (con.KeyFalling(Keys.NumPad0))
                draw.SelectedColor = Color.White;//The Eraser
            else if (con.KeyFalling(Keys.NumPad1))
                draw.SelectedColor = Color.Red;
            else if (con.KeyFalling(Keys.NumPad2))
                draw.SelectedColor = Color.Orange;
            else if (con.KeyFalling(Keys.NumPad3))
                draw.SelectedColor = Color.Yellow;
            else if (con.KeyFalling(Keys.NumPad4))
                draw.SelectedColor = Color.Green;
            else if (con.KeyFalling(Keys.NumPad5))
                draw.SelectedColor = Color.Blue;
            else if (con.KeyFalling(Keys.NumPad6))
                draw.SelectedColor = Color.Purple;
            else if (con.KeyFalling(Keys.NumPad7))
                draw.SelectedColor = Color.Black;

            //Changes the block to be placed on Mouse Click
            if (con.KeyFalling(Keys.X))
                draw.SelectedBlock = 0;//Rock
            else if (con.KeyFalling(Keys.C))
                draw.SelectedBlock = 1;//Actor
        }

        public void PlaceBlock(Location l)
        {
            /* This solves a bug, but it is not very elegant.
             * I also want to see if someone finds the bug,
             * and whether they do or don't find other bugs along the way.
             if (grid.Get(l) == grid.MazeGener || grid.Get(l) == grid.MazeSolver)
             {
                 this.Reset();
                 return;
              }
              */

            if (SelectedColor == Color.White)
            {
                grid.Remove(l);
            }
            switch (SelectedBlock)
            {
                case 0:
                    grid.Set(new Rock(SelectedColor, grid, l), l);
                    break;
                case 1:
                    grid.Set(new Actor(SelectedColor, grid, l), l);
                    break;
                default:
                    throw new NotImplementedException("Default of PlaceBlock was called!");
            }
        }


    }
}
