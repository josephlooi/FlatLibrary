using System;
using FlatEngine;
using FlatEngine.Util;
using Microsoft.Xna.Framework;

namespace FlatPhysics.BodyType
{
    public class EqualTriangle : Polygon
    {
        protected readonly double length;

        /// <summary>Static Equal Triangle</summary>
        public EqualTriangle(double length, double restitution, double frictionStatic, double frictionDynamic) :
            base(new FlatVector[3], restitution, frictionStatic, frictionDynamic)
        {
            this.length = length;
            this.SetVertices();
        }
        /// <summary>Static Rotation Equal Triangle</summary>
        public EqualTriangle(double length, double restitution, double frictionStatic, double frictionDynamic, double mass) :
            base(new FlatVector[3], restitution, frictionStatic, frictionDynamic, mass)
        {
            this.length = length;
            this.SetVertices();
        }
        /// <summary>Dynamic Equal Triangle</summary>
        public EqualTriangle(double length, double restitution, double frictionStatic, double frictionDynamic, double mass, bool isPositionStatic = false) :
            base(new FlatVector[3], restitution, frictionStatic, frictionDynamic, mass, mass * length * length / 12, isPositionStatic)
        {
            this.length = length;
            this.SetVertices();
        }



        protected override void SetVertices()
        {
            double halfBase = FlatMath.Div2(this.length);
            double halfHeight = FlatMath.Div2(Math.Sqrt(FlatMath.SubtractSquaresOf(this.length, halfBase)))r;
            this.vertices[0] = new FlatVector(halfBase, -halfHeight);
            this.vertices[1] = new FlatVector(-halfBase, -halfHeight);
            this.vertices[2] = new FlatVector(0, halfHeight);
        }



        public static double GetMass(double length, double density)
        {
            const double root3Div4 = 0.433012701892219323381861585376468091735701313452595157;
            return length * length * root3Div4 * density;
        }
    }
}
