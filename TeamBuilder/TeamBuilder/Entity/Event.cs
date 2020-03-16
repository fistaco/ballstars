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

        public int VarietyPenalty;
        
        private Dictionary<SportsMatchCategory, int> _categoryCounts = new Dictionary<SportsMatchCategory, int>()
        {
            {SportsMatchCategory.Badminton, 0},
            {SportsMatchCategory.BadmintonDoubles, 0},
            {SportsMatchCategory.Basketball, 0},
            {SportsMatchCategory.Floorball, 0},
            {SportsMatchCategory.Korfball, 0},
            // {Sport.Squash, 0},
            {SportsMatchCategory.TableTennis, 0},
            {SportsMatchCategory.TableTennisDoubles, 0},
            // {Sport.Volleyball, 0}
        };

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

        public void AddMatch(SportsMatch match)
        {
            this.Matches.Add(match);

            // Increase the variety penalty if this event will now have (even more) duplicate sport categories
            if (_categoryCounts[match.MatchType] > 0)
            {
                this.VarietyPenalty++;
            }
            
            _categoryCounts[match.MatchType]++;
        }

        public void RemoveMatch(SportsMatch match)
        {
            this.Matches.Remove(match);

            // Decrease the variety penalty if there were duplicate sport categories in this event
            if (_categoryCounts[match.MatchType] > 1)
            {
                this.VarietyPenalty--;
            }

            
            _categoryCounts[match.MatchType]--;
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

            clone.VarietyPenalty = this.VarietyPenalty;

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