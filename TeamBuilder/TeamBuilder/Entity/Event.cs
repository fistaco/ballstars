using System.Collections.Generic;

namespace TeamBuilder.Entity
{
    internal class Event
    {
        public int TeamOneId;
        public int TeamTwoId;

        public List<SportsMatch> Matches;

        public Event(int teamOneId, int teamTwoId)
        {
            this.TeamOneId = teamOneId;
            this.TeamTwoId = teamTwoId;
            
            this.Matches = new List<SportsMatch>();
        }
    }
}