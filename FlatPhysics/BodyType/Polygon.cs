using FlatEngine;
using FlatEngine.Graphics;
using FlatEngine.Misc;
using Microsoft.Xna.Framework;

namespace FlatPhysics.BodyType
{
    public class Polygon : Body
    {
        protected readonly FlatVector[] vertices;
        internal readonly FlatVector[] TransformedVertices;

        /// <summary>Static Polygon</summary>
        public Polygon(FlatVector[] vertices, double restitution, double frictionStatic, double frictionDynamic) :
            base(restitution, frictionStatic, frictionDynamic)
        {
            this.vertices = vertices;
            this.TransformedVertices = new FlatVector[this.vertices.Length];
        }
        /// <summary>Static Rotation polygon</summary>
        protected Polygon(FlatVector[] vertices, double restitution, double frictionStatic, double frictionDynamic, double mass) :
            base(restitution, frictionStatic, frictionDynamic, mass)
        {
            this.vertices = vertices;
            this.TransformedVertices = new FlatVector[this.vertices.Length];
        }
        /// <summary>Dynamic polygon</summary>
        protected Polygon(FlatVector[] vertices, double restitution, double frictionStatic, double frictionDynamic, double mass, double inertia, bool isPositionStatic = false) :
            base(restitution, frictionStatic, frictionDynamic, mass, inertia, isPositionStatic)
        {
            this.vertices = vertices;
            this.TransformedVertices = new FlatVector[this.vertices.Length];
        }



        public override void Update(double time)
        {
            base.Update(time);
            this.Transform();
        }
        protected override void DrawBody(Shapes shapes)
        {
            shapes.DrawPolygonFill(this.TransformedVertices, this.bodyColor.Value);
        }
        protected override void DrawOutline(Shapes shapes)
        {
            shapes.DrawPolygon(this.TransformedVertices, this.lineThickness, this.lineColor.Value);
        }
        protected override void SetAABB()
        {
            double minX = double.MaxValue;
            double minY = double.MaxValue;
            double maxX = double.MinValue;
            double maxY = double.MinValue;

            for (int i = 0; i < this.TransformedVertices.Length; i++)
            {
                FlatVector v = this.TransformedVertices[i];
                if (v.X < minX) minX = v.X;
                if (v.X > maxX) maxX = v.X;
                if (v.Y < minY) minY = v.Y;
                if (v.Y > maxY) maxY = v.Y;
            }
            this.AABB = new FlatAABB(minX, minY, maxX, maxY);
        }



        protected void Transform()
        {
            FlatTransform transform = new FlatTransform(this.Position, this.Rotation);
            for (int i = 0; i < this.vertices.Length; i++) 
                this.TransformedVertices[i] = this.vertices[i].Transform(transform);
            this.SetAABB();
        }
        protected virtual void SetVertices()
        {
            FlatVector adjust = this.Position - Shapes.GetCenter(this.vertices);
            if (adjust != FlatVector.Zero) for (int i = 0; i < this.vertices.Length; i++) this.vertices[i] += adjust;
        }

        public static double GetMass(FlatVector[] vertices, double density)
        {
            return Shapes.GetArea(vertices) * density;
        }
    }
}