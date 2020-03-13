using System;
using System.Collections.Generic;
using System.Linq;

namespace TeamBuilder.Entity
{
    internal class Event
    {
        public int TeamOneId;
        public int TeamTwoId;

        public List<SportsMatch> Matches;

        /// <summary>
        /// Constructs an event with an empty list of matches for the two given team IDs.
        /// </summary>
        /// <param name="teamOneId"></param>
        /// <param name="teamTwoId"></param>
        public Event(int teamOneId, int teamTwoId)
        {
            this.TeamOneId = teamOneId;
            this.TeamTwoId = teamTwoId;
            
            this.Matches = new List<SportsMatch>();
        }

        /// <summary>
        /// Constructs an Event with a random amount of sports matches where the amount of participating players remains
        /// under the limit if one is given.
        /// </summary>
        /// <param name="teamOneId"></param>
        /// <param name="teamTwoId"></param>
        /// <param name="matchPool"></param>
        /// <param name="avgPlayersPerTeam"></param>
        /// <returns></returns>
        public static Event Random(int teamOneId, int teamTwoId, List<SportsMatch> matchPool, int avgPlayersPerTeam)
        {
            var evnt = new Event(teamOneId, teamTwoId);
            
            // Add 1 to 3 random SportsMatch objects from the given pool.
            int amountToAdd = Globals.Rand.Next(1, 4);
            int allocatedPlayers = 0;
            for (int i = 0; i < amountToAdd; i++)
            {
                SportsMatch match = SportsMatch.Random(matchPool);

                // Check if the new match would stay within player limits
                int newPlayerCount = allocatedPlayers + match.PlayersPerTeam;
                if (newPlayerCount > avgPlayersPerTeam)
                {
                    break;
                }

                evnt.Matches.Add(match);
                allocatedPlayers = newPlayerCount;
            }

            return evnt;
        }

        public SportsMatch GetRandomSportsMatch()
        {
            return this.Matches[Globals.Rand.Next(this.Matches.Count)];
        }
        
        public Event Clone()
        {
            Event clone = new Event(TeamOneId, TeamTwoId);
            foreach (SportsMatch match in this.Matches)
            {
                clone.Matches.Add(match.Clone());
            }

            return clone;
        }

        public override string ToString()
        {
            // string matches =  Matches.Select(m => m.ToString()).Aggregate((result, matchString) => $"{result} {matchString}");
            string matches = string.Join(" ", Matches.Select(m => m.ToString()));
            return $"{TeamOneId} - {TeamTwoId}: {matches}";
        }
    }
}