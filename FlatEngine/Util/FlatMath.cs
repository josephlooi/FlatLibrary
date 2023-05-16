using Microsoft.Xna.Framework;
using System;

namespace FlatEngine.Util
{
    public class FlatMath
    {
        public const double MinAccuracy = 0.000000015; // 15 nanometers
        public const double Pi     = 3.14159265358979323846264338327950288419716939937510582;
        public const double PiSqRt = 1.7724538509055160272981674833411451827975494561223871282138;
        public const double PiMul2 = Pi * 2;
        public const double PiDiv2 = Pi * 0.5;
        public const double PiDiv4 = Pi * 0.25;
        public const double PiDiv8 = Pi * 0.125;

        public static double Average(double a, double b)
        {
            return FlatMath.Div2(a + b);
        }


        public static double Div2(double a, int pow = 1)
        {
            return Math.ScaleB(a, -pow);
        }
        public static double Mul2(double a, int pow = 1)
        {
            return Math.ScaleB(a, pow);
        }
        public static float Div2(float a, int pow = 1)
        {
            return MathF.ScaleB(a, -pow);
        }
        public static float Mul2(float a, int pow = 1)
        {
            return MathF.ScaleB(a, pow);
        }
        public static FlatVector Div2(FlatVector a, int pow = 1)
        {
            return new FlatVector(Math.ScaleB(a.X, -pow), Math.ScaleB(a.Y, -pow));
        }
        public static FlatVector Mul2(FlatVector a, int pow = 1)
        {
            return new FlatVector(Math.ScaleB(a.X, pow), Math.ScaleB(a.Y, pow));
        }
        public static Vector2 Div2(Vector2 a, int pow = 1)
        {
            return new Vector2(MathF.ScaleB(a.X, -pow), MathF.ScaleB(a.Y, -pow));
        }
        public static Vector2 Mul2(Vector2 a, int pow = 1)
        {
            return new Vector2(MathF.ScaleB(a.X, pow), MathF.ScaleB(a.Y, pow));
        }
    }
}
