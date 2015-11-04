using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Pyratron.UI.Monogame;

namespace Pyratron.UI.Demo
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game : Microsoft.Xna.Framework.Game
    {
        private Monogame.Manager UI { get; }

        private SpriteBatch spriteBatch;

        private GraphicsDeviceManager graphics;

        private Primitives primitives;
        public Game()
        {
            graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferHeight = 768,
                PreferredBackBufferWidth = 1024,
                PreferMultiSampling = true,
                SynchronizeWithVerticalRetrace = false,
            };
            IsMouseVisible = true;
            IsFixedTimeStep = false;
            graphics.ApplyChanges();
            UI = new Monogame.Manager();
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(graphics.GraphicsDevice);
            primitives = new Primitives(graphics.GraphicsDevice);

            UI.Content = Content;
            UI.SpriteBatch = spriteBatch;
            UI.Load();
            UI.Init(); 
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            var ks = Keyboard.GetState();
            if (ks.IsKeyDown(Keys.Escape))
                Exit();
            UI.Update((float) gameTime.ElapsedGameTime.TotalSeconds);
            UI.DrawDebug = ks.IsKeyDown(Keys.D);
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.White);
            UI.Draw((float) gameTime.ElapsedGameTime.TotalSeconds);

            base.Draw(gameTime);
        }
    }
}