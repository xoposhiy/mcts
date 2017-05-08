using System;
using System.Collections.Generic;

namespace MctsLib
{
	internal class Node
	{
		private IBoard board;
		private List<Node> children = new List<Node>();
		private double[] totalScore;
		public double TotalPlays { get; private set; }

		public Node(IBoard board)
		{
			this.board = board;
			totalScore = new double[board.PlayersCount];
		}

		public IMove Move { get; private set; }

		public IEnumerable<Node> GetChildren()
		{
			return children;
		}

		public double GetExpectedScore(int player)
		{
			return totalScore[player]/TotalPlays;
		}
	}
}