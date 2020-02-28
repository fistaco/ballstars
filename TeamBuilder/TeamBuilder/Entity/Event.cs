using System.Collections.Generic;

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

        public static Event Random(int teamOneId, int teamTwoId, List<SportsMatch> matchPool)
        {
            var evnt = new Event(teamOneId, teamTwoId);
            
            // Add 1 to 3 random SportsMatch objects from the given pool.
            int amountToAdd = Globals.Rand.Next(1, 4);
            for (int i = 0; i < amountToAdd; i++)
            {
                evnt.Matches.Add(SportsMatch.Random(matchPool));
            }

            return evnt;
        }
    }
}