using System.Collections;
using System.Collections.Generic;

namespace MctsLib
{
	public interface IBoard
	{
		IBoard MakeCopy();
		int CurrentPlayer { get; }
		int PlayersCount { get; }
		IList<IMove> GetPossibleMoves();
		IBoard ApplyMove(IMove chooseRandom);
		bool IsFinished();
		double[] GetScores();
	}
}