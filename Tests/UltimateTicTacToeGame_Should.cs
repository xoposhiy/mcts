using System;
using System.Collections.Generic;
using System.Linq;
using lib;
using NUnit.Framework;

namespace MctsLib.Tests.TicTacToe
{
	[TestFixture]
	public class UltimateTicTacToeGame_Should
	{
		[Test]
		public void SolveOne()
		{
			var rnd = new Random();
			RunGame(rnd, false);
		}

		[Test]
		public void CompareBots()
		{
			var rnd = new Random();

			var score = new[] { 0, 0 };
			var count = 10;
			for (int i = 0; i < count; i++)
			{
				var mcts1 = new Mcts<UltimateTTTGame>(random)
				{
					//Log = s => Console.Error.WriteLine(s),
					ExplorationConstant = 2,
					StrategyForSimulation = (game, moves) => moves.GetRandomBest(move => ScoreMove3(game, (UltimateTTTMove)move), random)
			};
				var mcts2 = new Mcts<UltimateTTTGame>(random)
				{
					//Log = s => Console.Error.WriteLine(s),
					ExplorationConstant = 2,
					StrategyForSimulation = (game, moves) => moves.GetRandomBest(move => ScoreMove3(game, (UltimateTTTMove)move), random)
				}; int winner = RunGame(rnd, mcts1, mcts2);
				if (winner >= 0) score[winner]++;
			}

			Console.WriteLine($"{score[0]} : {score[1]} (draws: {count - score.Sum()})");
			
		}

		private int RunGame(Random rnd, Mcts<UltimateTTTGame> mcts1, Mcts<UltimateTTTGame> mcts2)
		{
			var game = new UltimateTTTGame();
			while (!game.IsFinished())
			{
				var mcts = game.CurrentPlayer == 0 ? mcts1 : mcts2;
				var move = mcts.GetBestMove(game, 100);
				move.ApplyTo(game);
			}
			Console.WriteLine(game.GetWinner());
			return game.GetWinner();
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
			var game = new UltimateTTTGame();
			var mcts = new Mcts<UltimateTTTGame>(rnd);
			mcts.StrategyForSimulation = Dummy;
			if (log) mcts.Log = Console.WriteLine;

			while (!game.IsFinished())
			{
				
				var move = game.CurrentPlayer == 0 ? mcts.GetBestMove(game, 100) : Dummy(game, game.GetPossibleMoves());
				move.ApplyTo(game);
				if (log)
				{
					Console.WriteLine(game);
					Console.WriteLine(game.GetWinner());
				}
			}
			return game.GetScores();
		}

		static readonly Random random = new Random();
		private static IMove<UltimateTTTGame> Dummy(UltimateTTTGame game, ICollection<IMove<UltimateTTTGame>> moves)
		{
			return moves.GetRandomBest(move => ScoreMove(game, (UltimateTTTMove)move), random);
		}
		private static IMove<UltimateTTTGame> Dummy2(UltimateTTTGame game, ICollection<IMove<UltimateTTTGame>> moves)
		{
			return moves.GetRandomBest(move => ScoreMove2(game, (UltimateTTTMove)move), random);
		}

		private static readonly int[,] ws = { { 1, 0, 1 }, { 0, 2, 0 }, { 1, 0, 1 } };
		private static double ScoreMove(UltimateTTTGame game, UltimateTTTMove move)
		{
			var win = game.Miniboard(move.X / 3, move.Y / 3).IsWinMove(move.X % 3, move.Y % 3);
			return win ? 10 : ws[move.X % 3, move.Y % 3];
		}
		private static double ScoreMove2(UltimateTTTGame game, UltimateTTTMove move)
		{
			var win = game.Miniboard(move.X / 3, move.Y / 3).IsWinMove(move.X % 3, move.Y % 3);
			return win ? 10 : 0;
		}

		private static double ScoreMove3(UltimateTTTGame game, UltimateTTTMove move)
		{
			return ws[move.X % 3, move.Y % 3];
		}

	}
}