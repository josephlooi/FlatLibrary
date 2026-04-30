using FlatEngine;
using FlatEngine.Graphics;
using Microsoft.Xna.Framework.Graphics;

namespace FlatPhysics
{
    public class Sprite : FlatSprite
    {
        public Body HitBox { get; }

        public Sprite(Texture2D texture, double scale, Body hitBox) :
            base(texture, scale, hitBox.isPositionStatic, hitBox.isRotationStatic)
        {
            this.HitBox = hitBox;
        }



        protected override void SetAABB()
        {
            this.AABB = this.HitBox.AABB;
        }
        public override void Update(double time)
        {
            if (this.isStatic) return;
            this.HitBox.Update(time);
            this.Position = this.HitBox.Position;
            this.Rotation = this.HitBox.Rotation;
            this.SetAABB();
        }
        internal void Draw(Sprites sprites, Shapes shapes, Camera camera)
        {
            base.Draw(sprites, camera);
            this.HitBox.Draw(shapes, camera);
        }



        public override void Move(FlatVector magnitude)
        {
            this.HitBox.Move(magnitude);
        }
        public override void MoveTo(FlatVector destination)
        {
            this.HitBox.MoveTo(destination);
        }
        public override void Move(double x, double y)
        {
            this.HitBox.Move(x, y);
        }
        public override void MoveTo(double x, double y)
        {
            this.HitBox.MoveTo(x, y);
        }
        public override void Rotate(double magnitude)
        {
            this.HitBox.Rotate(magnitude);
        }
        public override void RotateTo(double magnitude)
        {
            this.HitBox.RotateTo(magnitude);
        }
        public override void LinearAccelerate(FlatVector magnitude)
        {
            this.HitBox.LinearAccelerate(magnitude);
        }
        public override void AngularAccelerate(double magnitude)
        {
            this.HitBox.AngularAccelerate(magnitude);
        }
    }
}
