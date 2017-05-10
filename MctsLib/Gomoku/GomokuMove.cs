namespace MctsLib.Gomoku
{
	public class GomokuMove : IMove<GomokuGame>
	{
		public readonly int X, Y;

		public GomokuMove(int x, int y)
		{
			X = x;
			Y = y;
		}

		public void ApplyTo(GomokuGame game)
		{
			game.MakeMove(X, Y);
		}

		public override string ToString()
		{
			return $"({X}, {Y})";
		}

		public (int x, int y) ToCoord() => (X, Y);
	}
}