using System;
using System.Collections.Generic;
using System.Linq;

// ReSharper disable AccessToModifiedClosure

namespace MctsLib
{
	public delegate double EstimateNode<TGame>(Node<TGame> node, TGame previousGame) 
		where TGame : IGame<TGame>;
	public delegate double EstimateMove<TGame>(IMove<TGame> move, TGame previousGame) 
		where TGame : IGame<TGame>;

	public class Mcts<TGame> where TGame : IGame<TGame>
	{
		public Mcts()
		{
			EstimateNodeForSelection =
				(n, b) => n.GetExpectedScore(b.CurrentPlayer) + Ubc.Margin(n, ExplorationConstant);
		}

		public EstimateNode<TGame> EstimateNodeForExpansion = (n, b) => 0.0;
		public EstimateNode<TGame> EstimateNodeForFinalChoice = (n, b) => n.GetExpectedScore(b.CurrentPlayer);
		public EstimateNode<TGame> EstimateNodeForSelection;
		public double ExplorationConstant = 1.4;
		public EstimateMove<TGame> EstimateNodeForSimulation = (move, b) => 0.0;
		public Action<string> Log = s => { };
		public int MaxSimulationsCount = 1000;
		public TimeSpan MaxTime = TimeSpan.FromMilliseconds(100);
		public Random Random = new Random();

		public IMove<TGame> GetBestMove(TGame game)
		{
			var root = BuildGameTree(game);
			return GetBestMove(game, root);
		}

		private IMove<TGame> GetBestMove(TGame game, Node<TGame> root)
		{
			var estimatedChildren = GetEstimatedChildren(game, root);
			LogMoveOptions(estimatedChildren);
			var best = estimatedChildren.SelectOne(n => n.estimate, Random);
			return best.child.Move;
		}

		private Node<TGame> BuildGameTree(TGame game)
		{
			var root = new Node<TGame>(game.PlayersCount);
			var startTime = DateTime.Now;
			var simulationCount = 0;
			var maxDepth = 0;
			while (DateTime.Now - startTime < MaxTime
				   && simulationCount < MaxSimulationsCount)
			{
				var gameCopy = game.MakeCopy();
				var newNode = GetExpandedNode(root, gameCopy);
				maxDepth = Math.Max(maxDepth, newNode.Depth);
				var scores = SimulateToEnd(gameCopy);
				BackpropagateScores(newNode, scores);
				simulationCount++;
			}

			Log($"simulations count: {simulationCount}, depth: {maxDepth}, nodes: {root.GetNodesCount()}");
			return root;
		}

		private void LogMoveOptions(IEnumerable<(Node<TGame> node, double estimate)> estimatedOptions)
		{
			string Format((Node<TGame> node, double estimate) child) =>
				$"  * estimate: {child.estimate} {child.node}";

			var text = string.Join("\n", estimatedOptions.Select(Format));
			Log($"Options:\n{text}");
		}

		private List<(Node<TGame> child, double estimate)> GetEstimatedChildren(TGame game, Node<TGame> root)
		{
			return root
				.GetChildren()
				.Select(n => (child: n, estimate: EstimateNodeForFinalChoice(n, game)))
				.OrderByDescending(n => n.estimate)
				.ToList();
		}

		private void BackpropagateScores(Node<TGame> node, double[] scores)
		{
			while (node != null)
			{
				node.RegisterPlay(scores);
				node = node.Parent;
			}
		}

		private Node<TGame> GetExpandedNode(Node<TGame> node, TGame game)
		{
			while (node.GetUnvisitedChildren(game).Count == 0)
			{
				var children = node.GetChildren();
				if (children.Count == 0) return node;
				node = children
					.SelectOne(
						childNode => EstimateNodeForSelection(childNode, game),
						Random
					);
				node.Move.ApplyTo(game);
			}
			return Expand(node, game);
		}

		private Node<TGame> Expand(Node<TGame> node, TGame game)
		{
			var child = node.GetUnvisitedChildren(game)
				.SelectOne(n => EstimateNodeForExpansion(n, game), Random);
			node.MakeVisited(child);
			child.Move.ApplyTo(game);
			return child;
		}

		private double[] SimulateToEnd(TGame game)
		{
			while (true)
			{
				var possibleMoves = game.GetPossibleMoves().ToList();
				if (!possibleMoves.Any()) break;
				var move = possibleMoves.SelectOne(
					m => EstimateNodeForSimulation(m, game),
					Random);
				move.ApplyTo(game);
			}
			return game.GetScores();
		}
	}
}