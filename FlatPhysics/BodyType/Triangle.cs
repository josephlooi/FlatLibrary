using System;
using FlatEngine;
using FlatEngine.Util;
using Microsoft.Xna.Framework;

namespace FlatPhysics.BodyType
{
    public class Triangle : Polygon
    {
        private const double minAngle = FlatMath.PiDiv8;
        private const double maxAngle = FlatMath.PiDiv2;
        private double @base; // b
        private double height; // h
        private double angle; // 0
        private double heightDepth; // d
        private Triangle(double restitution, double frictionStatic, double frictionDynamic) : 
            base(new FlatVector[3], restitution, frictionStatic, frictionDynamic)
        { }
        private Triangle(double restitution, double frictionStatic, double frictionDynamic, double mass) :
           base(new FlatVector[3], restitution, frictionStatic, frictionDynamic, mass)
        { }
        private Triangle(double restitution, double frictionStatic, double frictionDynamic, double mass, double inertia, bool isPositionStatic) :
            base(new FlatVector[3], restitution, frictionStatic, frictionDynamic, mass, inertia, isPositionStatic)
        { }



        /// <summary> Static ABC </summary>
        public static Triangle ABC(double a, double b, double c, double restitution, double frictionStatic, double frictionDynamic)
        {
            Triangle tri = new Triangle(restitution, frictionStatic, frictionDynamic);
            tri.@base = b;
            tri.height = Triangle.GetHeight(a, b, c);
            tri.angle = Triangle.GetAngle(a, b, c);
            tri.heightDepth = Triangle.GetHeightDepth(tri.height, tri.angle);
            tri.SetVertices();
            return tri;
        }
        /// <summary> Static Rotation ABC </summary>
        public static Triangle ABC(double a, double b, double c, double restitution, double frictionStatic, double frictionDynamic, double mass)
        {
            Triangle tri = new Triangle(restitution, frictionStatic, frictionDynamic, mass);
            tri.@base = b;
            tri.height = Triangle.GetHeight(a, b, c);
            tri.angle = Triangle.GetAngle(a, b, c);
            tri.heightDepth = Triangle.GetHeightDepth(tri.height, tri.angle);
            tri.SetVertices();
            return tri;
        }
        /// <summary> Dynamic ABC </summary>
        public static Triangle ABC(double a, double b, double c, double restitution, double frictionStatic, double frictionDynamic, double mass, bool isPositionStatic)
        {
            double inertia = Triangle.GetInertia(a, b, c, mass);
            Triangle tri = new Triangle(restitution, frictionStatic, frictionDynamic, mass, inertia, isPositionStatic);
            tri.@base = b;
            tri.height = Triangle.GetHeight(a, b, c);
            tri.angle = Triangle.GetAngle(a, b, c);
            tri.heightDepth = Triangle.GetHeightDepth(tri.height, tri.angle);
            tri.SetVertices();
            return tri;
        }



        /// <summary> Static BH0 </summary>
        public static Triangle BH0(double @base, double height, double angle, double restitution, double frictionStatic, double frictionDynamic)
        {
            Triangle.BH0toABC(@base, height, angle, out double a, out double c, out double heightDepth);
            Triangle tri = new Triangle(restitution, frictionStatic, frictionDynamic);
            tri.@base = @base;
            tri.height = height;
            tri.angle = FlatTools.Clamp(angle, Triangle.minAngle, Triangle.maxAngle);
            tri.heightDepth = heightDepth;
            tri.SetVertices();
            return tri;
        }
        /// <summary> Static Rotation BH0 </summary>
        public static Triangle BH0(double @base, double height, double angle, double restitution, double frictionStatic, double frictionDynamic, double mass)
        {
            Triangle.BH0toABC(@base, height, angle, out _, out _, out double heightDepth);
            Triangle tri = new Triangle(restitution, frictionStatic, frictionDynamic, mass);
            tri.@base = @base;
            tri.height = height;
            tri.angle = FlatTools.Clamp(angle, Triangle.minAngle, Triangle.maxAngle);
            tri.heightDepth = heightDepth;
            tri.SetVertices();
            return tri;
        }
        /// <summary> Dynamic BH0 </summary>
        public static Triangle BH0(double @base, double height, double angle, double restitution, double frictionStatic, double frictionDynamic, double mass, bool isPositionStatic)
        {
            Triangle.BH0toABC(@base, height, angle, out double a, out double c, out double heightDepth);
            double inertia = Triangle.GetInertia(a, @base, c, mass);
            Triangle tri = new Triangle(restitution, frictionStatic, frictionDynamic, mass, inertia, isPositionStatic);
            tri.@base = @base;
            tri.height = height;
            tri.angle = FlatTools.Clamp(angle, Triangle.minAngle, Triangle.maxAngle);
            tri.heightDepth = heightDepth;
            tri.SetVertices();
            return tri;
        }



        /// <summary> Static BHD </summary>
        public static Triangle BHD(double @base, double height, double heightDepth, double restitution, double frictionStatic, double frictionDynamic)
        {
            Triangle.BHDtoABC(@base, height, heightDepth, out double a, out double c, out double angle);
            Triangle tri = new Triangle(restitution, frictionStatic, frictionDynamic);
            tri.@base = @base;
            tri.height = height;
            tri.angle = FlatTools.Clamp(angle, Triangle.minAngle, Triangle.maxAngle);
            tri.heightDepth = heightDepth;
            tri.SetVertices();
            return tri;
        }
        /// <summary> Static Rotation BHD </summary>
        public static Triangle BHD(double @base, double height, double heightDepth, double restitution, double frictionStatic, double frictionDynamic, double mass)
        {
            Triangle.BHDtoABC(@base, height, heightDepth, out double a, out double c, out double angle);
            double inertia = Triangle.GetInertia(a, @base, c, mass);
            Triangle tri = new Triangle(restitution, frictionStatic, frictionDynamic, mass);
            tri.@base = @base;
            tri.height = height;
            tri.angle = FlatTools.Clamp(angle, Triangle.minAngle, Triangle.maxAngle);
            tri.heightDepth = heightDepth;
            tri.SetVertices();
            return tri;
        }
        /// <summary> Dynamic BHD </summary>
        public static Triangle BHD(double @base, double height, double heightDepth, double restitution, double frictionStatic, double frictionDynamic, double mass, bool isPositionStatic)
        {
            Triangle.BHDtoABC(@base, height, heightDepth, out double a, out double c, out double angle);
            double inertia = Triangle.GetInertia(a, @base, c, mass);
            Triangle tri = new Triangle(restitution, frictionStatic, frictionDynamic, mass, inertia, isPositionStatic);
            tri.@base = @base;
            tri.height = height;
            tri.angle = FlatTools.Clamp(angle, Triangle.minAngle, Triangle.maxAngle);
            tri.heightDepth = heightDepth;
            tri.SetVertices();
            return tri;
        }



        protected override void SetVertices()
        {
            this.vertices[0] = new FlatVector(0, this.@base);
            this.vertices[1] = new FlatVector(0, 0);
            this.vertices[2] = new FlatVector(this.heightDepth, this.height);
            base.SetVertices();
        }
        public static double GetMass(double a, double b, double c, double density)
        {
            return GetArea(a, b, c) * density;
        }
        public static double GetMass(double @base, double height, double density)
        {
            return GetArea(@base, height) * density;
        }


        private static double GetArea(double a, double b, double c)
        {
            double s = FlatMath.Div2(a + b + c);
            return Math.Sqrt(s * (s - a) * (s - b) * (s - c));
        }
        private static double GetArea(double @base, double height)
        {
            return FlatMath.Div2(@base * height);
        }
        private static double GetHeight(double @base, double height)
        {
            double area = Triangle.GetArea(@base, height);
            return FlatMath.Mul2(area) / @base;
        }
        private static double GetHeight(double a, double b, double c)
        {
            double area = Triangle.GetArea(a, b, c);
            return FlatMath.Mul2(area) / b;
        }
        private static double GetAngle(double a, double b, double c)
        {
            double bc = FlatMath.AddSquaresOf(b, c);
            double bca = FlatMath.MulAdd(a, -a, bc);
            return Math.Acos(bca / (2 * b * c)); // cosine rule
        }
        private static double GetAngle(double height, double heightDepth)
        {
            return Math.Atan(height / heightDepth);
        }
        private static double GetHeightDepth(double height, double angle)
        {
            return height / Math.Tan(angle);
        }
        private static double GetInertia(double a, double b, double c, double mass)
        {
            double ab = FlatMath.AddSquaresOf(a, b);
            double abc = FlatMath.MulAdd(c, c, ab);
            return mass * abc / 36;
        }
        private static void BH0toABC(double @base, double height, double angle, out double a, out double c, out double heightDepth)
        {
            heightDepth = Triangle.GetHeightDepth(height, angle);
            double b_ = @base - heightDepth;
            a = Math.Sqrt(FlatMath.AddSquaresOf(b_, height));
            c = height / Math.Sin(angle);
        }
        private static void BHDtoABC(double @base, double height, double heightDepth, out double a, out double c, out double angle)
        {
            double b_ = @base - heightDepth;
            angle = Triangle.GetAngle(height, heightDepth);
            a = Math.Sqrt(FlatMath.AddSquaresOf(b_, height));
            c = height / Math.Sin(angle);
        }
    }
}
