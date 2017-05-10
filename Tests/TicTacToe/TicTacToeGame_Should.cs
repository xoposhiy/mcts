using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace MctsLib.Tests.TicTacToe
{
	[TestFixture]
	public class TicTacToeGame_Should
	{
		[Test]
		public void SolveOne()
		{
			var rnd = new Random();
			RunGame(100000, 100000, rnd, true);
		}

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

		private static double[] RunGame(int xIterationsCount, int oIterationsCount, Random rnd, bool log = false)
		{
			var game = new TicTacToeGame();
			while (!game.IsFinished())
			{
				var mcts = new Mcts<TicTacToeGame>
				{
					Random = rnd,
					MaxSimulationsCount = game.CurrentPlayer == 0 ? xIterationsCount : oIterationsCount,
					MaxTime = TimeSpan.MaxValue,
					//ExplorationConstant = 1
				};
				if (log) mcts.Log = Console.WriteLine;
				var move = mcts.GetBestMove(game);
				move.ApplyTo(game);
				if (log) Console.WriteLine(game);
			}
			return game.GetScores();
		}
	}
}