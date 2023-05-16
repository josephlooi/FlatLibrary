using System;
using FlatEngine.Misc;
using FlatEngine.Util;
using Microsoft.Xna.Framework;

namespace FlatEngine.Graphics
{
    public sealed class Camera
    {
        private const double MinZ = 11d / 90_000d;
        private const double MaxZ = 4096;

        private readonly double aspectRatio;
        private readonly double fieldOfView;
        private readonly double scale;
        public  readonly double BaseZ;
        private readonly double scaleDivBaseZ;

        public FlatVector Position { get; private set; }
        public double Z { get; private set; }
        public double Zoom { get { return this.Z * this.scaleDivBaseZ; } }
        public Matrix View { get; private set; }
        public Matrix Projection { get; private set; }



        public Camera(Screen screen, double scale)
        {
            if (screen == null) throw new ArgumentNullException(nameof(screen));
            this.aspectRatio = (double)screen.Width / screen.Height;
            this.fieldOfView = FlatMath.PiDiv2;
            this.scale = scale;
            this.BaseZ = this.GetZFromScreenHeight(screen.Height);
            this.scaleDivBaseZ = scale / this.BaseZ;
            this.Reset();
            this.Update();
        }

        public void Update()
        {
            this.View = Matrix.CreateLookAt(new Vector3(this.Position.ToXna(), (float)this.Z), new Vector3(this.Position.ToXna(), 0f), Vector3.Up);
            this.Projection = Matrix.CreatePerspectiveFieldOfView((float)this.fieldOfView, (float)this.aspectRatio, (float)Camera.MinZ, (float)Camera.MaxZ);
        }
        public void Reset()
        {
            this.Position = FlatVector.Zero;
            this.Z = this.BaseZ;
        }
        


        public double GetZFromScreenHeight(double height)
        {
            return FlatMath.Div2(height / Math.Tan(FlatMath.Div2(this.fieldOfView)) * scale);
        }
        public double GetScreenHeightFromZ()
        {
            return FlatMath.Mul2(this.Z * Math.Tan(FlatMath.Div2(this.fieldOfView)));
        }



        public void MoveZ(double magnitude)
        {
            this.Z += magnitude;
            this.Z = FlatTools.Clamp(this.Z, Camera.MinZ, Camera.MaxZ);
        }
        public void Move(double x, double y)
        {
            this.Position += new FlatVector(x, y);
        }
        public void Move(FlatVector magnitude)
        {
            this.Position += magnitude;
        }
        public void MoveTo(double x, double y)
        {
            this.Position = new FlatVector(x, y);
        }
        public void MoveTo(FlatVector destination)
        {
            this.Position = destination;
        }



        public void GetExtents(out double width, out double height)
        {
            height = this.GetScreenHeightFromZ();
            width = height * this.aspectRatio;
        }
        public void GetExtents(out double left, out double right, out double bottom, out double top)
        {
            this.GetExtents(out double width, out double height);
            left = this.Position.X - FlatMath.Div2(width);
            right = left + width;
            bottom = this.Position.Y - FlatMath.Div2(height);
            top = bottom + height;
        }
        public void GetExtents(out FlatVector min, out FlatVector max)
        {
            this.GetExtents(out double left, out double right, out double bottom, out double top);
            min = new FlatVector(left, bottom);
            max = new FlatVector(right, top);
        }



        public bool ViewportContains(FlatVector p)
        {
            this.GetExtents(out FlatVector min, out FlatVector max);
            return p.IsInBoundary(min, max);
        }
        public bool ViewportContains(FlatAABB box)
        {
            this.GetExtents(out FlatVector min, out FlatVector max);
            return box.IsInBoundary(min, max);
        }
    }
}
