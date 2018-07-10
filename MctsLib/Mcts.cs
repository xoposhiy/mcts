using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

// ReSharper disable AccessToModifiedClosure

namespace lib
{
	public delegate double EstimateNode<TGame>(Node<TGame> node, TGame previousGame)
		where TGame : IGame<TGame>;

	public class Mcts<TGame> where TGame : IGame<TGame>
	{
		private readonly Random random;
		public EstimateNode<TGame> EstimateNodeForExpansion;
		public EstimateNode<TGame> EstimateNodeForFinalChoice;
		public EstimateNode<TGame> EstimateNodeForSelection;
		public Func<TGame, ICollection<IMove<TGame>>, IMove<TGame>> StrategyForSimulation;
		public double ExplorationConstant = 1.4;
		public Action<string> Log = s => { };

		public Mcts(Random random = null)
		{
			this.random = random ?? new Random();
			EstimateNodeForSelection =
				(n, b) => Ubc.Uct(n, b, ExplorationConstant);
			EstimateNodeForFinalChoice =
				(n, b) => n.GetExpectedScore(b.CurrentPlayer);
			EstimateNodeForExpansion = (n, b) => 0.0;
			StrategyForSimulation = (g, ms) => ms.ChooseRandom(this.random);
		}

		public Random Random => random;

		public IMove<TGame> GetBestMove(TGame game, Countdown countdown)
		{
			var root = BuildGameTree(game, countdown);
			return GetBestMove(game, root);
		}

		public IMove<TGame> GetBestMove(TGame game, Node<TGame> root)
		{
			var estimatedChildren = GetEstimatedChildren(game, root);
			LogMoveOptions(estimatedChildren);
			var best = estimatedChildren.GetRandomBest(n => n.Item2, random);
			return best.Item1.Move;
		}

		public Node<TGame> BuildGameTree(TGame game, Countdown countdown)
		{
			var root = new Node<TGame>(game.PlayersCount);
			var startTime = DateTime.Now;
			var simulationCount = 0;
			var maxDepth = 0;
			while (!countdown.IsFinished)
			{
				var gameCopy = game.MakeCopy();
				var newNode = GetExpandedNode(root, gameCopy);
				maxDepth = Math.Max(maxDepth, newNode.Depth);
				var scores = SimulateToEnd(gameCopy);
				BackpropagateScores(newNode, scores);
				simulationCount++;
				if (root.GetUnvisitedChildren(game).Count + root.GetChildren().Count <= 1) break;
			}

			var timeSpent = (DateTime.Now - startTime).TotalSeconds;
			Log(string.Join(", ",
				$"simulations count: {simulationCount}",
				$"depth: {maxDepth}",
				$"nodes: {root.GetNodesCount()}",
				$"time: {timeSpent.ToString("0.##", CultureInfo.InvariantCulture)} s",
				$"{simulationCount / timeSpent:#} sim/s"));
			return root;
		}

		private void LogMoveOptions(IEnumerable<Tuple<Node<TGame>, double>> estimatedOptions)
		{
			var text = string.Join("\n", estimatedOptions.Select(child => $"  * estimate: {child.Item2} {child.Item1}"));
			Log($"Options:\n{text}");
		}

		private List<Tuple<Node<TGame>, double>> GetEstimatedChildren(TGame game, Node<TGame> root)
		{
			return root
				.GetChildren()
				.Select(n => Tuple.Create(n, EstimateNodeForFinalChoice(n, game)))
				.OrderByDescending(n => n.Item2)
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
					.GetRandomBest(
						childNode => EstimateNodeForSelection(childNode, game),
						random
					);
				node.Move.ApplyTo(game);
			}
			return Expand(node, game);
		}

		private Node<TGame> Expand(Node<TGame> node, TGame game)
		{
			var child = node.GetUnvisitedChildren(game)
				.GetRandomBest(n => EstimateNodeForExpansion(n, game), random);
			node.MakeVisited(child);
			child.Move.ApplyTo(game);
			return child;
		}

		private double[] SimulateToEnd(TGame game)
		{
			while (true)
			{
				var possibleMoves = game.GetPossibleMoves();
				if (!possibleMoves.Any()) break;
				var move = StrategyForSimulation(game, possibleMoves);
				move.ApplyTo(game);
			}
			return game.GetScores();
		}
	}
}