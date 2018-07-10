using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.CompilerServices;

namespace lib
{
    public class Vec : IEquatable<Vec>, IFormattable
    {
        public readonly int X, Y;

        public Vec(int x, int y)
        {
            X = x;
            Y = y;
        }

        public Vec(double x, double y)
            : this(checked((int) Math.Round(x)), checked((int) Math.Round(y)))
        {
        }

        public int this[int dimension] => dimension == 0 ? X : Y;

        [Pure]
        public bool Equals(Vec other)
        {
            return X == other.X && Y == other.Y;
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            return $"{X.ToString(format, formatProvider)} {Y.ToString(format, formatProvider)}";
        }

        public static Vec FromPolar(double len, double angle)
        {
            return new Vec(len * Math.Cos(angle), len * Math.Sin(angle));
        }

        public static Vec Parse(string s)
        {
            var parts = s.Split();
            if (parts.Length != 2) throw new FormatException(s);
            return new Vec(int.Parse(parts[0]), int.Parse(parts[1]));
        }

        public static IEnumerable<Vec> Area(int size)
        {
            return Area(size, size);
        }

        public static IEnumerable<Vec> Area(int width, int height)
        {
            for (var x = 0; x < width; x++)
            for (var y = 0; y < height; y++)
                yield return new Vec(x, y);
        }

        public bool InArea(int size)
        {
            return X >= 0 && X < size && Y >= 0 && Y < size;
        }

        public bool InArea(int w, int h)
        {
            return X.InRange(0, w - 1) && Y.InRange(0, h - 1);
        }

        public Vec BoundTo(int minX, int minY, int maxX, int maxY)
        {
            return new Vec(X.BoundTo(minX, maxX), Y.BoundTo(minY, maxY));
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Vec && Equals((Vec) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (X.GetHashCode() * 397) ^ Y.GetHashCode();
            }
        }

        public override string ToString()
        {
            return $"{X} {Y}";
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool InRadiusTo(Vec b, double radius)
        {
            return SquaredDistTo(b) <= radius * radius;
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double DistTo(Vec b)
        {
            return (b - this).Length();
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int SquaredDistTo(Vec b)
        {
            var dx = X - b.X;
            var dy = Y - b.Y;
            return dx * dx + dy * dy;
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double Length()
        {
            return Math.Sqrt(X * X + Y * Y);
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int LengthSquared()
        {
            return X * X + Y * Y;
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vec operator -(Vec a, Vec b)
        {
            return new Vec(a.X - b.X, a.Y - b.Y);
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vec operator -(Vec a)
        {
            return new Vec(-a.X, -a.Y);
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vec operator +(Vec v, Vec b)
        {
            return new Vec(v.X + b.X, v.Y + b.Y);
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vec operator *(Vec a, int k)
        {
            return new Vec(a.X * k, a.Y * k);
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vec operator /(Vec a, int k)
        {
            return new Vec(a.X / k, a.Y / k);
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vec operator *(int k, Vec a)
        {
            return new Vec(a.X * k, a.Y * k);
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vec operator *(double k, Vec a)
        {
            return new Vec(a.X * k, a.Y * k);
        }



        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int ScalarProd(Vec p2)
        {
            return X * p2.X + Y * p2.Y;
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int VectorProdLength(Vec p2)
        {
            return X * p2.Y - p2.X * Y;
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vec Translate(int shiftX, int shiftY)
        {
            return new Vec(X + shiftX, Y + shiftY);
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vec MoveTowards(Vec target, double distance)
        {
            if (distance == 0.0 || target == this) return this;
            var d = target - this;
            var difLen = d.Length();
            if (difLen < distance) return target;
            var k = distance / difLen;
            return new Vec(X + k * d.X, Y + k * d.Y);
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vec Rotate90CW()
        {
            return new Vec(Y, -X);
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vec Rotate90CCW()
        {
            return new Vec(-Y, X);
        }

        public Vec Resize(double newLen)
        {
            return newLen / Length() * this;
        }

        /// <returns>angle in (-Pi..Pi]</returns>
        public double GetAngle()
        {
            return Math.Atan2(Y, X);
        }
    }
}