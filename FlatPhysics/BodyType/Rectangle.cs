using System;
using FlatEngine;
using FlatEngine.Util;
using Microsoft.Xna.Framework;

namespace FlatPhysics.BodyType
{
    public class Rectangle : Polygon
    {
        protected readonly double width;
        protected readonly double height;

        /// <summary>Static Rectangle</summary>
        public Rectangle(double width, double height, double restitution, double frictionStatic, double frictionDynamic) :
            base(new FlatVector[4], restitution, frictionStatic, frictionDynamic)
        {
            this.width = width;
            this.height = height;
            this.SetVertices();
        }
        /// <summary>Static Rotation Rectangle</summary>
        public Rectangle(double width, double height, double restitution, double frictionStatic, double frictionDynamic, double mass) :
            base(new FlatVector[4], restitution, frictionStatic, frictionDynamic, mass)
        {
            this.width = width;
            this.height = height;
            this.SetVertices();
        }
        /// <summary>Dynamic Rectangle</summary>
        public Rectangle(double width, double height, double restitution, double frictionStatic, double frictionDynamic, double mass, bool isPositionStatic) :
            base(new FlatVector[4], restitution, frictionStatic, frictionDynamic, mass, mass * Math.FusedMultiplyAdd(width, width, height * height) / 12, isPositionStatic)
        {
            this.width = width;
            this.height = height;
            this.SetVertices();
        }



        protected override void SetVertices()
        {
            double left = -FlatMath.Div2(this.width);
            double right = left + this.width;
            double bottom = -FlatMath.Div2(this.height);
            double top = bottom + this.height;

            this.vertices[0] = new FlatVector(left, top);
            this.vertices[1] = new FlatVector(right, top);
            this.vertices[2] = new FlatVector(right, bottom);
            this.vertices[3] = new FlatVector(left, bottom);
        }



        public static double GetMass(double width, double length, double density)
        {
            return width * length * density;
        }
    }
}
