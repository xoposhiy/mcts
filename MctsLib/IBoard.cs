using System.Collections.Generic;

namespace MctsLib
{
	public interface IBoard<TBoard> where TBoard : IBoard<TBoard>
	{
		int CurrentPlayer { get; }
		int PlayersCount { get; }
		bool IsFinished();

		/// <returns>applying moves to b.MakeCopy() result should not effect b itself</returns>
		TBoard MakeCopy();

		/// <returns>should be empty if game is finished</returns>
		IEnumerable<IMove<TBoard>> GetPossibleMoves();

		/// <returns>scores for every player in game</returns>
		double[] GetScores();
	}
}