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
    /* The pregenerated MonoGame Game class with just enough code to run the Maze.
     * Relies heavily on Helper classes.
     */
    public class StandardMazeGame : Game
    {
        private DrawHelper draw;
        private ControlsHelper con;

        public StandardMazeGame()
        {
            Content.RootDirectory = "Content";
            draw = new DrawHelper(this);
            con = new ControlsHelper(draw);
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            Texture2D t = this.Content.Load<Texture2D>("WhiteSquare16");
            SpriteFont f = this.Content.Load<SpriteFont>("Arial");
            draw.SetTextures(t, f);
            base.LoadContent();
        }

        protected override void BeginRun()
        {
            con.Update();//Updates on the frame right before the game starts
        }

        protected override void Update(GameTime gameTime)
        {
            if (IsActive)
            {
                //Checks to see if the Maze should move to the next step
                draw.LoopingCheck();

                //These controls set basic movement and maze changes
                //Step based on speed, manually reset/scramble/solve the grid, or 
                //go back to looping.
                //TODO: add auto move functionality.
                if (con.KeyFalling(Keys.Escape) || con.KeyFalling(Keys.Delete))
                    this.Exit();

                else if (con.KeyFalling(Keys.Right))
                    draw.grid.Step();

                else if (con.KeyFalling(Keys.Space))
                {
                    draw.grid.Step();
                    draw.CalcSpeed();
                }
                else if (con.KeyDown(Keys.Space))
                    draw.grid.Step(draw.CalcSpeed());

                else if (con.KeyRising(Keys.Space))
                    draw.HaltedFrames = 0;

                else if (con.KeyFalling(Keys.R))
                    draw.Reset();

                else if (con.KeyFalling(Keys.T))
                    draw.Scramble();

                else if (con.KeyFalling(Keys.Y))
                    draw.Solve();

                else if (con.KeyFalling(Keys.L))
                    draw.RestartLooping();

                else if (con.KeyFalling(Keys.A))
                    draw.Auto = !draw.Auto;

                //If nothing else was pressed, then do the Auto check
                else if (draw.Auto)
                    draw.grid.Step(draw.CalcSpeed());

                //Changes speed
                if (con.KeyFalling(Keys.Up))
                    draw.Speed++;
                else if (con.KeyFalling(Keys.Down))
                    draw.Speed--;

                //Turns Salting on and off.
                //Salting will randomly mark some generated rocks to be removed.
                if (con.KeyFalling(Keys.S))
                    draw.grid.Salting = true;

                //Flips fullscreen
                if (con.KeyFalling(Keys.F))
                {
                    if (draw.Graphics.IsFullScreen)
                        draw.DisableFullScreen();
                    else
                        draw.EnableFullScreen();
                }

                //Changes the texture size and reinstantiates the grid
                if (con.KeyFalling(Keys.D1))
                    draw.ChangeTextureSize(64);
                else if (con.KeyFalling(Keys.D2))
                    draw.ChangeTextureSize(32);
                else if (con.KeyFalling(Keys.D3))
                    draw.ChangeTextureSize(16);
                else if (con.KeyFalling(Keys.D4))
                    draw.ChangeTextureSize(8);
                else if (con.KeyFalling(Keys.D5))
                    draw.ChangeTextureSize(4);
                else if (con.KeyFalling(Keys.D6))
                    draw.ChangeTextureSize(2);

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

                //Performs all of the mouse actions, but only if the mouse is on the Grid.
                if (con.MouseIsOnGrid())
                {
                    Location LCurrent = con.LocationOfMouse();
                    if (draw.grid.IsValid(LCurrent))
                    {
                        //Always display what the mouse is hovering over.
                        draw.SetHeaderText(LCurrent);

                        //If mouse is falling, or down and moving, place a block
                        if (con.MouseFalling())
                            draw.PlaceBlock(LCurrent);
                        else if (con.MouseDown() && con.IsLocationChanging())
                            draw.PlaceBlock(LCurrent);
                    }
                }
            }
            else if (!IsActive && draw.Graphics.IsFullScreen)
                draw.DisableFullScreen();

            con.Update();
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Gray);

            draw.Batch.Begin();
            draw.DrawHeader(gameTime);
            draw.DrawGrid();
            draw.Batch.End();

            base.Draw(gameTime);
        }
    }
}
