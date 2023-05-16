using System;
using System.Runtime.CompilerServices;
using FlatEngine.Util;
using Microsoft.Xna.Framework;

namespace FlatEngine;
public readonly struct FlatVector
{
    public readonly double X;
    public readonly double Y;
    public readonly FlatVector Normal { get { return new FlatVector(-this.Y, this.X); } }

    public static readonly FlatVector Zero = new FlatVector(0, 0);
    public static readonly FlatVector One = new FlatVector(1, 1);
    public static readonly FlatVector Down = new FlatVector(0, -1);
    public static readonly FlatVector Up = new FlatVector(0, 1);
    public static readonly FlatVector Left = new FlatVector(-1, 0);
    public static readonly FlatVector Right = new FlatVector(1, 0);

    public FlatVector(double x, double y)
    {
        this.X = x;
        this.Y = y;
    }



    /// <summary>
    /// Vector Addition
    /// </summary>
    public static FlatVector operator +(FlatVector a, FlatVector b)
    {
        return new FlatVector(a.X + b.X, a.Y + b.Y);
    }
    /// <summary>
    /// Vector Subtraction
    /// </summary>
    public static FlatVector operator -(FlatVector a, FlatVector b)
    {
        return new FlatVector(a.X - b.X, a.Y - b.Y);
    }
    /// <summary>
    /// Vector Scaling
    /// </summary>
    public static FlatVector operator *(FlatVector a, double b)
    {
        return new FlatVector(a.X * b, a.Y * b);
    }
    /// <summary>
    /// Vector Scaling
    /// </summary>
    public static FlatVector operator *(double a, FlatVector b)
    {
        return new FlatVector(a * b.X, a * b.Y);
    }
    /// <summary>
    /// Dot Product
    /// </summary>
    public static double operator *(FlatVector a, FlatVector b)
    {
        return Math.FusedMultiplyAdd(a.X, b.X, a.Y * b.Y);
    }
    /// <summary>
    /// Cross Product
    /// </summary>
    public static double operator /(FlatVector a, FlatVector b)
    {
        return Math.FusedMultiplyAdd(a.X, b.Y, -a.Y * b.X);
    }
    /// <summary>
    /// Vector Inverse Scaling
    /// </summary>
    public static FlatVector operator /(FlatVector a, double b)
    {
        return new FlatVector(a.X / b, a.Y / b);
    }
    /// <summary>
    /// Vector Inversion</summary>
    public static FlatVector operator -(FlatVector a)
    {
        return new FlatVector(-a.X, -a.Y);
    }


    /// <summary>Almost Equal</summary>
    public static bool operator %(FlatVector a, FlatVector b)
    {
        return FlatVector.DistanceSquared(a, b) < FlatMath.MinAccuracy * FlatMath.MinAccuracy;
    }
    public static bool operator ==(FlatVector a, FlatVector b)
    {
        return (a.X == b.X) && (a.Y == b.Y);
    }
    public static bool operator !=(FlatVector a, FlatVector b)
    {
        return (a.X != b.X) || (a.Y != b.Y);
    }
    public static bool operator <(FlatVector a, FlatVector b)
    {
        return (a.X < b.X) && (a.Y < b.Y);
    }
    public static bool operator <=(FlatVector a, FlatVector b)
    {
        return (a.X <= b.X) && (a.Y <= b.Y);
    }
    public static bool operator >(FlatVector a, FlatVector b)
    {
        return (a.X > b.X) && (a.Y > b.Y);
    }
    public static bool operator >=(FlatVector a, FlatVector b)
    {
        return (a.X >= b.X) && (a.Y >= b.Y);
    }



    public FlatVector Transform(FlatTransform t)
    {
        return new FlatVector(
            Math.FusedMultiplyAdd(this.X, t.CosScale.X, -this.Y * t.SinScale.Y) + t.Position.X,
            Math.FusedMultiplyAdd(this.X, t.SinScale.X, this.Y * t.CosScale.Y) + t.Position.Y);
    }
    public FlatVector Project(FlatVector p)
    {
        double projDist = (p * this) / this.LengthSquared();
        if (projDist <= 0) return FlatVector.Zero;
        else if (projDist >= 1) return this;
        else return this * projDist;
    }
    public static FlatVector Project(FlatVector p, FlatVector a, FlatVector b)
    {
        FlatVector AB = b - a;
        FlatVector AP = p - a;
        double projDist = (AP * AB) / AB.LengthSquared();
        if (projDist <= 0) return a;
        else if (projDist >= 1) return b;
        else return a + AB * projDist;
    }
    public FlatVector Unit(double length)
    {
        return this / length;
    }
    public FlatVector Unit()
    {
        return this / this.Length();
    }
    public double Length()
    {
        return Math.Sqrt(this.LengthSquared());
    }
    public double LengthSquared()
    {
        return Math.FusedMultiplyAdd(this.X, this.X, this.Y * this.Y);
    }
    public static double Distance(FlatVector a, FlatVector b)
    {
        return Math.Sqrt(FlatVector.DistanceSquared(a, b));
    }
    public static double DistanceSquared(FlatVector a, FlatVector b)
    {
        double dx = a.X - b.X;
        double dy = a.Y - b.Y;
        return Math.FusedMultiplyAdd(dx, dx, dy * dy);
    }


    public bool IsInBoundary(double halfWidth, double halfHeight, FlatVector position)
    {
        return
            this.X > position.X - halfWidth && this.Y > position.Y - halfHeight &&
            this.X < position.X + halfWidth && this.Y < position.Y + halfHeight;
    }
    public bool IsInBoundary(FlatVector minBound, FlatVector maxBound)
    {
        return this > minBound && this < maxBound;
    }



    public override bool Equals(Object obj)
    {
        if (obj is FlatVector other) return this == other;
        return false;
    }
    public override int GetHashCode()
    {
        return new { this.X, this.Y }.GetHashCode();
    }
    public override string ToString()
    {
        return $"({this.X}, {this.Y})";
    }
    [method: MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector2 ToXna()
    {
        return new Vector2((float)this.X, (float)this.Y);
    }
}
