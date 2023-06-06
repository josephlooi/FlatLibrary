using FlatEngine;
using FlatEngine.Util;
using Microsoft.Xna.Framework;

namespace FlatPhysics.BodyType
{
    public class Square : Polygon
    {
        protected readonly double length;

        /// <summary>Static Square</summary>
        public Square(double length, double restitution, double frictionStatic, double frictionDynamic) :
            base(new FlatVector[4], restitution, frictionStatic, frictionDynamic)
        {
            this.length = length;
            this.SetVertices();
        }
        /// <summary>Static Rotation Square</summary>
        public Square(double length, double restitution, double frictionStatic, double frictionDynamic, double mass) :
            base(new FlatVector[4], restitution, frictionStatic, frictionDynamic, mass)
        {
            this.length = length;
            this.SetVertices();
        }
        /// <summary>Dynamic Square</summary>
        public Square(double length, double restitution, double frictionStatic, double frictionDynamic, double mass, bool isPositionStatic = false) :
            base(new FlatVector[4], restitution, frictionStatic, frictionDynamic, mass, mass * length * length / 6, isPositionStatic)
        {
            this.length = length;
            this.SetVertices();
        }



        protected override void SetVertices()
        {
            double min = -FlatMath.Div2(this.length);
            double max = min + this.length;
            this.vertices[0] = new FlatVector(min, max);
            this.vertices[1] = new FlatVector(max, max);
            this.vertices[2] = new FlatVector(max, min);
            this.vertices[3] = new FlatVector(min, min);
        }


        public static double GetMass(double length, double density)
        {
            return length * length * density;
        }
    }
}
