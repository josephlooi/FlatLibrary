using FlatEngine.Graphics;
using Microsoft.Xna.Framework.Graphics;
using System;
using FlatEngine.Misc;

namespace FlatEngine
{
    public class FlatSprite : FlatEntity
    {
        protected Texture2D texture;
        protected double scale;

        public FlatSprite(Texture2D texture, double scale, bool isPositionStatic, bool isRotationStatic) :
            base(isPositionStatic, isRotationStatic)
        {
            this.texture = texture;
            this.scale = scale;
        }



        protected override void SetAABB()
        {
            throw new NotImplementedException();
        }
        public void Draw(Sprites sprites, Camera camera)
        {
            if (!camera.ViewportContains(this.AABB)) return;
            sprites.Draw(this.texture, this.Position, this.Rotation, this.scale);
        }
    }
}
