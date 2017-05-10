using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MctsLib.Gomoku
{
	public class GomokuGame : IGame<GomokuGame>
	{
		private int winner = -1;
		private readonly int[,] cells;
		private Dictionary<(int, int), IMove<GomokuGame>> possibleMoves;

		public int Size => cells.GetLength(0);

		public int this[int x, int y]
		{
			get => cells[x, y];
		}

		public GomokuGame(int[,] cells, int currentPlayer)
		{
			if (cells.GetLength(0) != cells.GetLength(1))
				throw new ArgumentException("not a square", nameof(cells));
			this.cells = cells;
			CurrentPlayer = currentPlayer;
			var size = cells.GetLength(0);
			var moves =
				from x in Enumerable.Range(0, size)
				from y in Enumerable.Range(0, size)
				where cells[x, y] == 0
				select (x:x, y:y);
			possibleMoves = moves.ToDictionary(m => m, m => (IMove<GomokuGame>)new GomokuMove(m.x, m.y));
		}

		public int CurrentPlayer { get; private set; }
		public int PlayersCount => 2;
		public GomokuGame MakeCopy()
		{
			return new GomokuGame((int[,])cells.Clone(), CurrentPlayer);
		}

		public ICollection<IMove<GomokuGame>> GetPossibleMoves() 
			=> winner >= 0 ? (ICollection<IMove<GomokuGame>>)new IMove<GomokuGame>[0] : possibleMoves.Values;

		private int GetWinner()
		{
			return winner;
		}

		public double[] GetScores()
		{
			if (winner == 0) return new[] { 1.0, 0 };
			else if (winner == 1) return new[] { 0, 1.0 };
			else return new[] { 0.5, 0.5 };
		}

		public GomokuGame()
			: this(new int[9, 9], 0)
		{
		}

		public bool IsFinished => !GetPossibleMoves().Any();

		public void MakeMove(int x, int y)
		{
			cells[x, y] = 1 + CurrentPlayer;
			possibleMoves.Remove((x, y));
			if (HasWinSequence(x, y)) winner = CurrentPlayer;
			CurrentPlayer = 1 - CurrentPlayer;
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
			var size = cells.GetLength(0);
			var sym = cells[x0, y0];
			int x = x0;
			int y = y0;
			int i = 0;
			while (i < 5 && x >= 0 && x < size && y >= 0 && y < size)
			{
				if (cells[x, y] != sym) break;
				x += dx;
				y += dy;
				i++;
			}
			return i-1;
		}

		public override string ToString()
		{
			var size = cells.GetLength(0);
			var syms = ".XO";
			var sb = new StringBuilder();
			for (var y = 0; y < size; y++)
			{
				for (var x = 0; x < size; x++)
					sb.Append(syms[cells[x, y]]);
				sb.AppendLine();
			}
			sb.AppendLine("CurrentPlayer: " + syms[CurrentPlayer + 1]);
			return sb.ToString();
		}

	}
}