﻿using System.Collections.Generic;

namespace MctsLib
{
	public interface IGame<TActualGame> where TActualGame : IGame<TActualGame>
	{
		int CurrentPlayer { get; }
		int PlayersCount { get; }

		/// <summary>applying moves to b.MakeCopy() result should not effect b itself</summary>
		TActualGame MakeCopy();

		/// <returns>should be empty if game is finished</returns>
		IEnumerable<IMove<TActualGame>> GetPossibleMoves();

		/// <returns>scores for every player in game after game is finished</returns>
		double[] GetScores();
	}
}