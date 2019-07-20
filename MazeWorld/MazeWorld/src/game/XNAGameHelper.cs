using MazeWorld.src.game;
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
    public partial class XNAGame : Game
    {
        //General
        public GraphicsDeviceManager Graphics { get; set; }
        public int TexSize { get; set; } = 64;

        //Drawing
        public SpriteBatch Batch { get; set; }
        public SpriteFont Font { get; set; }
        public Texture2D BaseTex { get; set; }
        public Rectangle PlayArea { get; set; }
        public Rectangle Header { get; set; }
        public String HeaderObjectText { get; set; }
        public int ResolutionX { get; set; }
        public int ResolutionY { get; set; }

        /// <summary>
        /// Useful control helpers, does not specify actual controls.
        /// </summary>
        public ControlsHelper Ctrl { get; set; }

        /// <summary>
        /// Determines controls and game loop functions
        /// </summary>
        public GameMode GameMode { get; set; }

        private void SetTextures( Texture2D baseTex, SpriteFont font)
        {
            Font = font;
            BaseTex = baseTex;

            Batch = new SpriteBatch(GraphicsDevice);
        }

        private void DrawGrid()
        {
            Grid g = GameMode.Grid;
            for (int i = 0; i < g.MaxX; i++)
                for (int j = 0; j < g.MaxY; j++)
                {
                    Color c = Color.White;
                    if (g.Get(i, j) != null)
                        c = g.Get(i, j).Color;

                    Point loc = new Point(PlayArea.Location.X + (TexSize * i), PlayArea.Location.Y + (TexSize * j));
                    Point size = new Point(TexSize);
                    Rectangle r = new Rectangle(loc, size);

                    Batch.Draw(texture: BaseTex, color: c, destinationRectangle: r);
                }
        }

        private void DrawHeader(GameTime gt)
        {
            Batch.Draw(BaseTex, Header, Color.Black);

            String finalText;
            float fps = (float)(1 / gt.ElapsedGameTime.TotalSeconds);

            finalText = ("FPS: " + fps.ToString("0.0")) + " " + GameMode.GetHeaderText();

            Batch.DrawString(Font, finalText, new Vector2(9, 8), Color.LimeGreen);
        }

        private void ChangeTextureSize(int i)
        {
            TexSize = i;
            ResetGrid();
        }

        private void EnableFullScreen()
        {
            ResolutionX = Graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            ResolutionY = Graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            Graphics.IsFullScreen = true;
            ResetGrid();
            Graphics.ApplyChanges();
        }

        private void DisableFullScreen()
        {
            ResolutionX = Graphics.PreferredBackBufferWidth =  (int)(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width  * (double).75);
            ResolutionY = Graphics.PreferredBackBufferHeight = (int)(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height * (double).75);
            Graphics.IsFullScreen = false;
            ResetGrid();
            Graphics.ApplyChanges();
        }

        private void CalculateRectangles()
        {
            int rightAdd = ResolutionX % 16;
            int bottomAdd = ResolutionY % 16;

            if (rightAdd + 8 >= 16)
                rightAdd -= 8;

            if (bottomAdd + 8 >= 16)
                bottomAdd -= 8;

            Header = new Rectangle(8, 7, ResolutionX - (16 + rightAdd), 18);
            PlayArea = new Rectangle(8, 32, ResolutionX - (16 + rightAdd), ResolutionY - (40 + rightAdd));
        }

        private void ResetGrid()
        {
            this.CalculateRectangles();
            GameMode.Reset(GetGridSize());
        }

        private (int, int) GetGridSize()
        {
            return (PlayArea.Width / TexSize, PlayArea.Height / TexSize);
        }
    }
}
