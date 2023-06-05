using System;
using System.Collections.Generic;
using FlatEngine;
using FlatEngine.Util;
using FlatEngine.Graphics;
using FlatPhysics.Constants;
using Microsoft.Xna.Framework;
using FlatEngine.Misc;

namespace FlatPhysics
{
    public sealed class World
    {
        public const int MinIterations = 1;
        public const int MaxIterations = 64;
        private Color? worldColor;

        private readonly List<FlatEntity> entityList;
        private FlatVector gravity;

        public readonly double Width;
        public readonly double Height;
        public readonly double HalfWidth;
        public readonly double HalfHeight;
        public bool HasGround;
        public int BodyCount { get { return this.entityList.Count; } }
        public FlatVector Dim { get { return new FlatVector(this.Width, this.Height); } }
        public FlatVector HalfDim { get { return new FlatVector(this.HalfWidth, this.HalfHeight); } }


        public World(double width, double height, Color? worldColor)
        {
            this.entityList = new List<FlatEntity>();
            this.gravity = FlatVector.Zero;
            this.worldColor = worldColor;
            this.Width = width;
            this.Height = height;
            this.HalfWidth = FlatMath.Div2(width);
            this.HalfHeight = FlatMath.Div2(height);
            this.WallWorldBorder(0.5, 0.5, 0.5);
        }
        public void SetGround(double gravity, double thickness, double restitution, double frictionStatic, double frictionDynamic,
             Color? groundColor, Color? lineColor, double lineThickness)
        {
            const double padding = 2;
            double totalThickness = thickness + padding;
            this.gravity = new FlatVector(0, -gravity);
            Body ground = new BodyType.Rectangle(this.Width + FlatMath.Mul2(padding), totalThickness, restitution, frictionStatic, frictionDynamic);
            ground.SetColor(groundColor);
            ground.SetOutline(lineColor, lineThickness);
            ground.MoveTo(0, thickness - FlatMath.Div2(totalThickness) - this.HalfHeight);
            entityList.Add(ground);
        }
        public void Update(double time, int iterationCount)
        {
            iterationCount = FlatTools.Clamp(iterationCount, MinIterations, MaxIterations);
            for (int i = 0; i < iterationCount; i++) this.UpdateEntities(time / iterationCount);
        }
        private void UpdateCollision(Body bodyA, int i)
        {
            for (int j = i + 1; j < this.entityList.Count; j++)
            {
                Body bodyB = World.GetBody(this.entityList[j]);

                if (bodyA.isStatic && bodyB.isStatic) continue;
                if (!bodyA.AABB.IsColliding(bodyB.AABB)) continue;

                if (Collision.IsColliding(bodyA, bodyB, out double depth, out FlatVector normal))
                    Collision.Resolve(bodyA, bodyB, depth, normal);
            }
        }
        private void UpdateEntities(double time)
        {
            for (int i = 0; i < this.entityList.Count; i++)
            {
                FlatEntity entity = this.entityList[i];
                entity.Update(time);
                entity.LinearAccelerate(this.gravity * time);
                this.VoidWorldBorder(entity);
                this.UpdateCollision(World.GetBody(entity), i);
            }
        }



        public void Draw(Sprites sprites, Shapes shapes, Camera camera, bool drawHitBox = false)
        {
            this.DrawWorldBox(shapes);
            this.DrawEntities(sprites, shapes, camera, drawHitBox);
        }
        private void DrawWorldBox(Shapes shapes)
        {
            if (this.worldColor != null) shapes.DrawRectangleFill(-this.HalfWidth, -this.HalfHeight, this.Width, this.Height, this.worldColor.Value);
        }
        private void DrawEntities(Sprites sprites, Shapes shapes, Camera camera, bool drawHitBox)
        {
            for (int i = 0; i < this.entityList.Count; i++)
            {
                FlatEntity obj = this.entityList[i];
                if (obj is Body body) body.Draw(shapes, camera);
                else if (obj is Sprite sprite)
                {
                    if (drawHitBox) sprite.Draw(sprites, shapes, camera);
                    else sprite.Draw(sprites, camera);
                }
                else throw new Exception();
            }
        }



        public void AddEntity(FlatEntity entity)
        {
            this.entityList.Add(entity);
        }
        public bool RemoveEntity(FlatEntity entity)
        {
            return this.entityList.Remove(entity);
        }
        public bool GetBody(int i, out Body entity)
        {
            entity = null;
            if (Math.Abs(i) >= this.entityList.Count) return false;
            if (i < 0) i += this.entityList.Count;
            entity = World.GetBody(this.entityList[i]);
            return true;
        }
        private static Body GetBody(FlatEntity entity)
        {
            if (entity is Sprite sprite) return sprite.HitBox;
            else return (Body)entity;
        }



        public bool WorldContains(FlatAABB boundingBox)
        {
            return boundingBox.IsInBoundary(HalfWidth, HalfHeight, FlatVector.Zero);
        }
        public bool WorldContains(FlatVector v)
        {
            return v.IsInBoundary(HalfWidth, HalfHeight, FlatVector.Zero);
        }



        private void VoidWorldBorder(FlatEntity obj)
        {
            if (obj.isStatic || this.WorldContains(obj.AABB)) return;
            this.RemoveEntity(obj);
        }
        private void WallWorldBorder(double restitution, double frictionStatic, double frictionDynamic)
        {
            const double thickness = 32;
            double halfThickness = FlatMath.Div2(thickness);
            double width = this.Width + FlatMath.Mul2(thickness);
            double height = this.Height + FlatMath.Mul2(thickness);

            Body top = new BodyType.Rectangle(width, thickness, restitution, frictionStatic, frictionDynamic);
            Body left = new BodyType.Rectangle(thickness, height, restitution, frictionStatic, frictionDynamic);
            Body right = new BodyType.Rectangle(thickness, height, restitution, frictionStatic, frictionDynamic);
            Body bottom = new BodyType.Rectangle(width, thickness, restitution, frictionStatic, frictionDynamic);
            top.MoveTo(0, this.HalfHeight + halfThickness);
            left.MoveTo(-this.HalfWidth - halfThickness, 0);
            right.MoveTo(this.HalfWidth + halfThickness, 0);
            bottom.MoveTo(0, -this.HalfHeight - halfThickness);
            this.entityList.Add(top);
            this.entityList.Add(left);
            this.entityList.Add(right);
            this.entityList.Add(bottom);
        }
    }
}
