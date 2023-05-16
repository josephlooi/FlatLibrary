using FlatEngine;
using FlatEngine.Misc;
using FlatEngine.Util;
using Microsoft.Xna.Framework;

namespace FlatPhysics.BodyType
{
    public class Circle : Body
    {
        public readonly double Radius;

        /// <summary>Static Circle</summary>
        public Circle(double radius, double restitution, double frictionStatic, double frictionDynamic) :
            base(restitution, frictionStatic, frictionDynamic)
        {
            this.Radius = radius;
        }
        /// <summary>Static Rotation Circle</summary>
        public Circle(double radius, double restitution, double frictionStatic, double frictionDynamic, double mass) :
            base(restitution, frictionStatic, frictionDynamic, mass)
        {
            this.Radius = radius;
        }
        /// <summary>Dynamic Circle</summary>
        public Circle(double radius, double restitution, double frictionStatic, double frictionDynamic, double mass, bool isPositionStatic) : 
            base(restitution, frictionStatic, frictionDynamic, mass, FlatMath.Div2(radius * radius * mass), isPositionStatic)
        {
            this.Radius = radius;
        }


        public override void Update(double time)
        {
            base.Update(time);
            this.SetAABB();
        }
        protected override void DrawBody(FlatEngine.Graphics.Shapes shapes)
        {
            shapes.DrawCircleFill(this.Position, this.Radius, this.bodyColor.Value);
            //FlatVector radius = new FlatVector(this.Radius, 0f).Transform(new FlatTransform(this.Position, this.Angle));
            //shapes.DrawLine(this.Position, radius, Body.outlineThickness, Body.outlineColor);
        }
        protected override void DrawOutline(FlatEngine.Graphics.Shapes shapes)
        {
            shapes.DrawCircle(this.Position, this.Radius, this.lineThickness, this.lineColor.Value);
        }
        protected override void SetAABB()
        {
            double minX = this.Position.X - this.Radius;
            double minY = this.Position.Y - this.Radius;
            double maxX = this.Position.X + this.Radius;
            double maxY = this.Position.Y + this.Radius;
            this.AABB = new FlatAABB(minX, minY, maxX, maxY);
        }


        public static double GetMass(double radius, double density)
        {
            return radius * radius * FlatMath.Pi * density;
        }
    }
}
