using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace lib
{
	public interface INode
	{
		INode Parent { get; }
		int TotalPlays { get; }
	}

	public class Node<TGame> : INode where TGame : IGame<TGame>
	{
		public readonly int Depth;
		public readonly Node<TGame> Parent;
		private readonly double[] totalScore;
		private List<Node<TGame>> children;
		private List<Node<TGame>> unvisitedChildren;

		public int GetNodesCount()
		{
			return 1 + children?.Aggregate(0, (n, node) => n + node.GetNodesCount()) ?? 0;
		}

		public Node(int playersCount)
			: this(null, playersCount, null)
		{
		}

		private Node(IMove<TGame> move, int playersCount, Node<TGame> parent)
		{
			totalScore = new double[playersCount];
			Move = move;
			Parent = parent;
			Depth = (parent?.Depth ?? 0) + 1;
		}

		private List<Node<TGame>> Children => children ?? (children = new List<Node<TGame>>());

		public IMove<TGame> Move { get; }
		INode INode.Parent => Parent;

		public int TotalPlays { get; private set; }

		public override string ToString()
		{
			var scores = string.Join(
				",",
				totalScore.Select(s => s.ToString(NumberFormatInfo.InvariantInfo)));
			return $"{Move} plays:{TotalPlays} scores:{scores}";
		}

		public IReadOnlyList<Node<TGame>> GetUnvisitedChildren(TGame game)
		{
			return unvisitedChildren
				   ?? (unvisitedChildren = CreateUnvisitedChildren(game));
		}

		private List<Node<TGame>> CreateUnvisitedChildren(TGame game)
		{
			return game.GetPossibleMoves()
				.Select(m => new Node<TGame>(m, totalScore.Length, this))
				.ToList();
		}

		public IReadOnlyList<Node<TGame>> GetChildren()
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

		public void MakeVisited(Node<TGame> child)
		{
			if (!unvisitedChildren.Remove(child))
				throw new InvalidOperationException();
			Children.Add(child);
		}
	}
}