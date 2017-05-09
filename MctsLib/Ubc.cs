using System;

namespace MctsLib
{
	public static class Ubc
	{
		public static double Margin(INode node, double explorationConstant)
		{
			return explorationConstant * Math.Sqrt(Math.Log(node.Parent.TotalPlays) / node.TotalPlays);
		}
	}
}