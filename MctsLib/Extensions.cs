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

		public static T SelectOne<T>(this IEnumerable<T> items, Func<T, IComparable> getKey, Random random)
		{
			if (getKey == null) return items.ChooseRandom(random);
			return items.MaxBy(getKey).ChooseRandom(random);
		}

		public static List<T> MaxBy<T>(this IEnumerable<T> items, Func<T, IComparable> getKey)
		{
			List<T> result = null;
			IComparable bestKey = null;
			foreach (var item in items)
			{
				var itemKey = getKey(item);
				if (result == null || bestKey.CompareTo(itemKey) < 0)
				{
					result = new List<T> { item };
					bestKey = itemKey;
				}
				else if (bestKey.CompareTo(itemKey) == 0)
				{
					result.Add(item);
				}
			}
			return result ?? new List<T>();
		}
	}
}