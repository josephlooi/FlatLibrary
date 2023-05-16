using System;
using FlatEngine.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FlatEngine.Graphics
{
    public sealed class Screen : IDisposable
    {
        private const int MinDim = 64;
        private const int MaxDim = 4096;

        public readonly int Width;
        public readonly int Height;
        public readonly double HalfWidth;
        public readonly double HalfHeight;
        private readonly Game game;
        private readonly RenderTarget2D target;

        private bool isSet;
        private bool isDisposed;

        public Screen(Game game, int width, int height)
        {
            width = FlatTools.Clamp(width, Screen.MinDim, Screen.MaxDim);
            height = FlatTools.Clamp(height, Screen.MinDim, Screen.MaxDim);

            this.game = game ?? throw new ArgumentNullException(nameof(game));
            this.target = new RenderTarget2D(this.game.GraphicsDevice, width, height);
            this.Width = this.target.Width;
            this.Height = this.target.Height;
            this.HalfWidth = FlatMath.Div2(width);
            this.HalfHeight = FlatMath.Div2(height);
            this.isSet = false;
        }

        public void Dispose()
        {
            if (this.isDisposed) return;
            this.isDisposed = true;
            this.target?.Dispose();
        }

        public void Set()
        {
            if (this.isSet) throw new Exception("Render target already set.");
            this.game.GraphicsDevice.SetRenderTarget(this.target);
            this.isSet = true;
        }

        public void UnSet()
        {
            if (!this.isSet) throw new Exception("Render target already unset.");
            this.game.GraphicsDevice.SetRenderTarget(null);
            this.isSet = false;
        }

        public void Present(Sprites sprites, bool textureFiltering = true)
        {
            if (sprites == null) throw new ArgumentNullException(nameof(sprites));
            this.game.GraphicsDevice.Clear(Color.Black);
            sprites.Begin(null, textureFiltering);
            sprites.Draw(this.target, null, this.GetDestinationRect(), Color.White);
            sprites.End();
        }

        public Rectangle GetDestinationRect()
        {
            Rectangle backbufferBounds = this.game.GraphicsDevice.PresentationParameters.Bounds;
            double backbufferAspectRatio = (double)backbufferBounds.Width / backbufferBounds.Height;
            double screenAspectRatio = (double)this.Width / this.Height;

            double x = 0;
            double y = 0;
            double w = backbufferBounds.Width;
            double h = backbufferBounds.Height;

            if (backbufferAspectRatio > screenAspectRatio)
            {
                w = h * screenAspectRatio;
                x = FlatMath.Div2(backbufferBounds.Width - w);
            }
            else if (backbufferAspectRatio < screenAspectRatio)
            {
                h = w / screenAspectRatio;
                y = FlatMath.Div2(backbufferBounds.Height - h);
            }

            return new Rectangle((int)x, (int)y, (int)w, (int)h);
        }
    }
}
