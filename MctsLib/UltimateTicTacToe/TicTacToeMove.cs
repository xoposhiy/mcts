using System.Collections.Generic;
using System.Linq;

namespace lib
{
	public class UltimateTTTMove : IMove<UltimateTTTGame>
	{
		public readonly int X, Y;

		public UltimateTTTMove(int x, int y)
		{
			X = x;
			Y = y;
		}

		public void ApplyTo(UltimateTTTGame game)
		{
			game.MakeMove(X, Y);
		}

		public override string ToString()
		{
			return $"({X}, {Y})";
		}
	}
}