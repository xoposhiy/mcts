using System;
using MctsLib.Gomoku;
using NUnit.Framework;

namespace MctsLib.Tests.Gomoku
{
	[TestFixture]
	public class Gomoku_Should
	{
		[TestCase(0, 0, 1, 0)]
		[TestCase(2, 0, 1, 0)]
		[TestCase(0, 0, 0, 1)]
		[TestCase(0, 2, 0, 1)]
		[TestCase(0, 0, 1, 1)]
		[TestCase(1, 1, 1, 1)]
		[TestCase(6, 0, -1, 1)]
		[TestCase(6, 1, -1, 1)]
		[TestCase(0, 1, 1, 0)]
		[TestCase(0, 1, 0, 1)]
		[TestCase(0, 1, 1, 1)]
		[TestCase(6, 1, -1, 1)]
		public void FindWinnerX(int x0, int y0, int dx, int dy)
		{
			var xGame = new GomokuGame();
			var oGame = new GomokuGame();
			for (var i = 0; i < 5; i++)
			{
				xGame.MakeMove(x0 + dx * i, y0 + dy * i);
				xGame.MakeMove(6, 6);
				oGame.MakeMove(6, 6);
				oGame.MakeMove(x0 + dx * i, y0 + dy * i);
			}
			Assert.AreEqual(new[] { 1.0, 0 }, xGame.GetScores());
			Assert.AreEqual(new[] { 0, 1.0 }, oGame.GetScores());
		}

		[TestCase]
		public void HaveEvenScore_WhenNoWinner()
		{
			var game = new GomokuGame();
			Assert.AreEqual(new[] { 0.5, 0.5 }, game.GetScores());
			game.MakeMove(1, 2);
			Assert.AreEqual(new[] { 0.5, 0.5 }, game.GetScores());
		}

		[Test]
		public void PlayGame()
		{
			var mcts = new Mcts<GomokuGame>
			{
				MaxTime = TimeSpan.MaxValue,
				MaxSimulationsCount = 10000,
				ExplorationConstant = 0.5,
				//Log = Console.WriteLine
			};
			var game = new GomokuGame();
			var i = 0;
			while (!game.IsFinished)
			{
				var move = mcts.GetBestMove(game);
				move.ApplyTo(game);
				//Console.WriteLine(game);
				//if (i++ > 10) break;
			}
			Console.WriteLine(string.Join(":", game.GetScores()));
		}
	}
}