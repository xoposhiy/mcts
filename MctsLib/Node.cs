using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace MctsLib
{
	public class Node<TBoard> : INode where TBoard : IBoard<TBoard>
	{
		public readonly Node<TBoard> Parent;
		private readonly double[] totalScore;
		private List<Node<TBoard>> children;
		private List<Node<TBoard>> unvisitedChildren;

		public Node(int playersCount)
			: this(null, playersCount, null)
		{
		}

		private Node(IMove<TBoard> move, int playersCount, Node<TBoard> parent)
		{
			totalScore = new double[playersCount];
			Move = move;
			Parent = parent;
		}

		private List<Node<TBoard>> Children => children ?? (children = new List<Node<TBoard>>());

		public IMove<TBoard> Move { get; }
		INode INode.Parent => Parent;

		public int TotalPlays { get; private set; }

		public override string ToString()
		{
			var scores = string.Join(
				",",
				totalScore.Select(s => s.ToString(NumberFormatInfo.InvariantInfo)));
			return $"{Move} plays:{TotalPlays} scores:{scores}";
		}

		public IReadOnlyList<Node<TBoard>> GetUnvisitedChildren(TBoard board)
		{
			return unvisitedChildren
				   ?? (unvisitedChildren = CreateUnvisitedChildren(board));
		}

		private List<Node<TBoard>> CreateUnvisitedChildren(TBoard board)
		{
			return board.GetPossibleMoves()
				.Select(m => new Node<TBoard>(m, totalScore.Length, this))
				.ToList();
		}

		public IReadOnlyList<Node<TBoard>> GetChildren()
		{
			return Children;
		}

		public double GetExpectedScore(int player)
		{
			return totalScore[player] / TotalPlays;
		}

		public void RegisterPlay(double[] scores)
		{
			TotalPlays++;
			for (var i = 0; i < scores.Length; i++)
				totalScore[i] += scores[i];
		}

		public void MakeVisited(Node<TBoard> child)
		{
			if (!unvisitedChildren.Remove(child))
				throw new InvalidOperationException();
			Children.Add(child);
		}
	}
}