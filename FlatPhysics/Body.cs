using System;
using FlatEngine;
using FlatEngine.Graphics;
using FlatEngine.Util;
using FlatPhysics.BodyType;
using Microsoft.Xna.Framework;

namespace FlatPhysics
{
    public abstract class Body : FlatEntity
    {
        public Color? bodyColor = null;
        public Color? lineColor = null;
        public double lineThickness = 0;

        protected FlatVector linearForce = FlatVector.Zero;
        protected double angularForce = 0;

        public readonly double Restitution;
        public readonly double FrictionStatic;
        public readonly double FrictionDynamic;
        public readonly double Mass;
        public readonly double InvMass;
        protected readonly double inertia;
        public readonly double InvInertia;


        /// <summary>Fully Static Body</summary>
        protected Body(double restitution, double frictionStatic, double frictionDynamic) :
            base(true, true)
        {
            this.Restitution = FlatTools.Clamp(restitution, 0, 1);
            this.FrictionStatic = FlatTools.Clamp(frictionStatic, 0, 1);
            this.FrictionDynamic = FlatTools.Clamp(frictionDynamic, 0, 1);
            this.Mass = this.InvMass = this.inertia = this.InvInertia = 0;
        }
        /// <summary>Static Rotation Body</summary>
        protected Body(double restitution, double frictionStatic, double frictionDynamic, double mass) :
            base(false, true)
        {
            this.Restitution = FlatTools.Clamp(restitution, 0, 1);
            this.FrictionStatic = FlatTools.Clamp(frictionStatic, 0, 1);
            this.FrictionDynamic = FlatTools.Clamp(frictionDynamic, 0, 1);
            this.Mass = mass;
            this.InvMass = 1 / mass;
            this.inertia = this.InvInertia = 0;
        }
        /// <summary>Dynamic Body</summary>
        protected Body(double restitution, double frictionStatic, double frictionDynamic, double mass, double inertia, bool isPositionStatic) : 
            base(isPositionStatic, false)
        {
            this.Restitution = FlatTools.Clamp(restitution, 0, 1);
            this.FrictionStatic = FlatTools.Clamp(frictionStatic, 0, 1);
            this.FrictionDynamic = FlatTools.Clamp(frictionDynamic, 0, 1);
            this.Mass = mass;
            this.InvMass = 1 / mass;
            this.inertia = inertia;
            this.InvInertia = 1 / inertia;
        }



        public override void Update(double time)
        {
            if (this.isStatic) return;
            double timeDivMass = this.InvMass * time;

            this.LinearVelocity += this.linearForce * timeDivMass;
            this.AngularVelocity += this.angularForce * timeDivMass;
            this.linearForce = FlatVector.Zero;
            this.angularForce = 0;
            this.Position += this.LinearVelocity * time;
            this.Rotation += this.AngularVelocity * time;
        }
        internal virtual void Draw(Shapes shapes, Camera camera)
        {
            if (!camera.ViewportContains(this.AABB)) return;
            if (this.bodyColor is not null) this.DrawBody(shapes);
            if (this.lineThickness != 0 && this.lineColor != null) this.DrawOutline(shapes);
        }
        protected abstract void DrawBody(Shapes shapes);
        protected abstract void DrawOutline(Shapes shapes);



        public void SetColor(Color? bodyColor)
        {
            this.bodyColor = bodyColor;
        }
        public void SetOutline(Color? lineColor, double lineThickness)
        {
            this.lineColor = lineColor;
            this.lineThickness = lineThickness;
        }
        public virtual void ApplyLinearForce(FlatVector magnitude)
        {
            if (!this.isPositionStatic) this.linearForce = magnitude;
        }
        public virtual void ApplyAngularForce(double magnitude)
        {
            if (!this.isRotationStatic) this.angularForce = magnitude;
        }



        public static Body TennisBall(out double radius)
        {
            radius = 0.0343;
            return new Circle(0.0343, 0.82, 0.6, 0.6, 0.056, false);
        }
        public static Body BasketBall(out double radius)
        {
            radius = 0.1194;
            return new Circle(radius, 0.75, 0.41, 0.41, 0.624, false);
        }
    }
}
