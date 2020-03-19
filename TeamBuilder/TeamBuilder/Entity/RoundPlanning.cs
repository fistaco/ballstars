using System;
using System.Collections.Generic;
using System.Linq;
using TeamBuilder.Entity.Individual;

namespace TeamBuilder.Entity
{
    internal class RoundPlanning
    {
        public Event[] Events;

        public int RefereesRequired => Events.Sum(e => e.RefereesRequired);
        
        public int RefereePenalty => (RefereesRequired - PlayersPerMatchType[SportsMatchCategory.Referee]).Abs();

        /// <summary>
        /// The amount of players assigned to each sport during this round, divided by 2 to track allocations per team.
        /// </summary>
        public Dictionary<SportsMatchCategory, int> PlayersPerMatchType = new Dictionary<SportsMatchCategory, int>()
        {
            { SportsMatchCategory.Badminton, 0 },
            { SportsMatchCategory.BadmintonDoubles, 0 },
            { SportsMatchCategory.Basketball, 0 },
            { SportsMatchCategory.Floorball, 0 },
            { SportsMatchCategory.Korfball, 0 },
            // { SportsMatchCategory.Squash, 0 },
            { SportsMatchCategory.TableTennis, 0 },
            { SportsMatchCategory.TableTennisDoubles, 0 },
            // { SportsMatchCategory.Volleyball, 0 },
            { SportsMatchCategory.Referee, 0 }
        };

        /// <summary>
        /// Constructs a new Round with an empty Event array for a given amount of events.
        /// </summary>
        /// <param name="amountOfEvents"></param>
        public RoundPlanning(int amountOfEvents)
        {
            this.Events = new Event[amountOfEvents];
        }

        /// <summary>
        /// Creates an empty array for this rounds' events. The length is based on whether the amount of teams is even
        /// or odd.
        /// </summary>
        /// <param name="amountOfTeams"></param>
        /// <returns></returns>
        private Event[] CreateEmptyRoundEventArray(int amountOfTeams)
        {
            return amountOfTeams % 2 == 0 ? new Event[amountOfTeams / 2] : new Event[amountOfTeams / 2 + 1];
        }

        public static RoundPlanning Random(int amountOfTeams, int amountOfEvents, int amountOfRegularEvents,
            List<SportsMatch> matchPool, int avgPlayersPerTeam, bool breakRound = false,
            List<Tuple<int, int>> predefinedMatchUps = null)
        {
            var round = new RoundPlanning(amountOfEvents);

            bool usePredefinedMatchUps = predefinedMatchUps != null;

            // Generate amountOfTeams/2 regular events.
            for (int i = 0; i < amountOfRegularEvents; i++)
            {
                // Use a predefined match-up if possible. Randomly generate one otherwise.
                (int t0, int t1) = usePredefinedMatchUps ?
                    (predefinedMatchUps[i].Item1, predefinedMatchUps[i].Item2) :
                    (Globals.Rand.Next(0, amountOfTeams), Globals.Rand.Next(0, amountOfTeams));

                round.Events[i] = Event.Random(t0, t1, matchPool, avgPlayersPerTeam, round.PlayersPerMatchType);
            }
            // Generate a break event if the amount of teams is odd.
            if (breakRound)
            {
                int t = Globals.Rand.Next(0, amountOfTeams);
                round.Events[amountOfEvents - 1] = new BreakEvent(t);
            }
            
            return round;
        }

        public Event GetRandomEvent()
        {
            return this.Events[Globals.Rand.Next(this.Events.Length)];
        }

        public bool ModifyPlayerAssignmentIfWithinLimit(SportsMatchCategory category, int modification)
        {
            int newAmount = PlayersPerMatchType[category] + modification;
            bool legalAssignment = newAmount <= Globals.MatchTypePlayerLimitsPerTeam[category];

            if (legalAssignment)
            {
                PlayersPerMatchType[category] = newAmount;
            }
            
            return legalAssignment;
        }

        /// <summary>
        /// Returns whether the given player amount modification to the given category is legal or not.
        /// </summary>
        /// <param name="category"></param>
        /// <param name="modification"></param>
        /// <returns></returns>
        public bool LegalPlayerAssignment(SportsMatchCategory category, int modification)
        {
            return PlayersPerMatchType[category] + modification <= Globals.MatchTypePlayerLimitsPerTeam[category];
        }

        /// <summary>
        /// Checks whether or not the tracked player counts per category are equal to the actual sum of player counts in
        /// this round's events.
        /// </summary>
        /// <returns></returns>
        public bool MatchTypePlayerCountsAreValid()
        {
            int actualCounts = 0;
            foreach (Event e in Events)
            {
                if (e == null) break;
                foreach (SportsMatch match in e.Matches)
                {
                    actualCounts += match.PlayersPerTeam;
                }
            }
            int trackedCounts = this.PlayersPerMatchType.Values.Sum();

            return actualCounts == trackedCounts;
        }
        
        public void ModifyPlayerAssignment(SportsMatchCategory category, int modification)
        {
            PlayersPerMatchType[category] += modification;
        }

        public (bool, Dictionary<SportsMatchCategory, int>) LegalEventSwap(Event old, Event @new)
        {
            var categoryCounts = new Dictionary<SportsMatchCategory, int>(PlayersPerMatchType);

            // Make all category count changes and check if the counts remain valid
            old.Matches.ForEach(m => categoryCounts[m.MatchType] -= m.PlayersPerTeam);
            // @new.Matches.ForEach(m => categoryCounts[m.MatchType] += m.PlayersPerTeam);
            foreach (SportsMatch match in @new.Matches)
            {
                categoryCounts[match.MatchType] += match.PlayersPerTeam;
            }
            
            bool legalSwap = true;
            foreach (KeyValuePair<SportsMatchCategory, int> pair in categoryCounts)
            {
                if (pair.Value > Globals.MatchTypePlayerLimitsPerTeam[pair.Key])
                {
                    legalSwap = false;
                    break;
                }
            }

            return (legalSwap, categoryCounts);
        }

        public bool LegalSportsMatchSwap(SportsMatch old, SportsMatch @new)
        {
            if (old.MatchType == @new.MatchType)
            {
                return LegalPlayerAssignment(old.MatchType, @new.PlayersPerTeam - old.PlayersPerTeam);
            }
            
            int oldCategoryNewAmount = PlayersPerMatchType[old.MatchType] - old.PlayersPerTeam;
            int newCategoryNewAmount = PlayersPerMatchType[@new.MatchType] + @new.PlayersPerTeam;

            return oldCategoryNewAmount <= Globals.MatchTypePlayerLimitsPerTeam[old.MatchType] &&
                   newCategoryNewAmount <= Globals.MatchTypePlayerLimitsPerTeam[@new.MatchType];
        }

        public override string ToString()
        {
            return Events.Select(e => e.ToString()).Aggregate((result, eventString) => $"{result}\n{eventString}");
        }

        public RoundPlanning Clone()
        {
            RoundPlanning clone = new RoundPlanning(this.Events.Length);
            for (int i = 0; i < this.Events.Length; i++)
            {
                clone.Events[i] = this.Events[i].Clone();
            }

            foreach (KeyValuePair<SportsMatchCategory, int> pair in this.PlayersPerMatchType)
            {
                clone.PlayersPerMatchType[pair.Key] = pair.Value;
            }

            return clone;
        }
    }
}