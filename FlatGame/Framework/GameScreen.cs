using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Xml.Serialization;

namespace FlatGame.Framework
{
    internal abstract class GameScreen
    {
        protected ContentManager content;
        [XmlIgnore]
        public Type Type;

        public GameScreen()
        {
            this.Type = this.GetType();
        }
        
        public virtual void LoadContent()
        {
            this.content = new ContentManager(ScreenManager.Instance.Content.ServiceProvider, "Content");
        }
        public virtual void UnloadContent()
        {
            this.content.Unload();
        }
        public abstract void Update(GameTime gameTime);
        public abstract void Draw(SpriteBatch spriteBatch);
    }
}
