using System;
using Microsoft.Xna.Framework;

namespace FlatEngine
{
    public readonly struct FlatTransform
    {
        public readonly FlatVector Position;
        public readonly FlatVector SinScale;
        public readonly FlatVector CosScale;

        public static readonly FlatTransform Zero = new FlatTransform(0, 0, 0);

        public FlatTransform(FlatVector position, double angle)
        {
            this.Position = position;
            (double sin, double cos) = Math.SinCos(angle);
            this.SinScale = new FlatVector(sin, sin);
            this.CosScale = new FlatVector(cos, cos);
        }
        public FlatTransform(double x, double y, double angle)
        {
            this.Position = new FlatVector(x, y);
            (double sin, double cos) = Math.SinCos(angle);
            this.SinScale = new FlatVector(sin, sin);
            this.CosScale = new FlatVector(cos, cos);
        }
        public FlatTransform(FlatVector position, double angle, double scale)
        {
            this.Position = position;
            (double sin, double cos) = Math.SinCos(angle);
            this.SinScale = new FlatVector(sin, sin) * scale;
            this.CosScale = new FlatVector(cos, cos) * scale;
        }
        public FlatTransform(double x, double y, double angle, double scale)
        {
            this.Position = new FlatVector(x, y);
            (double sin, double cos) = Math.SinCos(angle);
            this.SinScale = new FlatVector(sin, sin) * scale;
            this.CosScale = new FlatVector(cos, cos) * scale;
        }



        public static FlatTransform Rotate(double angle)
        {
            return new FlatTransform(0, 0, angle);
        }
        public static FlatTransform MoveTo(FlatVector p)
        {
            return new FlatTransform(p, 0);
        }
        public static FlatTransform MoveTo(double x, double y)
        {
            return new FlatTransform(x, y, 0);
        }
        public static FlatTransform Scale(double scale)
        {
            return new FlatTransform(0, 0, 0, scale);
        }



        public Matrix ToMatrix()
        {
            Matrix result = Matrix.Identity;
            result.M11 = (float)this.CosScale.X;
            result.M12 = (float)this.SinScale.Y;
            result.M21 = (float)-this.SinScale.X;
            result.M22 = (float)this.CosScale.Y;
            result.M41 = (float)this.Position.X;
            result.M42 = (float)this.Position.Y;
            return result;
        }
    }
}
