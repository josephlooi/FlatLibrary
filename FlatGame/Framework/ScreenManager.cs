using System;
using FlatEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace FlatGame.Framework
{
    internal class ScreenManager
    {
        private static readonly Lazy<ScreenManager> Lazy = new Lazy<ScreenManager>(() => new ScreenManager());
        public static ScreenManager Instance { get { return Lazy.Value; } }
        public readonly int ResolutionX;
        public readonly int ResolutionY;
        public ContentManager Content;
        protected GameScreen currentScreen;
        protected XmlManager<GameScreen> xmlGameScreenManager;



        protected ScreenManager()
        {
            this.ResolutionX = 1280;
            this.ResolutionY = 720;
            this.currentScreen = new SplashScreen();
            this.xmlGameScreenManager = new XmlManager<GameScreen>();
            this.xmlGameScreenManager.Type = this.currentScreen.Type;
            this.currentScreen = this.xmlGameScreenManager.Load("Load/SplashScreen.xml");

        }
        public void LoadContent(ContentManager content)
        {
            this.Content = new ContentManager(content.ServiceProvider, "Content");
            this.currentScreen.LoadContent();
        }
        public void UnloadContent()
        {
            this.currentScreen.UnloadContent();
            this.Content.Unload();
        }
        public void Update(GameTime gameTime)
        {
            this.currentScreen.Update(gameTime);
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            this.currentScreen.Draw(spriteBatch);
        }
    }
}
