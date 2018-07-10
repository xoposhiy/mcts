using System;
using System.Collections.Generic;
using System.Linq;

namespace lib
{
    public static class Extensions
    {
        public static int IndexOf<T>(this IEnumerable<T> items, Func<T, bool> predicate)
        {
            var i = 0;
            foreach (var item in items)
            {
                if (predicate(item)) return i;
                i++;
            }
            return -1;
        }

        public static T MinBy<T>(this IEnumerable<T> items, Func<T, IComparable> getKey)
        {
            var best = default(T);
            IComparable bestKey = null;
            var found = false;
            foreach (var item in items)
                if (!found || getKey(item).CompareTo(bestKey) < 0)
                {
                    best = item;
                    bestKey = getKey(best);
                    found = true;
                }
            return best;
        }

        public static double MaxOrDefault<T>(this IEnumerable<T> items, Func<T, double> getCost, double defaultValue)
        {
            var bestCost = double.NegativeInfinity;
            foreach (var item in items)
            {
                var cost = getCost(item);
                if (cost > bestCost)
                    bestCost = cost;
            }
            return double.IsNegativeInfinity(bestCost) ? defaultValue : bestCost;
        }

        public static T MaxBy<T>(this IEnumerable<T> items, Func<T, IComparable> getKey)
        {
            var best = default(T);
            IComparable bestKey = null;
            var found = false;
            foreach (var item in items)
                if (!found || getKey(item).CompareTo(bestKey) > 0)
                {
                    best = item;
                    bestKey = getKey(best);
                    found = true;
                }
            return best;
        }

        public static IList<T> AllMaxBy<T>(this IEnumerable<T> items, Func<T, double> getKey)
        {
            IList<T> result = null;
            double bestKey = double.MinValue;
            foreach (var item in items)
            {
                var itemKey = getKey(item);
                if (result == null || bestKey < itemKey)
                {
                    result = new List<T> { item };
                    bestKey = itemKey;
                }
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                else if (bestKey == itemKey)
                {
                    result.Add(item);
                }
            }
            return result ?? new T[0];
        }

        public static int BoundTo(this int v, int left, int right)
        {
            if (v < left) return left;
            if (v > right) return right;
            return v;
        }

        public static double BoundTo(this double v, double left, double right)
        {
            if (v < left) return left;
            if (v > right) return right;
            return v;
        }

        public static double TruncateAbs(this double v, double maxAbs)
        {
            if (v < -maxAbs) return -maxAbs;
            if (v > maxAbs) return maxAbs;
            return v;
        }

        public static int TruncateAbs(this int v, int maxAbs)
        {
            if (v < -maxAbs) return -maxAbs;
            if (v > maxAbs) return maxAbs;
            return v;
        }

        public static IEnumerable<T> Times<T>(this int count, Func<int, T> create)
        {
            return Enumerable.Range(0, count).Select(create);
        }

        public static IEnumerable<T> Times<T>(this int count, T item)
        {
            return Enumerable.Repeat(item, count);
        }

        public static bool InRange(this int v, int min, int max)
        {
            return v >= min && v <= max;
        }

        public static int IndexOf<T>(this IReadOnlyList<T> readOnlyList, T value)
        {
            var count = readOnlyList.Count;
            var equalityComparer = EqualityComparer<T>.Default;
            for (var i = 0; i < count; i++)
            {
                var current = readOnlyList[i];
                if (equalityComparer.Equals(current, value)) return i;
            }
            return -1;
        }

        public static TV GetOrCreate<TK, TV>(this IDictionary<TK, TV> d, TK key, Func<TK, TV> create)
        {
            TV v;
            if (d.TryGetValue(key, out v)) return v;
            return d[key] = create(key);
        }

        public static TV GetOrDefault<TK, TV>(this IDictionary<TK, TV> d, TK key, TV def = default(TV))
        {
            TV v;
            if (d.TryGetValue(key, out v)) return v;
            return def;
        }

        public static int ElementwiseHashcode<T>(this IEnumerable<T> items)
        {
            unchecked
            {
                return items.Select(t => t.GetHashCode()).Aggregate((res, next) => (res * 379) ^ next);
            }
        }

        public static List<T> Shuffle<T>(this IEnumerable<T> items, Random random)
        {
            var copy = items.ToList();
            for (var i = 0; i < copy.Count; i++)
            {
                var nextIndex = random.Next(i, copy.Count);
                var t = copy[nextIndex];
                copy[nextIndex] = copy[i];
                copy[i] = t;
            }
            return copy;
        }

        public static double NormAngleInRadians(this double angle)
        {
            while (angle > Math.PI) angle -= 2 * Math.PI;
            while (angle <= -Math.PI) angle += 2 * Math.PI;
            return angle;
        }

        public static double NormDistance(this double value, double worldDiameter)
        {
            return value / worldDiameter;
        }

        public static int ToInt(this string s)
        {
            return int.Parse(s);
        }

        public static string StrJoin<T>(this IEnumerable<T> items, string delimiter)
        {
            return string.Join(delimiter, items);
        }

        public static string StrJoin<T>(this IEnumerable<T> items, string delimiter, Func<T, string> toString)
        {
            return items.Select(toString).StrJoin(delimiter);
        }
    }

    public static class RandomExtensions
    {
        public static T GetRandomBest<T>(this IEnumerable<T> items, Func<T, double> getKey, Random random)
        {
            return items.AllMaxBy(getKey).ChooseRandom(random);
        }

        public static T ChooseRandom<T>(this IEnumerable<T> items, Random random)
        {
            if (!(items is ICollection<T> col))
                col = items.ToList();
            if (col.Count == 0) return default(T);
            var index = random.Next(col.Count);
            if (col is IList<T>) return ((IList<T>)col)[index];
            if (col is IReadOnlyList<T>) return ((IReadOnlyList<T>)col)[index];
            return col.ElementAt(index);
        }

        public static ulong NextUlong(this Random r)
        {
            var a = unchecked ((ulong) r.Next());
            var b = unchecked ((ulong) r.Next());
            return (a << 32) | b;
        }
        
        public static double NextDouble(this Random r, double min, double max)
        {
            return r.NextDouble() * (max - min) + min;
        }

        public static ulong[,,] CreateRandomTable(this Random r, int size1, int size2, int size3)
        {
            var res = new ulong[size1, size2, size3];
            for (var x = 0; x < size1; x++)
            for (var y = 0; y < size2; y++)
            for (var h = 0; h < size3; h++)
            {
                var value = r.NextUlong();
                res[x, y, h] = value;
            }
            return res;
        }

        public static ulong[,] CreateRandomTable(this Random r, int size1, int size2)
        {
            var res = new ulong[size1, size2];
            for (var x = 0; x < size1; x++)
            for (var y = 0; y < size2; y++)
            {
                var value = r.NextUlong();
                res[x, y] = value;
            }
            return res;
        }
    }
}