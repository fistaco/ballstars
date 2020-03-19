using System;
using System.Collections.Generic;
using System.Linq;

namespace TeamBuilder.Entity
{
    internal class Event
    {
        public int TeamOneId;
        public int TeamTwoId;

        public readonly List<SportsMatch> Matches;

        public int VarietyPenalty;

        public int RefereesRequired;
        
        private readonly Dictionary<SportsMatchCategory, int> _categoryCounts = new Dictionary<SportsMatchCategory, int>
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
            {SportsMatchCategory.Referee, 0}
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

            UpdateVarietyPenaltyAfterMatchAddition(match.MatchType);
            UpdateVarietyPenaltyForSimilarCategories(match, true);
            UpdateRequiredRefereeAmount(match.MatchType, true);

            _categoryCounts[match.MatchType]++;
        }

        public void RemoveMatchAtIndex(int matchIndex)
        {
            SportsMatch match = this.Matches[matchIndex];

            UpdateVarietyPenaltyAfterMatchRemoval(match.MatchType);
            UpdateVarietyPenaltyForSimilarCategories(match, false);
            UpdateRequiredRefereeAmount(match.MatchType, false);

            _categoryCounts[match.MatchType]--;
            
            this.Matches.RemoveAt(matchIndex);
        }

        /// <summary>
        /// Updates the variety penalty for the singles or doubles counterpart of a category that has just been added or
        /// removed.
        /// </summary>
        /// <param name="match"></param>
        /// <param name="added"></param>
        public void UpdateVarietyPenaltyForSimilarCategories(SportsMatch match, bool added)
        {
            // If a category's counterpart has just been added/removed, consider it when updating the variety penalty
            // e.g. if a badminton match has just been added, check the badmintonDoubles category for duplicates as well
            SportsMatchCategory cat = match.MatchType;
            
            switch (cat)
            {
                case SportsMatchCategory.Badminton:
                    UpdateVarietyPenaltyAfterMatchUpdate(SportsMatchCategory.BadmintonDoubles, added);
                    break;
                case SportsMatchCategory.BadmintonDoubles:
                    UpdateVarietyPenaltyAfterMatchUpdate(SportsMatchCategory.Badminton, added);
                    break;
                case SportsMatchCategory.TableTennis:
                    UpdateVarietyPenaltyAfterMatchUpdate(SportsMatchCategory.TableTennisDoubles, added);
                    break;
                case SportsMatchCategory.TableTennisDoubles:
                    UpdateVarietyPenaltyAfterMatchUpdate(SportsMatchCategory.TableTennis, added);
                    break;
            }
        }

        public void UpdateVarietyPenaltyAfterMatchUpdate(SportsMatchCategory category, bool added)
        {
            if (added)
            {
                UpdateVarietyPenaltyAfterMatchAddition(category);
            }
            else
            {
                UpdateVarietyPenaltyAfterMatchRemoval(category);
            }
        }

        public void UpdateVarietyPenaltyAfterMatchAddition(SportsMatchCategory category)
        {
            // Increase the variety penalty if this event will now have (even more) duplicate sport categories
            if (_categoryCounts[category] > 0)
            {
                this.VarietyPenalty++;
            }
        }

        public void UpdateVarietyPenaltyAfterMatchRemoval(SportsMatchCategory category)
        {
            // Decrease the variety penalty if there were duplicate sport categories in this event
            if (_categoryCounts[category] > 1)
            {
                this.VarietyPenalty--;
            }
        }

        public void UpdateVarietyPenaltyAfterSwap(SportsMatch old, SportsMatch @new)
        {
            UpdateVarietyPenaltyAfterMatchRemoval(old.MatchType);
            UpdateVarietyPenaltyAfterMatchAddition(@new.MatchType);
        }
        
        public void UpdateRequiredRefereeAmount(SportsMatchCategory category, bool added)
        {
            int modification = added ? 1 : -1;

            if (Globals.MatchTypesRequiringReferee.Contains(category))
            {
                this.RefereesRequired += modification;
            }
        }

        /// <summary>
        /// Constructs an Event with a random amount of sports matches where the amount of participating players remains
        /// under the limit if one is given.
        /// </summary>
        /// <param name="teamOneId">The ID of one of the teams participating in this event.</param>
        /// <param name="teamTwoId">The ID of the other team participating in this event.</param>
        /// <param name="matchPool">List of matches from which the random matches in this event will be chosen.</param>
        /// <param name="avgPlayersPerTeam">The maximum amount of players per team that will be assigned to this event.
        /// </param>
        /// <param name="round">The round in which this event is planned.</param>
        /// <param name="playersPerMatchType"></param>
        /// <returns></returns>
        public static Event Random(int teamOneId, int teamTwoId, List<SportsMatch> matchPool, int avgPlayersPerTeam,
            Dictionary<SportsMatchCategory, int> playersPerMatchType)
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
                
                // Check if there is enough space for the new match's sport within this round
                int newMatchTypeCount = playersPerMatchType[match.MatchType] + match.PlayersPerTeam;
                if (newMatchTypeCount > Globals.MatchTypePlayerLimitsPerTeam[match.MatchType])
                {
                    continue;
                }

                evnt.AddMatch(match);
                allocatedPlayers = newPlayerCount;
                playersPerMatchType[match.MatchType] = newMatchTypeCount;
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

            foreach (KeyValuePair<SportsMatchCategory,int> pair in _categoryCounts)
            {
                clone._categoryCounts[pair.Key] = pair.Value;
            }

            clone.VarietyPenalty = this.VarietyPenalty;
            clone.RefereesRequired = this.RefereesRequired;

            return clone;
        }

        public override string ToString()
        {
            string matches = string.Join(" ", Matches.Select(m => m.ToString()));
            return $"{TeamOneId} - {TeamTwoId}: {matches}";
        }
    }
}