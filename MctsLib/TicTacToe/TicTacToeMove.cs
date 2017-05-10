using System.Collections.Generic;
using System.Linq;

namespace MctsLib.TicTacToe
{
	public class TicTacToeMove : IMove<TicTacToeGame>
	{
		public readonly int X, Y;

		public TicTacToeMove(int x, int y)
		{
			X = x;
			Y = y;
		}

		public void ApplyTo(TicTacToeGame game)
		{
			game.MakeMove(X, Y);
		}

		public override string ToString()
		{
			return $"({X}, {Y})";
		}
	}
}