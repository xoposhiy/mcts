using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace lib
{
	public class TicTacToeGame : IGame<TicTacToeGame>
	{
		private readonly int[,] cells;

		public TicTacToeGame(int[,] cells, int currentPlayer)
		{
			this.cells = cells;
			CurrentPlayer = currentPlayer;
		}

		public TicTacToeGame()
			: this(new int[3, 3], 0)
		{
		}

		public TicTacToeGame MakeCopy()
		{
			return new TicTacToeGame((int[,]) cells.Clone(), CurrentPlayer);
		}

		public int CurrentPlayer { get; set; }
		public int PlayersCount => 2;

		public ICollection<TicTacToeMove> GetPossibleTTTMoves()
		{
			if (GetWinner() >= 0) return new List<TicTacToeMove>();
			var moves =
				from x in new[] { 0, 1, 2 }
				from y in new[] { 0, 1, 2 }
				where cells[x, y] == 0
				select new TicTacToeMove(x, y);
			return moves.ToList();
		}
		public ICollection<IMove<TicTacToeGame>> GetPossibleMoves()
		{
			if (GetWinner() >= 0) return new List<IMove<TicTacToeGame>>();
			var moves =
				from x in new[] { 0, 1, 2 }
				from y in new[] { 0, 1, 2 }
				where cells[x, y] == 0
				select new TicTacToeMove(x, y) as IMove<TicTacToeGame>;
			return moves.ToList();
		}

		public void MakeMove(int x, int y)
		{
			cells[x, y] = 1 + CurrentPlayer;
			CurrentPlayer = 1 - CurrentPlayer;
			winner = null;
		}

		public bool IsFinished()
		{
			return !GetPossibleMoves().Any();
		}

		public double[] GetScores()
		{
			var w = GetWinner();
			if (w == 0) return new[] { 1.0, 0 };
			else if (w == 1) return new[] { 0, 1.0 };
			else return new[] { 0.5, 0.5 };
		}

		private int? winner;

		public int GetWinner()
		{
			if (winner.HasValue) return winner.Value;
			var sym =
				SameSymbolInLine(0, 0, 1, 0)
				?? SameSymbolInLine(0, 1, 1, 0)
				?? SameSymbolInLine(0, 2, 1, 0)
				?? SameSymbolInLine(0, 0, 0, 1)
				?? SameSymbolInLine(1, 0, 0, 1)
				?? SameSymbolInLine(2, 0, 0, 1)
				?? SameSymbolInLine(0, 0, 1, 1)
				?? SameSymbolInLine(2, 0, -1, 1)
				?? 0;
			sym--;
			winner = sym;
			return sym;
		}

		public bool IsWinMove(int x, int y)
		{
			cells[x, y] = 1 + CurrentPlayer;
			var sym1 = SameSymbolInLine(0, y, 1, 0) ?? SameSymbolInLine(x, 0, 0, 1) ??
					   SameSymbolInLine(0, 0, 1, 1) ?? SameSymbolInLine(2, 0, -1, 1);
			cells[x, y] = 2-CurrentPlayer;
			var sym2 = SameSymbolInLine(0, y, 1, 0) ?? SameSymbolInLine(x, 0, 0, 1) ??
					   SameSymbolInLine(0, 0, 1, 1) ?? SameSymbolInLine(2, 0, -1, 1);
			cells[x, y] = 0;
			return (sym1 ?? sym2) != null;
		}

		public int? SameSymbolInLine(int x0, int y0, int dx, int dy)
		{
			var sym = cells[x0, y0];
			if (sym == 0) return null;
			for (var i = 1; i < 3; i++)
				if (sym != cells[x0 + dx * i, y0 + dy * i]) return null;
			return sym;
		}

		public override string ToString()
		{
			var syms = ".XO";
			var sb = new StringBuilder();
			for (var y = 0; y < 3; y++)
			{
				for (var x = 0; x < 3; x++)
					sb.Append(syms[cells[x, y]]);
				sb.AppendLine();
			}
			sb.AppendLine("CurrentPlayer: " + syms[CurrentPlayer + 1]);
			return sb.ToString();
		}

		public int this[int x, int y] => cells[x, y];
	}
}