using System;
using Microsoft.Xna.Framework;

namespace FlatEngine.Util
{
    public static class FlatRandom
    {
        public static Random Rand = new Random();

        public static int Integer()
        {
            return Rand.Next();
        }

        public static int Integer(Random rand)
        {
            return rand.Next();
        }
        public static int Integer(int min, int max)
        {
            if (min >= max) (min, max) = (max, min);
            return min + Rand.Next() % (max - min);
        }
        public static int Integer(Random rand, int min, int max)
        {
            if (min >= max) (min, max) = (max, min);
            return min + rand.Next() % (max - min);
        }


        public static double Percent()
        {
            return Rand.NextDouble();
        }
        public static double Percent(Random rand)
        {
            return rand.NextDouble();
        }
        public static double Double(double min, double max)
        {
            if (min >= max) (min, max) = (max, min);
            return min + Rand.NextDouble() * (max - min);
        }
        public static double Double(Random rand, double min, double max)
        {
            if (min >= max) (min, max) = (max, min);
            return min + rand.NextDouble() * (max - min);
        }


        public static bool Bool()
        {
            int result = FlatRandom.Integer(0, 2);
            if (result == 0) return false;
            else return true;
        }


        public static Color Color()
        {
            return new Color(Rand.NextSingle(), Rand.NextSingle(), Rand.NextSingle());
        }
        public static Color Color(Random rand)
        {
            return new Color(rand.NextSingle(), rand.NextSingle(), rand.NextSingle());
        }



        public static FlatVector Direction()
        {
            Direction(Rand, out double x, out double y);
            return new FlatVector(x, y);
        }
        public static void Direction(out double x, out double y)
        {
            double angle = Double(Rand, 0, FlatMath.PiMul2);
            (x, y) = Math.SinCos(angle);
        }
        public static FlatVector Direction(Random rand)
        {
            Direction(rand, out double x, out double y);
            return new FlatVector(x, y);
        }
        public static void Direction(Random rand, out double x, out double y)
        {
            double angle = Double(rand, 0, FlatMath.PiMul2);
            (x, y) = Math.SinCos(angle);
        }
    }
}
