using System;
using FlatEngine.Misc;

namespace FlatEngine
{
    public abstract class FlatEntity
    {
        public readonly bool isPositionStatic;
        public readonly bool isRotationStatic;
        public readonly bool isStatic;
        public FlatAABB AABB;
        public FlatVector Position { get; protected set; }
        public double Rotation { get; protected set; }
        public FlatVector LinearVelocity { get; protected set; }
        public double AngularVelocity { get; protected set; }

        public FlatEntity(bool isPositionStatic, bool isRotationStatic)
        {
            this.isPositionStatic = isPositionStatic;
            this.isRotationStatic = isRotationStatic;
            this.isStatic = isPositionStatic && isRotationStatic;
            this.Position = FlatVector.Zero;
            this.Rotation = 0;
            this.LinearVelocity = FlatVector.Zero;
            this.AngularVelocity = 0;
        }


        public virtual void Update(double time)
        {
            if (!this.isStatic) this.Position += this.LinearVelocity * time;
            if (!this.isRotationStatic) this.Rotation += this.AngularVelocity * time;
            if (!this.isStatic) this.SetAABB();
        }
        protected abstract void SetAABB();



        public virtual void Move(FlatVector magnitude)
        {
            this.Position += magnitude;
        }
        public virtual void MoveTo(FlatVector destination)
        {
            this.Position = destination;
        }
        public virtual void Move(double x, double y)
        {
            this.Position += new FlatVector(x, y);
        }
        public virtual void MoveTo(double x, double y)
        {
            this.Position = new FlatVector(x, y);
        }
        public virtual void Rotate(double magnitude)
        {
            this.Rotation += magnitude;
        }
        public virtual void RotateTo(double magnitude)
        {
            this.Rotation = magnitude;
        }
        public virtual void LinearAccelerate(FlatVector magnitude)
        {
            if (!this.isPositionStatic) this.LinearVelocity += magnitude;
        }
        public virtual void LinearAccelerateTo(FlatVector magnitude)
        {
            if (!this.isPositionStatic) this.LinearVelocity = magnitude;
        }
        public virtual void AngularAccelerate(double magnitude)
        {
            if (!this.isRotationStatic) this.AngularVelocity += magnitude;
        }
        public virtual void AngularAccelerateTo(double magnitude)
        {
            if (!this.isRotationStatic) this.AngularVelocity = magnitude;
        }
    }
}
