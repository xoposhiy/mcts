using System;

namespace lib
{
	public static class Ubc
	{
		public static double Uct<TGame>(Node<TGame> node, IGame<TGame> game, double explorationConstant) where TGame : IGame<TGame>
		{
			return node.GetExpectedScore(game.CurrentPlayer) + Margin(node, explorationConstant);
		}

		public static double Margin(INode node, double explorationConstant)
		{
			return explorationConstant * Math.Sqrt(Math.Log(node.Parent.TotalPlays) / node.TotalPlays);
		}
	}
}