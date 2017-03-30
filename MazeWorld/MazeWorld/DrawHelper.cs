using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazeWorld
{
    /* The current main Helper class.
     * Contains all of the variables needed to run the game,
     * as well as most of the helper methods.
     * Since the methods are generally very simple and are not conditional, 
     * few will be documented.
     */
    public class DrawHelper
    {
        //General
        public Game game { get; set; }
        public GraphicsDeviceManager Graphics { get; set; }
        public int TexSize { get; set; } = 64;
        public Grid grid { get; set; }
        public ControlsHelper con { get; set; }

        //Drawing
        public SpriteBatch Batch { get; set; }
        public SpriteFont Font { get; set; }
        public Texture2D BaseTex { get; set; }
        public Rectangle PlayArea { get; set; }
        public Rectangle Header { get; set; }
        public String HeaderObjectText { get; set; }
        public int X { get; set; }
        public int Y { get; set; }

        //Looping
        public int Speed { get; set; } = 0;
        public int HaltedFrames { get; set; } = 0;
        public int Phase { get; set; } = 0;
        public bool Looping { get; set; } = true;
        public bool Auto { get; set; } = false;

        //Color and Block placement
        public Color SelectedColor { get; set; } = Color.Red;
        public int SelectedBlock { get; set; } = 0;

        public DrawHelper(Game g)
        {
            game = g;
            Graphics = new GraphicsDeviceManager(game);
            game.IsFixedTimeStep = false;
            game.IsMouseVisible = true;

            EnableFullScreen();
        }

        public void SetTextures( Texture2D baseTex, SpriteFont font)
        {
            Font = font;
            BaseTex = baseTex;

            this.InstantiateGrid();
            Batch = new SpriteBatch(game.GraphicsDevice);
        }

        public void DrawGrid()
        {
            for (int i = 0; i < grid.MaxX; i++)
                for (int j = 0; j < grid.MaxY; j++)
                {
                    Color c = Color.White;
                    if (grid.Get(i, j) != null)
                        c = grid.Get(i, j).color;

                    Point loc = new Point(PlayArea.Location.X + (TexSize * i), PlayArea.Location.Y + (TexSize * j));
                    Point size = new Point(TexSize);
                    Rectangle r = new Rectangle(loc, size);

                    Batch.Draw(texture: BaseTex, color: c, destinationRectangle: r);
                }
        }

        public void DrawHeader(GameTime gt)
        {
            Batch.Draw(BaseTex, Header, Color.Black);
            float fps = (float)(1 / gt.ElapsedGameTime.TotalSeconds);
            Batch.DrawString(Font, ("FPS: " + fps.ToString("0.0")), new Vector2(9, 8), Color.LimeGreen);
            Batch.DrawString(Font, "Speed: " + Speed, new Vector2(85, 8), Color.LimeGreen);
            Batch.DrawString(Font, HeaderObjectText, new Vector2(160, 8), Color.LimeGreen);
        }

        public int CalcSpeed()
        {
            if (HaltedFrames > 0)
            {
                HaltedFrames--;
                return 0;
            }
            else
            {
                if (Speed < 0)
                {
                    HaltedFrames = (int)Math.Pow(2, Math.Abs(Speed));
                    return 1;
                }
                else
                    return (int)Math.Pow(2, Speed);
            }
        }

        public void LoopingCheck()
        {
            if (Looping && grid.FinishedWithStep)
            {
                switch (Phase)
                {
                    case 0:
                        grid.FinishedWithStep = false;
                        grid.Scramble();
                        Phase++;
                        break;
                    case 1:
                        grid.FinishedWithStep = false;
                        grid.Solve();
                        Phase++;
                        if (Speed > 5)
                            Speed = 5;
                        break;
                    case 2:
                        grid.FinishedWithStep = false;
                        grid.Reset();
                        grid.Scramble();
                        Phase = 1;
                        break;
                }
            }
        }

        public void RestartLooping()
        {
            if (Phase != 0)
            {
                if (Phase == 1 && grid.MazeGener is DFSgener)
                    ((DFSgener)(grid.MazeGener)).Finish();
                else if (Phase == 2 && grid.MazeSolver is BFSsolver)
                    ((BFSsolver)(grid.MazeSolver)).Finish();
            }
            grid.FinishedWithStep = true;
            Looping = true;
            Auto = false;
            Phase = 0;
            grid.Reset();
        }

        public void Reset()
        {
            if (Phase != 0)
            {
                if (Phase == 1 && grid.MazeGener is DFSgener)
                    ((DFSgener)(grid.MazeGener)).Finish();
                else if (Phase == 2 && grid.MazeSolver is BFSsolver)
                    ((BFSsolver)(grid.MazeSolver)).Finish();
            }
            Phase = 0;
            Looping = false;
            Auto = false;
            grid.Reset();
        }

        public void Scramble()
        {
            if (Phase != 1)
            {
                if (Phase == 2 && grid.MazeSolver is BFSsolver)
                    ((BFSsolver)(grid.MazeSolver)).Finish();
                Phase = 1;
                Looping = false;
                Auto = false;
                grid.Reset();
                grid.Scramble();
            }
        }

        public void Solve()
        {
            if (Phase != 2)
            {
                if (Phase == 1 && grid.MazeGener is DFSgener)
                    ((DFSgener)(grid.MazeGener)).Finish();
                Phase = 2;
                Looping = false;
                Auto = false;
                grid.Solve();
            }
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

        public void SetHeaderText(Location l)
        {
            if (grid.Get(l) == null)
                HeaderObjectText = "Location: " + l.ToString() + " Object: " + "null";
            else
                HeaderObjectText = "Location: " + l.ToString() + " Object: " + grid.Get(l).ToString();
        }

        public void ChangeTextureSize(int i)
        {
            TexSize = i;
            InstantiateGrid();
            Phase = 0;
            Looping = true;
        }

        public void EnableFullScreen()
        {
            X = Graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            Y = Graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            Graphics.IsFullScreen = true;
            InstantiateGrid();
            Phase = 0;
            Looping = true;
            Graphics.ApplyChanges();
        }
        public void DisableFullScreen()
        {
            X = Graphics.PreferredBackBufferWidth =  (int)(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width  * (double).75);
            Y = Graphics.PreferredBackBufferHeight = (int)(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height * (double).75);
            Graphics.IsFullScreen = false;
            InstantiateGrid();
            Phase = 0;
            Looping = true;
            Graphics.ApplyChanges();
        }

        public void CalculateRectangles()
        {
            int rightAdd = X % 16;
            int bottomAdd = Y % 16;

            if (rightAdd + 8 >= 16)
                rightAdd -= 8;

            if (bottomAdd + 8 >= 16)
                bottomAdd -= 8;

            Header = new Rectangle(8, 7, X - (16 + rightAdd), 18);
            PlayArea = new Rectangle(8, 32, X - (16 + rightAdd), Y - (40 + rightAdd));
        }

        private void InstantiateGrid()
        {
            this.CalculateRectangles();
            grid = new Grid(PlayArea.Width / TexSize, PlayArea.Height / TexSize, new DFSgener(), new BFSsolver());
        }
    }
}
