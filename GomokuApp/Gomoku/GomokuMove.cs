namespace lib.Gomoku
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
			game.MakeMove(this);
		}

		public override string ToString()
		{
			return $"({X}, {Y})";
		}

		protected bool Equals(GomokuMove other)
		{
			return X == other.X && Y == other.Y;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((GomokuMove) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (X * 397) ^ Y;
			}
		}

		public static bool operator ==(GomokuMove left, GomokuMove right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(GomokuMove left, GomokuMove right)
		{
			return !Equals(left, right);
		}

		public (int x, int y) ToCoord() => (X, Y);
	}
}