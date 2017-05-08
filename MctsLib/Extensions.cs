using System;
using System.Collections.Generic;

namespace MctsLib
{
	public static class Extensions
	{
		public static T ChooseRandom<T>(this IList<T> items, Random random)
		{
			return items[random.Next(items.Count)];
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