using System;
using System.Text;
using FlatEngine.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FlatEngine.Graphics
{
    public sealed class Sprites : IDisposable
    {
        private readonly Game game;
        private readonly SpriteBatch sprites;
        private readonly BasicEffect effect;

        private bool isDisposed;
        private Camera camera;



        public Sprites(Game game, Camera camera)
        {
            this.game = game ?? throw new ArgumentNullException(nameof(game));
            this.camera = camera;
            this.isDisposed = false;
            this.sprites = new SpriteBatch(this.game.GraphicsDevice);
            this.effect = new BasicEffect(this.game.GraphicsDevice);
            this.effect.FogEnabled = false;
            this.effect.LightingEnabled = false;
            this.effect.PreferPerPixelLighting = false;
            this.effect.VertexColorEnabled = true;
            this.effect.Texture = null;
            this.effect.TextureEnabled = true;
            this.effect.Projection = Matrix.Identity;
            this.effect.View = Matrix.Identity;
            this.effect.World = Matrix.Identity;

        }
        public void Dispose()
        {
            if (this.isDisposed) return;
            this.effect?.Dispose();
            this.sprites?.Dispose();
            this.isDisposed = true;
            GC.SuppressFinalize(this);
        }
        public void Begin(Camera camera, bool isTextureFilteringEnabled = false)
        {
            SamplerState samplerState = SamplerState.PointClamp;
            if (isTextureFilteringEnabled) samplerState = SamplerState.LinearClamp;

            if (camera == null)
            {
                Viewport vp = this.game.GraphicsDevice.Viewport;
                this.effect.View = Matrix.Identity;
                this.effect.Projection = Matrix.CreateOrthographicOffCenter(0f, vp.Width, 0f, vp.Height, 0f, 1f);
            }
            else
            {
                this.camera = camera;
                this.camera.Update();
                this.effect.View = camera.View;
                this.effect.Projection = camera.Projection;
            }

            this.sprites?.Begin(blendState: BlendState.AlphaBlend, samplerState: samplerState, rasterizerState: RasterizerState.CullNone, effect: this.effect);
        }
        public void End()
        {
            this.sprites.End();
        }



        public void Draw(Texture2D texture, FlatVector position, double angle, double scale)
        {
            Vector2 origin = FlatMath.Div2(new Vector2(texture.Width, texture.Height));
            this.sprites.Draw(texture, position.ToXna(), null, Color.White, (float)angle, origin, (float)scale, SpriteEffects.FlipVertically, 0f);
        }
        public void Draw(Texture2D texture, FlatVector origin, FlatVector position, Color color)
        {
            this.sprites.Draw(texture, position.ToXna(), null, color, 0f, origin.ToXna(), 1f, SpriteEffects.FlipVertically, 0f);
        }
        public void Draw(Texture2D texture, Rectangle? sourceRect, FlatVector origin, FlatVector position, double angle, double scale, Color color)
        {
            this.sprites.Draw(texture, position.ToXna(), sourceRect, color, (float)angle, origin.ToXna(), new Vector2((float)scale, (float)scale), SpriteEffects.FlipVertically, 0f);
        }
        public void Draw(Texture2D texture, Rectangle? sourceRect, Rectangle destinationRect, Color color)
        {
            this.sprites.Draw(texture, destinationRect, sourceRect, color, 0f, Vector2.Zero, SpriteEffects.FlipVertically, 0f);
        }



        public void DrawString(SpriteFont font, string text, FlatVector position, Color color)
        {
            this.sprites.DrawString(font, text, position.ToXna(), color, 0f, Vector2.Zero, 1f, SpriteEffects.FlipVertically, 0f);
        }
        public void DrawString(SpriteFont font, StringBuilder text, FlatVector position, Color color)
        {
            this.sprites.DrawString(font, text, position.ToXna(), color, 0f, Vector2.Zero, 1f, SpriteEffects.FlipVertically, 0f);
        }
        public void DrawString(SpriteFont font, string text, FlatVector origin, FlatVector position, double angle, double scale, Color color)
        {
            this.sprites.DrawString(font, text, position.ToXna(), color, (float)angle, origin.ToXna(), (float)scale, SpriteEffects.FlipVertically, 0f);
        }
    }
}
