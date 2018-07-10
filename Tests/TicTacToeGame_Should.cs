using System;
using System.Collections.Generic;
using lib;
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
			RunGame(rnd, true);
		}

		[Test]
		public void BeSolvable()
		{
			double[] scores = new double[2];
			var rnd = new Random();
			for (int i = 0; i < 10; i++)
			{
				var s = RunGame(rnd);
				scores[0] += s[0];
				scores[1] += s[1];
			}
			Console.WriteLine(string.Join(" : ", scores));
			Assert.AreEqual(new[] { 5.0, 5 }, scores);
		}

		private static double[] RunGame(Random rnd, bool log = false)
		{
			var game = new TicTacToeGame();
			var mcts = new Mcts<TicTacToeGame>(rnd);
			while (!game.IsFinished())
			{
				if (log) mcts.Log = Console.WriteLine;
				var move = mcts.GetBestMove(game, 200);
				move.ApplyTo(game);
				if (log) Console.WriteLine(game);
			}
			return game.GetScores();
		}
	}
}