using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MctsLib.Gomoku
{
	public class GomokuGame : IGame<GomokuGame>
	{
		private static readonly ICollection<IMove<GomokuGame>> EmptyCollection = new IMove<GomokuGame>[0];
		private readonly int[,] cells;
		private readonly HashSet<IMove<GomokuGame>> reasonableMoves;
		public ICollection<IMove<GomokuGame>> ReasonableMoves => reasonableMoves;
		private int winner = -1;

		public GomokuGame(int size = 9)
			: this(new int[size, size], 0)
		{
		}

		public GomokuGame(int[,] cells, int currentPlayer, HashSet<IMove<GomokuGame>> reasonableMoves)
		{
			CurrentPlayer = currentPlayer;
			this.cells = cells;
			this.reasonableMoves = reasonableMoves;
			Size = cells.GetLength(0);
		}

		public GomokuGame(int[,] cells, int currentPlayer)
		{
			if (cells.GetLength(0) != cells.GetLength(1))
				throw new ArgumentException("not a square", nameof(cells));
			this.cells = cells;
			CurrentPlayer = currentPlayer;
			Size = cells.GetLength(0);
			reasonableMoves = new HashSet<IMove<GomokuGame>> { new GomokuMove(Size / 2, Size / 2) };
		}

		public GomokuGame()
			: this(new int[9, 9], 0)
		{
		}

		public int Size { get; }

		public int this[int x, int y] => cells[x, y];

		public bool IsFinished => !GetPossibleMoves().Any();


		public int CurrentPlayer { get; private set; }
		public int PlayersCount => 2;

		public GomokuGame MakeCopy()
		{
			return new GomokuGame((int[,])cells.Clone(), CurrentPlayer, new HashSet<IMove<GomokuGame>>(reasonableMoves));
		}

		public ICollection<IMove<GomokuGame>> GetPossibleMoves()
		{
			// performace critical method
			return winner >= 0 ? EmptyCollection : reasonableMoves;
		}

		public double[] GetScores()
		{
			if (winner == 0) return new[] { 1.0, 0 };
			else if (winner == 1) return new[] { 0, 1.0 };
			else return new[] { 0.5, 0.5 };
		}

		public void MakeMove(GomokuMove move)
		{
			reasonableMoves.Remove(move);
			cells[move.X, move.Y] = 1 + CurrentPlayer;
			if (HasWinSequence(move.X, move.Y)) winner = CurrentPlayer;
			else
			{
				var minX = Math.Max(0, move.X - 2);
				var maxX = Math.Min(Size - 1, move.X + 2);
				var minY = Math.Max(0, move.Y - 2);
				var maxY = Math.Min(Size - 1, move.Y + 2);
				for (int x = minX; x <= maxX; x++)
				for (int y = minY; y <= maxY; y++)
					if (cells[x, y] == 0) reasonableMoves.Add(new GomokuMove(x, y));
			}
			CurrentPlayer = 1 - CurrentPlayer;
		}

		public void MakeMove(int x, int y)
		{
			MakeMove(new GomokuMove(x, y));
		}

		private bool HasWinSequence(int x, int y)
		{
			return
				GetRayLen(x, y, 1, 0) + GetRayLen(x, y, -1, 0) >= 4
				|| GetRayLen(x, y, 0, 1) + GetRayLen(x, y, 0, -1) >= 4
				|| GetRayLen(x, y, 1, 1) + GetRayLen(x, y, -1, -1) >= 4
				|| GetRayLen(x, y, 1, -1) + GetRayLen(x, y, -1, 1) >= 4;
		}

		private int GetRayLen(int x0, int y0, int dx, int dy)
		{
			var sym = cells[x0, y0];
			var x = x0;
			var y = y0;
			var i = 0;
			while (i < 5 && x >= 0 && x < Size && y >= 0 && y < Size)
			{
				if (cells[x, y] != sym) break;
				x += dx;
				y += dy;
				i++;
			}
			return i - 1;
		}

		public override string ToString()
		{
			var syms = ".XO";
			var sb = new StringBuilder();
			for (var y = 0; y < Size; y++)
			{
				for (var x = 0; x < Size; x++)
					sb.Append(syms[cells[x, y]]);
				sb.AppendLine();
			}
			sb.AppendLine("CurrentPlayer: " + syms[CurrentPlayer + 1]);
			return sb.ToString();
		}
	}
}