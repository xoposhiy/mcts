using System;
using System.Collections.Generic;
using System.Linq;

// ReSharper disable AccessToModifiedClosure

namespace MctsLib
{
	public delegate double EstimateNode<TBoard>(Node<TBoard> node, TBoard previousBoard) 
		where TBoard : IBoard<TBoard>;
	public delegate double EstimateMove<TBoard>(IMove<TBoard> move, TBoard previousBoard) 
		where TBoard : IBoard<TBoard>;

	public class Mcts<TBoard> where TBoard : IBoard<TBoard>
	{
		public Mcts()
		{
			EstimateNodeForSelection =
				(n, b) => n.GetExpectedScore(b.CurrentPlayer) + Ubc.Margin(n, ExplorationConstant);
		}

		public EstimateNode<TBoard> EstimateNodeForExpansion = (n, b) => 0.0;
		public EstimateNode<TBoard> EstimateNodeForFinalChoice = (n, b) => n.GetExpectedScore(b.CurrentPlayer);
		public EstimateNode<TBoard> EstimateNodeForSelection;
		public double ExplorationConstant = 1.4;
		public EstimateMove<TBoard> EstimateNodeForSimulation = (move, b) => 0.0;
		public Action<string> Log = s => { };
		public int MaxSimulationsCount = 1000;
		public TimeSpan MaxTime = TimeSpan.FromMilliseconds(100);
		public Random Random = new Random();

		public IMove<TBoard> GetBestMove(TBoard startBoard)
		{
			var root = BuildGameTree(startBoard);
			return GetBestMove(startBoard, root);
		}

		private IMove<TBoard> GetBestMove(TBoard startBoard, Node<TBoard> root)
		{
			var estimatedChildren = GetEstimatedChildren(startBoard, root);
			LogMoveOptions(estimatedChildren);
			var best = estimatedChildren.SelectOne(n => n.estimate, Random);
			return best.child.Move;
		}

		private Node<TBoard> BuildGameTree(TBoard startBoard)
		{
			var root = new Node<TBoard>(startBoard.PlayersCount);
			var startTime = DateTime.Now;
			var simulationCount = 0;
			while (DateTime.Now - startTime < MaxTime
				   && simulationCount < MaxSimulationsCount)
			{
				var board = startBoard.MakeCopy();
				var newNode = GetExpandedNode(root, board);
				var scores = SimulateToEnd(board);
				BackpropagateScores(newNode, scores);
				simulationCount++;
			}

			Log($"simulations count: {simulationCount}");
			return root;
		}

		private void LogMoveOptions(IEnumerable<(Node<TBoard> node, double estimate)> estimatedOptions)
		{
			string Format((Node<TBoard> node, double estimate) child) =>
				$"  * estimate: {child.estimate} {child.node}";

			var text = string.Join("\n", estimatedOptions.Select(Format));
			Log($"Options:\n{text}");
		}

		private List<(Node<TBoard> child, double estimate)> GetEstimatedChildren(TBoard startBoard, Node<TBoard> root)
		{
			return root
				.GetChildren()
				.Select(n => (child: n, estimate: EstimateNodeForFinalChoice(n, startBoard)))
				.OrderByDescending(n => n.estimate)
				.ToList();
		}

		private void BackpropagateScores(Node<TBoard> node, double[] scores)
		{
			while (node != null)
			{
				node.RegisterPlay(scores);
				node = node.Parent;
			}
		}

		private Node<TBoard> GetExpandedNode(Node<TBoard> node, TBoard board)
		{
			while (node.GetUnvisitedChildren(board).Count == 0)
			{
				var children = node.GetChildren();
				if (children.Count == 0) return node;
				node = children
					.SelectOne(
						childNode => EstimateNodeForSelection(childNode, board),
						Random
					);
				node.Move.ApplyTo(board);
			}
			return Expand(node, board);
		}

		private Node<TBoard> Expand(Node<TBoard> node, TBoard board)
		{
			var child = node.GetUnvisitedChildren(board)
				.SelectOne(n => EstimateNodeForExpansion(n, board), Random);
			node.MakeVisited(child);
			child.Move.ApplyTo(board);
			return child;
		}

		private double[] SimulateToEnd(TBoard board)
		{
			while (!board.IsFinished())
			{
				var possibleMoves = board.GetPossibleMoves();
				var move = possibleMoves.SelectOne(
					m => EstimateNodeForSimulation(m, board),
					Random);
				move.ApplyTo(board);
			}
			return board.GetScores();
		}
	}
}