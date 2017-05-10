using System;
using System.Collections.Generic;
using System.Linq;

namespace MctsLib
{
	public static class Extensions
	{
		public static T ChooseRandom<T>(this IEnumerable<T> items, Random random)
		{
			var list = items.AsList();
			if (list.Count == 0) return default(T);
			return list[random.Next(list.Count)];
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

		public static T SelectWithWeights<T>(this IEnumerable<T> items, Func<T, double> getKey, Random random)
		{
			var estimated = items.Select(item => (item:item, estimate:getKey(item))).OrderByDescending(t => t.estimate).ToList();
			var totalSum = estimated.Sum(t => t.estimate);
			var dice = random.NextDouble() * totalSum;
			var sum = 0.0;
			foreach (var item in estimated)
			{
				sum += item.estimate;
				if (sum > dice) return item.item;
			}
			return estimated.Last().item;
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