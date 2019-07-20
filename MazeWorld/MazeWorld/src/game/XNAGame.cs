using MazeWorld.src.game;
using MazeWorld.src.maze;
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
    /// <summary>
    /// Essential MonoGame/XNA class, contains main game loop, with drawing and controls.
    /// </summary>
    /// <remarks>This is a partial class. Only the inherited "Game" methods are in this file.
    /// The rest are private helpers in XNAGameHelper</remarks>
    public partial class XNAGame : Game
    {
        /// <summary>
        /// Makes a new game and initializes helpers. No textures are loaded.
        /// </summary>
        public XNAGame()
        {
            Content.RootDirectory = "Content";
            Graphics = new GraphicsDeviceManager(this);
            this.IsFixedTimeStep = false;
            this.IsMouseVisible = true;

            Ctrl = new ControlsHelper(this);
            GameMode = new MazeMode(Ctrl);
            DisableFullScreen();
        }

        // If any work needed to be done before content (textures) is loaded, it would be done here.
        protected override void Initialize()
        {
            base.Initialize();
        }
        

        /// <summary>
        /// Loads and sets default textures.
        /// </summary>
        protected override void LoadContent()
        {
            Texture2D t = this.Content.Load<Texture2D>("WhiteSquare16");
            SpriteFont f = this.Content.Load<SpriteFont>("Arial");
            SetTextures(t, f);
            base.LoadContent();
        }

        /// <summary>
        /// Performs work that needs to be done in the frame right before the game starts.
        /// </summary>
        protected override void BeginRun()
        {
            Ctrl.Update();//Stores keypress info, in case a key is pressed while the game starts.
        }


        /// <summary>
        /// Game loop. Each loop is 1 frame.
        /// </summary>
        /// <param name="gameTime"></param>
        protected override void Update(GameTime gameTime)
        {
            if (this.IsActive)
            {

                if (Ctrl.KeyFalling(Keys.Escape) || Ctrl.KeyFalling(Keys.Delete))
                    this.Exit();

                //Flips fullscreen
                if (Ctrl.KeyFalling(Keys.F))
                {
                    if (Graphics.IsFullScreen)
                        DisableFullScreen();
                    else
                        EnableFullScreen();
                }

                //Changes the texture size and reinstantiates the grid
                if (Ctrl.KeyFalling(Keys.D1))
                    ChangeTextureSize(64);
                else if (Ctrl.KeyFalling(Keys.D2))
                    ChangeTextureSize(32);
                else if (Ctrl.KeyFalling(Keys.D3))
                    ChangeTextureSize(16);
                else if (Ctrl.KeyFalling(Keys.D4))
                    ChangeTextureSize(8);
                else if (Ctrl.KeyFalling(Keys.D5))
                    ChangeTextureSize(4);
                else if (Ctrl.KeyFalling(Keys.D6))
                    ChangeTextureSize(2);
            }
            else if (!IsActive && Graphics.IsFullScreen)
                DisableFullScreen();

            GameMode.Update();

            Ctrl.Update();
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Gray);

            Batch.Begin();
            DrawHeader(gameTime);
            DrawGrid();
            Batch.End();

            base.Draw(gameTime);
        }
    }
}
