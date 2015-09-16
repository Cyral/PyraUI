﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Pyratron.UI.Demo
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game : Microsoft.Xna.Framework.Game
    {
        private Monogame.Manager UI { get; }

        private SpriteFont Font { get; set; }
        private readonly GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        public Game()
        {
            graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferHeight = 768,
                PreferredBackBufferWidth = 1280
            };
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
            spriteBatch = new SpriteBatch(GraphicsDevice);

            UI.Content = Content;
            UI.SpriteBatch = spriteBatch;
            UI.Font = Font;
            UI.Load();
            UI.Init(); 
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            UI.Update((float) gameTime.ElapsedGameTime.TotalSeconds);
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);
            UI.Draw((float) gameTime.ElapsedGameTime.TotalSeconds);
            base.Draw(gameTime);
        }
    }
}