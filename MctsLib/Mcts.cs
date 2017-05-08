using System;
using System.Collections.Generic;
using System.Linq;

namespace MctsLib
{
	public class Mcts
	{
		private readonly IBoard startBoard;
		private readonly Action<string> log;
		private readonly Random random;
		private readonly Node root;

		public Mcts(IBoard startBoard, Action<string> log, Random random)
		{
			this.startBoard = startBoard;
			this.log = log;
			this.random = random;
			root = new Node(startBoard);
		}

		public IMove GetBestMove(TimeSpan timeLimit, int maxSimulationsCount)
		{
			var startTime = DateTime.Now;
			int simulationCount = 0;
			while (DateTime.Now - startTime < timeLimit && simulationCount < maxSimulationsCount)
			{
				var board = startBoard.MakeCopy();
				var nodeAndBoard = SelectExploredNode(root, board);
				var scores = SimulateToScores(nodeAndBoard.Item2);
				BackpropagateScores(nodeAndBoard.Item1, scores);
				simulationCount++;
			}

			log($"simulations count: {simulationCount}");
			var best = GetBestNodes();
			return best.ChooseRandom(random).Move;
		}

		private List<Node> GetBestNodes()
		{
			var orderedNodes = root.GetChildren().OrderByDescending(node => node.GetExpectedScore(startBoard.CurrentPlayer)).ToList();
			foreach (var child in orderedNodes.Take(10))
				log($"option exp={child.GetExpectedScore(startBoard.CurrentPlayer)} {child.Move} plays={child.TotalPlays}");
			var bestScore = orderedNodes[0].GetExpectedScore(startBoard.CurrentPlayer);
			// ReSharper disable once CompareOfFloatsByEqualityOperator
			var best = orderedNodes.TakeWhile(n => n.GetExpectedScore(startBoard.CurrentPlayer) == bestScore);
			return best.ToList();
		}

		private void BackpropagateScores(Node node, double[] scores)
		{
			throw new NotImplementedException();
		}

		private Tuple<Node, IBoard> SelectExploredNode(Node node, IBoard board)
		{
			throw new NotImplementedException();
		}

		private double[] SimulateToScores(IBoard b)
		{
			while (!b.IsFinished())
			{
				var moves = b.GetPossibleMoves();
				b = b.ApplyMove(moves.ChooseRandom(random));
			}
			return b.GetScores();
		}
	}
}