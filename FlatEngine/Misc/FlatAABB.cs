using FlatEngine.Graphics;
using Microsoft.Xna.Framework;

namespace FlatEngine.Misc
{
    public readonly struct FlatAABB
    {
        public readonly FlatVector Min;
        public readonly FlatVector Max;

        public FlatAABB(FlatVector min, FlatVector max)
        {
            Min = min;
            Max = max;
        }
        public FlatAABB(double minX, double minY, double maxX, double maxY)
        {
            Min = new FlatVector(minX, minY);
            Max = new FlatVector(maxX, maxY);
        }



        public bool IsColliding(FlatAABB other)
        {
            return
                 Max.X >= other.Min.X && other.Max.X >= Min.X &&
                 Max.Y >= other.Min.Y && other.Max.Y >= Min.Y;
        }

        public bool IsInBoundary(double halfWidth, double halfHeight, FlatVector position)
        {
            return
                Max.X > position.X - halfWidth && Max.Y > position.Y - halfHeight &&
                Min.X < position.X + halfWidth && Min.Y < position.Y + halfHeight;
        }
        public bool IsInBoundary(FlatVector minBound, FlatVector maxBound)
        {
            return Max > minBound && Min < maxBound;
        }
        public void Draw(Shapes shapes, Color color)
        {
            shapes.DrawRectangle(Min, Max.X - Min.X, Max.Y - Min.Y, 0.1, color);
        }
    }
}
