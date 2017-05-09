using System.Collections.Generic;
using System.Linq;

namespace MctsLib.Tests.TicTacToe
{
	public class TicTacToeMove : IMove<TicTacToeBoard>
	{
		public readonly int X, Y;

		public TicTacToeMove(int x, int y)
		{
			X = x;
			Y = y;
		}

		public void ApplyTo(TicTacToeBoard board)
		{
			board.MakeMove(X, Y);
		}

		public override string ToString()
		{
			return $"({X}, {Y})";
		}
	}
}