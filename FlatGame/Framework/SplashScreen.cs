using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlatGame.Framework
{
    internal class SplashScreen : GameScreen
    {
        protected Texture2D texture;
        public string Path;

        public override void LoadContent()
        {
            base.LoadContent();
            this.texture = this.content.Load<Texture2D>(this.Path);
        }
        public override void UnloadContent()
        {
            base.UnloadContent();
        }
        public override void Update(GameTime gameTime)
        {
            
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, new Vector2(-100, 100), Color.White);
        }
    }
}
