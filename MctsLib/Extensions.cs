using System;
using System.Collections.Generic;
using System.Linq;

namespace MctsLib
{
	public static class Extensions
	{
		public static T ChooseRandom<T>(this IEnumerable<T> items, Random random)
		{
			if (!(items is ICollection<T> col))
				col = items.ToList();
			if (col.Count == 0) return default(T);
			var index = random.Next(col.Count);
			switch (col)
			{
				case IList<T> list: return list[index];
				case IReadOnlyList<T> list: return list[index];
				default: return col.ElementAt(index);
			}
		}

		public static IList<T> AsList<T>(this IEnumerable<T> items)
		{
			if (items is IList<T> res) return res;
			return items.ToList();
		}

		public static T SelectBest<T>(this IEnumerable<T> items, Func<T, double> getKey, Random random)
		{
			return items.MaxBy(getKey).ChooseRandom(random);
		}

		public static T SelectWithWeights<T>(this IReadOnlyCollection<T> items, Func<T, double> getKey, Random random)
		{
			var totalSum = items.Sum(getKey);
			var dice = random.NextDouble() * totalSum;
			var sum = 0.0;
			T lastItem = default(T);
			foreach (var item in items)
			{
				sum += getKey(lastItem = item);
				if (sum >= dice) return item;
			}
			return lastItem;
		}

		public static IList<T> MaxBy<T>(this IEnumerable<T> items, Func<T, double> getKey)
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
	}
}