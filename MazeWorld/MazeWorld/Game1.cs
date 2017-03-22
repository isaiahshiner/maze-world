using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;

namespace MazeWorld
{
    /* This is a pregenerated MonoGame class used to load content and actually run
     * the game, with controls and drawing and such. At the moment, it is
     * nothing but total spaghetti code, and should NOT be taken as final. This is
     * good enough for initial viewing, but basically everything here needs to be
     * redone and moved to other classes. As such, it will be sparsely documented.
     * Ask me on Discord directly if something is confusing, as I'm sure a lot of this
     * will be.
     */
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D tex2;
        Texture2D tex4;
        Texture2D tex8;
        Texture2D tex16;
        Texture2D tex32;
        Texture2D tex64;
        Texture2D texSelected;
        Color selectedColor = Color.Red;
        SpriteFont font;
        Grid grid;
        String headerObjectText;
        int X;
        int Y;
        int speed = 0;
        int haltedFrames = 0;
        int phase = 0;
        int clickAction = 0;
        bool looping = true;
        Rectangle playArea;
        Rectangle header;
        KeyboardState kPrevious;
        MouseState mPrevious;
        Location lPrevious;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            X = graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            Y = graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            graphics.IsFullScreen = true;
            this.IsFixedTimeStep = false;
            graphics.ApplyChanges();
        }

        protected override void Initialize()
        {
            InstantiateGrid(64);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            tex2 = this.Content.Load<Texture2D>("WhiteSquare2");
            tex4 = this.Content.Load<Texture2D>("WhiteSquare4");
            tex8 = this.Content.Load<Texture2D>("WhiteSquare8");
            tex16 = this.Content.Load<Texture2D>("WhiteSquare16");
            tex32 = this.Content.Load<Texture2D>("WhiteSquare32");
            tex64 = this.Content.Load<Texture2D>("WhiteSquare64");
            texSelected = ChooseTexture(64);
            font = Content.Load<SpriteFont>("Arial");
        }

        protected override void UnloadContent()
        {
            Content.Unload();
        }

        /* This method is where most of the spaghetti is. It controls ALL of the user controls,
         * and how to change the Grid based on those controls without anything breaking.
         */
        protected override void Update(GameTime gameTime)
        {
            if (IsActive)
            {
                KeyboardState kCurrent = Keyboard.GetState();
                MouseState mCurrent = Mouse.GetState();

                //Makes the Grid Scramble and Solve itself over and over.
                if(looping && grid.FinishedWithStep)
                {
                    switch(phase)
                    {
                        case 0:
                            grid.FinishedWithStep = false;
                            grid.Scramble();
                            phase++;
                            break;
                        case 1:
                            grid.FinishedWithStep = false;
                            grid.Solve();
                            phase++;
                            if (speed > 5)
                                speed = 5;
                            break;
                        case 2:
                            grid.FinishedWithStep = false;
                            grid.Reset();
                            grid.Scramble();
                            phase = 1;
                            break;
                    }
                }

                //Basic controls.
                if (Keyboard.GetState().IsKeyDown(Keys.Escape))//Any frame escape is pressed
                    Exit();

                if (kCurrent.IsKeyDown(Keys.Right) && !kPrevious.IsKeyDown(Keys.Right))//The first frame right is pressed
                    grid.Step();

                if (kCurrent.IsKeyDown(Keys.Up) && !kPrevious.IsKeyDown(Keys.Up))//The first frame up is pressed
                    speed++;

                if (kCurrent.IsKeyDown(Keys.Down) && !kPrevious.IsKeyDown(Keys.Down))//The first frame down is pressed
                    speed--;

                if (kCurrent.IsKeyDown(Keys.Space) && !kPrevious.IsKeyDown(Keys.Space))//The first frame that space is pressed
                {
                    grid.Step();
                    CalcSpeed();
                }
                else if (kCurrent.IsKeyDown(Keys.Space) && kPrevious.IsKeyDown(Keys.Space))//The next several frames that space is pressed
                    grid.Step(CalcSpeed());

                else if (!kCurrent.IsKeyDown(Keys.Space) && kPrevious.IsKeyDown(Keys.Space))//The first frame space is no longer being pressed
                    haltedFrames = 0;

                //Turns Salting on or off
                if (kCurrent.IsKeyDown(Keys.S) && !kPrevious.IsKeyDown(Keys.S))
                    grid.Salting = !grid.Salting;

                //Resets the Grid, and resets the Gener or Solver if they were running.
                //Also disables looping
                if (kCurrent.IsKeyDown(Keys.R) && !kPrevious.IsKeyDown(Keys.R))
                {
                    if (phase != 0)
                    {
                        if (phase == 1 && grid.MazeGener is DFSgener)
                            ((DFSgener)(grid.MazeGener)).Finish();
                        else if (phase == 2 && grid.MazeSolver is BFSsolver)
                            ((BFSsolver)(grid.MazeSolver)).Finish();
                    }
                    phase = 0;
                    looping = false;
                    grid.Reset();
                }
                
                //Scrambles the grid, resets the Solver if it was running, and disables looping    
                if (kCurrent.IsKeyDown(Keys.T) && !kPrevious.IsKeyDown(Keys.T) && phase != 1)
                {
                    if (phase == 2 && grid.MazeSolver is BFSsolver)
                        ((BFSsolver)(grid.MazeSolver)).Finish();
                    looping = false;
                    phase = 1;
                    grid.Reset();
                    grid.Scramble();
                }

                //Solves the grid, resets the Gener if it was running, and disables looping
                if (kCurrent.IsKeyDown(Keys.Y) && !kPrevious.IsKeyDown(Keys.Y) && phase != 2)
                {
                    if(phase == 1 && grid.MazeGener is DFSgener)
                        ((DFSgener)(grid.MazeGener)).Finish();
                    looping = false;
                    phase = 2;
                    grid.Solve();
                }

                //Resets the grid, resets the Solver and if they were running, and reenables looping 
                if (kCurrent.IsKeyDown(Keys.L) && !kPrevious.IsKeyDown(Keys.L))
                {
                    if (phase != 0)
                    {
                        if (phase == 1 && grid.MazeGener is DFSgener)
                            ((DFSgener)(grid.MazeGener)).Finish();
                        else if (phase == 2 && grid.MazeSolver is BFSsolver)
                            ((BFSsolver)(grid.MazeSolver)).Finish();
                    }
                    grid.FinishedWithStep = true;
                    looping = true;
                    phase = 0;
                    grid.Reset();
                }

                //Changes the texture size by reinstatiating the Grid and recalculating everything
                if (kCurrent.IsKeyDown(Keys.D1) && !kPrevious.IsKeyDown(Keys.D1))
                    ChangeTexture(64);
                if (kCurrent.IsKeyDown(Keys.D2) && !kPrevious.IsKeyDown(Keys.D2))
                    ChangeTexture(32);
                if (kCurrent.IsKeyDown(Keys.D3) && !kPrevious.IsKeyDown(Keys.D3))
                    ChangeTexture(16);
                if (kCurrent.IsKeyDown(Keys.D4) && !kPrevious.IsKeyDown(Keys.D4))
                    ChangeTexture(8);
                if (kCurrent.IsKeyDown(Keys.D5) && !kPrevious.IsKeyDown(Keys.D5))
                    ChangeTexture(4);
                if (kCurrent.IsKeyDown(Keys.D6) && !kPrevious.IsKeyDown(Keys.D6))
                    ChangeTexture(2);

                //Changes the color of the Entity to be spawned. All 6 color wheel colors, plus black. White is an eraser.
                if (kCurrent.IsKeyDown(Keys.NumPad1) && !kPrevious.IsKeyDown(Keys.NumPad1))
                    selectedColor = Color.Red;
                if (kCurrent.IsKeyDown(Keys.NumPad2) && !kPrevious.IsKeyDown(Keys.NumPad2))
                    selectedColor = Color.Orange;
                if (kCurrent.IsKeyDown(Keys.NumPad3) && !kPrevious.IsKeyDown(Keys.NumPad3))
                    selectedColor = Color.Yellow;
                if (kCurrent.IsKeyDown(Keys.NumPad4) && !kPrevious.IsKeyDown(Keys.NumPad4))
                    selectedColor = Color.Green;
                if (kCurrent.IsKeyDown(Keys.NumPad5) && !kPrevious.IsKeyDown(Keys.NumPad5))
                    selectedColor = Color.Blue;
                if (kCurrent.IsKeyDown(Keys.NumPad6) && !kPrevious.IsKeyDown(Keys.NumPad6))
                    selectedColor = Color.Purple;
                if (kCurrent.IsKeyDown(Keys.NumPad7) && !kPrevious.IsKeyDown(Keys.NumPad7))
                    selectedColor = Color.Black;
                if (kCurrent.IsKeyDown(Keys.NumPad8) && !kPrevious.IsKeyDown(Keys.NumPad8))
                    selectedColor = Color.White;

                //Changes the current action to be done on click.
                if (kCurrent.IsKeyDown(Keys.Z) && !kPrevious.IsKeyDown(Keys.Z))
                    clickAction = 0;
                if (kCurrent.IsKeyDown(Keys.X) && !kPrevious.IsKeyDown(Keys.X))
                    clickAction = 1;
                if (kCurrent.IsKeyDown(Keys.C) && !kPrevious.IsKeyDown(Keys.C))
                    clickAction = 2;

                //Performs the click action, as long as the cursor is actually on the Grid.
                if (mCurrent.LeftButton == ButtonState.Pressed && (playArea.Contains(mCurrent.X, mCurrent.Y)))
                {
                    Location lCurrent = CoordsToLocations(new Vector2(mCurrent.X, mCurrent.Y));
                    if (grid.IsValid(lCurrent) && !lCurrent.Equals(lPrevious))//Only performs the action once per click or per Grid Square.
                    {
                        lPrevious = lCurrent;
                        if (selectedColor.Equals(Color.White) && clickAction != 0)//If the color is white, just remove whatever was there and nothing else/
                        {
                            grid.Remove(lCurrent);
                        }
                        else switch (clickAction)
                            {
                                case 0:
                                    if (grid.Get(lCurrent) == null)
                                        headerObjectText = "Location: " + lCurrent.ToString() + " Entity: null";
                                    else
                                        headerObjectText = "Location: " + lCurrent.ToString() + " Entity: " + grid.Get(lCurrent).ToString();
                                    break;
                                case 1:
                                    grid.Set(new Actor(selectedColor, grid, lCurrent), lCurrent);
                                    break;
                                case 2:
                                    grid.Set(new Rock(selectedColor, grid, lCurrent), lCurrent);
                                    break;
                                default:
                                    throw new NotImplementedException("Default of ClickAction switch was called.");
                            }
                    }
                }

                //Removes previous location after you stop clicking
                if(mCurrent.LeftButton == ButtonState.Released && mPrevious.LeftButton == ButtonState.Pressed)
                    lPrevious = null;

                kPrevious = kCurrent;
                mPrevious = mCurrent;
                base.Update(gameTime);
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            spriteBatch.Begin();
            spriteBatch.Draw(tex16, new Rectangle(0, 0, X, Y), Color.Gray);
            DrawHeader(spriteBatch, gameTime);
            DrawGrid(spriteBatch, texSelected, new Vector2(playArea.X, playArea.Y));
            spriteBatch.End();
            base.Draw(gameTime);
        }

        private void DrawGrid(SpriteBatch s, Texture2D t, Vector2 v)
        {
            for (int i = 0; i < grid.MaxX; i++)
                for (int j = 0; j < grid.MaxY; j++)
                {
                    if (grid.Get(i, j) != null)
                    {
                        Color c = grid.Get(i, j).color;
                        Vector2 p = new Vector2(v.X + (t.Width * i), v.Y + (t.Height * j));

                        s.Draw(texture: t, color: c, position: p);
                    }
                    else
                    {
                        Color c = Color.White;
                        Vector2 p = new Vector2(v.X + (t.Width * i), v.Y + (t.Height * j));

                        s.Draw(texture: t, color: c, position: p);
                    }
                }
        }

        private void DrawHeader(SpriteBatch s, GameTime g)
        {
            s.Draw(tex16, header, Color.Black);
            float fps = (float)(1 / g.ElapsedGameTime.TotalSeconds);
            s.DrawString(font, ("FPS: " + fps.ToString("0.0")), new Vector2(9, 8), Color.LimeGreen);
            s.DrawString(font, "Speed: " + speed, new Vector2(85, 8), Color.LimeGreen);
            s.DrawString(font, "Object: " + headerObjectText, new Vector2(160, 8), Color.LimeGreen);
        }

        private Location CoordsToLocations(Vector2 v)
        {
            v.X = v.X - playArea.Left;
            v.Y = v.Y - playArea.Top;
            return new Location((int)Math.Floor((double)(v.X) / texSelected.Width), (int)Math.Floor((double)(v.Y)/ texSelected.Width));
        }

        private void CalculateRectangles()
        {
            int rightAdd = X % 16;
            int bottomAdd = Y % 16;

            if (rightAdd == 0)
                rightAdd = 8;
            else if (rightAdd + 8 > 16)
                rightAdd -= 8;

            if (bottomAdd == 0)
                bottomAdd = 8;
            else if (bottomAdd + 8 > 16)
                bottomAdd -= 8;

            header = new Rectangle(8, 7, X - (8 + rightAdd), 18);
            playArea = new Rectangle(8, 32, X - (8 + rightAdd), Y - (32 + 8 + bottomAdd));
        }

        private int CalcSpeed()
        {
            if (haltedFrames > 0)
            {
                haltedFrames--;
                return 0;
            }
            else
            {
                if (speed < 0)
                {
                    haltedFrames = (int)Math.Pow(2, Math.Abs(speed));
                    return 1;
                }
                else
                    return (int)Math.Pow(2, speed);
            }

        }

        private void InstantiateGrid()
        {
            CalculateRectangles();
            grid = new Grid(playArea.Width / texSelected.Width, playArea.Height / texSelected.Width, new DFSgener(), new BFSsolver());
        }

        private void InstantiateGrid(int size)
        {
            CalculateRectangles();
            grid = new Grid(playArea.Width / size, playArea.Height / size, new DFSgener(), new BFSsolver());
        }

        private void ChangeTexture(int t)
        {
            looping = true;
            phase = 0;
            texSelected = ChooseTexture(t);
            InstantiateGrid();
        }

        private Texture2D ChooseTexture(int t)
        {
            switch(t)
            {
                case 2:
                    return tex2;
                case 4:
                    return tex4;
                case 8:
                    return tex8;
                case 16:
                    return tex16;
                case 32:
                    return tex32;
                case 64:
                    return tex64;
                default:
                    return tex64;
            }
        }
    }
}
