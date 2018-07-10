using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.Linq;

/**
 * Auto-generated code below aims at helping you parse
 * the standard input according to the problem statement.
 **/
namespace lib
{
	class Player
	{
		static readonly Random random = new Random();
		static void Main(string[] args)
		{
			string[] inputs;
			var game = new UltimateTTTGame();

			var mcts = new Mcts<UltimateTTTGame>(random)
			{
				Log = s => Console.Error.WriteLine(s),
				ExplorationConstant = 2,
				StrategyForSimulation = Dummy
			};
			int me = 1;
			bool isFirst = true;
			while (true)
			{
				inputs = Console.ReadLine().Split(' ');
				int opponentRow = int.Parse(inputs[0]);
				int opponentCol = int.Parse(inputs[1]);
				Console.Error.WriteLine(opponentRow + " " + opponentCol);
				int validActionCount = int.Parse(Console.ReadLine());
				var ms = new List<Tuple<int, int>>();
				for (int i = 0; i < validActionCount; i++)
				{
					inputs = Console.ReadLine().Split(' ');
					int row = int.Parse(inputs[0]);
					int col = int.Parse(inputs[1]);
					ms.Add(Tuple.Create(row, col));
				}
				var countdown = new Countdown(isFirst ? 800 : 90);
				isFirst = false;
				Console.Error.WriteLine("Valid actions count " + validActionCount);
				if (opponentRow < 0)
				{
					game.MakeMove(4, 4);
					var hisMove = mcts.GetBestMove(game, countdown); // heat up!
					Console.Error.WriteLine(hisMove);
					Console.WriteLine("4 4");
					continue;
				}
				else
				{
					game.MakeMove(opponentRow, opponentCol);
					if (game.GetPossibleMoves().Count != validActionCount)
					{
						Console.Error.WriteLine("Beda...");
					}
					UltimateTTTMove move = (UltimateTTTMove)mcts.GetBestMove(game, countdown);
					Console.WriteLine(move.X + " " + move.Y);
					move.ApplyTo(game);
				}

				Console.Error.WriteLine(countdown);
				
			}
		}

		private static IMove<UltimateTTTGame> Dummy(UltimateTTTGame game, ICollection<IMove<UltimateTTTGame>> moves)
		{
			return moves.GetRandomBest(move => ScoreMove(game, (UltimateTTTMove)move), random);
		}

		private static readonly int[,] ws = { { 1, 0, 1 }, { 0, 2, 0 }, { 1, 0, 1 } };
		private static double ScoreMove(UltimateTTTGame game, UltimateTTTMove move)
		{
			var win = game.Miniboard(move.X / 3, move.Y / 3).IsWinMove(move.X % 3, move.Y % 3);
			return win ? 10 : 0;//ws[move.X % 3, move.Y % 3];
		}
	}
}