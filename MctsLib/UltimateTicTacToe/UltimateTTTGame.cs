using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace lib
{
	public class UltimateTTTGame : IGame<UltimateTTTGame>
	{
		private readonly TicTacToeGame[,] cells;

		public UltimateTTTGame(TicTacToeGame[,] cells, int currentPlayer, Vec lastPlay)
		{
			this.cells = cells;
			CurrentPlayer = currentPlayer;
			LastPlay = lastPlay;
		}

		public UltimateTTTGame()
			: this(CreateCells(3), 0, new Vec(-1, -1))
		{
		}

		private static TicTacToeGame[,] CreateCells(int size)
		{
			var res = new TicTacToeGame[size, size];
			for (int x = 0; x < size; x++)
				for (int y = 0; y < size; y++)
					res[x, y] = new TicTacToeGame();
			return res;
		}

		public UltimateTTTGame MakeCopy()
		{
			return new UltimateTTTGame(CloneCells(), CurrentPlayer, LastPlay);
		}

		private TicTacToeGame[,] CloneCells()
		{
			var size = cells.GetLength(0);
			var res = new TicTacToeGame[size, size];
			for (int x = 0; x < size; x++)
				for (int y = 0; y < size; y++)
					res[x, y] = cells[x, y].MakeCopy();
			return res;
		}

		public int CurrentPlayer { get; private set; }
		public Vec LastPlay { get; set; }
		public int PlayersCount => 2;
		public TicTacToeGame Miniboard(int x, int y) => cells[x, y];

		public ICollection<IMove<UltimateTTTGame>> GetPossibleMoves()
		{
			if (GetWinner() >= 0) return new List<IMove<UltimateTTTGame>>();
			if (LastPlay.X >= 0)
			{
				int boardX = LastPlay.X % 3;
				int boardY = LastPlay.Y % 3;
				var board = cells[boardX, boardY];
				if (!board.IsFinished())
					return board.GetPossibleTTTMoves().Select(m => new UltimateTTTMove(boardX * 3 + m.X, boardY * 3 + m.Y) as IMove<UltimateTTTGame>).ToList();
			}

			var moves =
				from x in new[] { 0, 1, 2 }
				from y in new[] { 0, 1, 2 }
				from m in cells[x, y].GetPossibleTTTMoves()
				select new UltimateTTTMove(x * 3 + m.X, y * 3 + m.Y) as IMove<UltimateTTTGame>;
			return moves.ToList();
		}

		public void MakeMove(int x, int y)
		{
			var miniBoard = cells[x / 3, y / 3];
			miniBoard.CurrentPlayer = CurrentPlayer;
			miniBoard.MakeMove(x % 3, y % 3);
			CurrentPlayer = 1 - CurrentPlayer;
			LastPlay = new Vec(x, y);
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
			var score = new[] { 0, 0.0 };
			for (int x = 0; x < 3; x++)
				for (int y = 0; y < 3; y++)
				{
					int miniWinner = cells[x, y].GetWinner();
					if (miniWinner >= 0) score[miniWinner]+=0.01;
				}
			return score;
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

		private int? SameSymbolInLine(int x0, int y0, int dx, int dy)
		{
			var sym = cells[x0, y0];
			int winner = sym.GetWinner();
			if (winner == -1) return null;
			for (var i = 1; i < 3; i++)
				if (winner != cells[x0 + dx * i, y0 + dy * i].GetWinner()) return null;
			return winner + 1;
		}

		public int this[int x, int y] => cells[x / 3, y / 3][x % 3, y % 3];
		public string[] X = { @"\ /", " X ", @"/ \" };
		public string[] O = { @"Г^T", "| |", @"L_J" };

		public override string ToString()
		{
			var syms = ".XO";
			var sb = new StringBuilder();
			for (var y = 0; y < 9; y++)
			{
				for (var x = 0; x < 9; x++)
				{
					int winPlayer = cells[x/3, y/3].GetWinner();
					char smallSym = winPlayer == -1 ? syms[this[x, y]] : winPlayer == 0 ? X[y%3][x%3] : O[y%3][x%3];
					sb.Append(smallSym);
				}
				sb.AppendLine();
			}
			sb.AppendLine("CurrentPlayer: " + syms[CurrentPlayer + 1]);
			return sb.ToString();
		}
	}
}