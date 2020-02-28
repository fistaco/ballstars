using System.Collections.Generic;

namespace TeamBuilder.Entity
{
    public class SportsMatch
    {
        public SportsMatchCategory MatchType;
        public int PlayersPerTeam;

        public SportsMatch(SportsMatchCategory matchType, int playersPerTeam)
        {
            this.MatchType = matchType;
            this.PlayersPerTeam = playersPerTeam;
        }

        /// <summary>
        /// Returns a random SportsMatch event from a given pool.
        /// </summary>
        /// <returns></returns>
        public static SportsMatch Random(List<SportsMatch> matchPool)
        {
            int randIndex = Globals.Rand.Next(0, matchPool.Count);
            return matchPool[randIndex]; // TODO: Check if cloning is necessary
        }
        
        /// <summary>
        /// Returns a copy of this SportsMatch object.
        /// </summary>
        /// <returns></returns>
        public SportsMatch Clone()
        {
            return new SportsMatch(this.MatchType, this.PlayersPerTeam);
        }
    }
}