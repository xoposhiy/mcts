using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace MctsLib.Tests.TicTacToe
{
	[TestFixture]
	public class TicTacToeBoard_Should
	{
		[Test]
		public void BeSolvable()
		{
			var xIterationsCount = 2000;
			var oIterationsCount = 2000;
			double[] scores = new double[2];
			var rnd = new Random();
			for (int i = 0; i < 10; i++)
			{
				var s = RunGame(xIterationsCount, oIterationsCount, rnd);
				scores[0] += s[0];
				scores[1] += s[1];
			}
			Console.WriteLine(string.Join(" : ", scores));
			Assert.AreEqual(new[] { 5.0, 5 }, scores);
		}

		private static double[] RunGame(int xIterationsCount, int oIterationsCount, Random rnd)
		{
			var board = new TicTacToeBoard();
			while (!board.IsFinished())
			{
				var mcts = new Mcts<TicTacToeBoard>
				{
					Random = rnd,
					MaxSimulationsCount = board.CurrentPlayer == 0 ? xIterationsCount : oIterationsCount,
					MaxTime = TimeSpan.MaxValue
				};
				var move = mcts.GetBestMove(board);
				move.ApplyTo(board);
				//Console.WriteLine(board);
			}
			return board.GetScores();
		}
	}
}