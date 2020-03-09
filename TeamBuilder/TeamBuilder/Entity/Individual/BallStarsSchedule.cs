using System;
using System.Collections.Generic;
using System.Linq;

namespace TeamBuilder.Entity.Individual
{
    internal class BallStarsSchedule : Individual
    {
        public RoundPlanning[] Rounds;
        
        private const int MediumEvalPenalty = 300;
        private const int HeavyEvalPenalty = 999;

        /// <summary>
        /// Tracks each team's statistics for efficient usage during fitness evaluation.
        /// </summary>
        private ScheduleTeamStatistics[] _teamStats; // TODO: Update team stats after random schedule generation


        /// <summary>
        /// Constructs an empty BallStarsSchedule with an initialised, but undefined array of a given number of rounds.
        /// </summary>
        /// <param name="amountOfRounds"></param>
        /// <param name="amountOfTeams"></param>
        public BallStarsSchedule(int amountOfRounds, int amountOfTeams)
        {
            this.Rounds = new RoundPlanning[amountOfRounds];
            
            _teamStats = new ScheduleTeamStatistics[amountOfTeams];
            for (int i = 0; i < amountOfTeams; i++)
            {
                _teamStats[i] = new ScheduleTeamStatistics(amountOfTeams, amountOfRounds);
            }
        }

        /// <summary>
        /// Constructs a random schedule consisting of a given amount of rounds where teams are randomly scheduled to
        /// compete against each other in random events.
        /// </summary>
        /// <param name="amountOfTeams"></param>
        /// <param name="amountOfEvents"></param>
        /// <param name="amountOfRegularEvents"></param>
        /// <param name="amountOfRounds"></param>
        /// <param name="matchPool"></param>
        /// <param name="breakRound"></param>
        public static BallStarsSchedule Random(int amountOfTeams, int amountOfEvents, int amountOfRegularEvents,
            int amountOfRounds, List<SportsMatch> matchPool, bool breakRound)
        {
            var schedule = new BallStarsSchedule(amountOfRounds, amountOfTeams);

            // TODO: Incorporate maxPlayersPerTeam instead of leaving it to the evaluation method
            for (int i = 0; i < amountOfRounds; i++)
            {
                schedule.Rounds[i] = RoundPlanning.Random(amountOfTeams, amountOfEvents, amountOfRegularEvents,
                    matchPool, breakRound);
            }
            
            return schedule;
        }

        public override float Evaluate()
        {
            int fitness = 7;

            this.Fitness = fitness;
            return fitness;
        }

        public override Individual Crossover(Individual other)
        {
            throw new System.NotImplementedException();
        }
        
        public override void Mutate()
        {
            throw new System.NotImplementedException();
            // With some probability, apply one of the following mutations:
            // Add a random SportsMatch from the pool // Has to be done from the main loop
            // Remove a random SportsMatch from the schedule
            // Swap 2 random SportsMatch objects with identical player amounts within the schedule
            // Swap 2 random Event objects between rounds
            // For some SportsMatch, increment or decrement its player count by 1 // TODO: Implement player limits
        }

        private void SwapSportsMatches()
        {
            // Generate indices and get SportsMatch objects
            (Event e0, int r0) = this.GetRandomEventWithRoundIndex();
            int m0Index = Globals.Rand.Next(e0.Matches.Count);
            SportsMatch m0 = e0.Matches[m0Index];
            (Event e1, int r1) = this.GetRandomEventWithRoundIndex();
            int m1Index = Globals.Rand.Next(e1.Matches.Count);
            SportsMatch m1 = e1.Matches[m1Index];
            
            // Only swap if the two matches have the same amount of players
            if (m0.PlayersPerTeam != m1.PlayersPerTeam)
            {
                return;
            }

            // Swap the SportsMatch objects
            e0.Matches[m0Index] = m1;
            e1.Matches[m1Index] = m0;
            
            // Update stats
            // Remove the current SportsMatches
            this.UpdateEventTeamStatsAfterSportsMatchRemoval(r0, e0, m0);
            this.UpdateEventTeamStatsAfterSportsMatchRemoval(r1, e1, m1);
            // Add new SportsMatches
            this.UpdateEventTeamStatsAfterSportsMatchAddition(r0, e0, m1);
            this.UpdateEventTeamStatsAfterSportsMatchAddition(r1, e1, m0);
        }

        private void SwapEvents()
        {
            // Get the required variables to swap an event from r0 with an event from r1
            int r0Index = Globals.Rand.Next(this.Rounds.Length);
            RoundPlanning r0 = this.Rounds[r0Index];
            int e0Index = Globals.Rand.Next(r0.Events.Length);
            Event e0 = r0.Events[e0Index];

            int r1Index = Globals.Rand.Next(this.Rounds.Length);
            RoundPlanning r1 = this.Rounds[r1Index];
            int e1Index = Globals.Rand.Next(r1.Events.Length);
            Event e1 = r1.Events[e1Index];

            // Swap
            r0.Events[e0Index] = e1;
            r1.Events[e1Index] = e0;
            
            // Update relevant team stats, i.e. each team's events played per round.
            this.ModifyEventTeamsEventsPerRound(e0, r0Index, -1);
            this.ModifyEventTeamsEventsPerRound(e0, r1Index, 1);
            this.ModifyEventTeamsEventsPerRound(e1, r1Index, -1);
            this.ModifyEventTeamsEventsPerRound(e1, r0Index, 1);
        }

        private void AddSportsMatchWithPlayerAmount(int playerAmount)
        {
            throw new NotImplementedException();
        }

        private void RemoveSportsMatchWithPlayerAmount(int playerAmount)
        {
            throw new NotImplementedException();
        }

        private void RemoveSportsMatch()
        {
            (Event e, int roundIndex) = this.GetRandomEventWithRoundIndex();
            int matchIndex = Globals.Rand.Next(e.Matches.Count);
            
            this.UpdateEventTeamStatsAfterSportsMatchRemoval(roundIndex, e, e.Matches[matchIndex]);
            e.Matches.RemoveAt(matchIndex);
        }

        private void ModifyRandomSportsMatchPlayerAmount(int modification)
        {
            (Event evnt, int roundIndex) = this.GetRandomEventWithRoundIndex(); 
            
            SportsMatch match = evnt.GetRandomSportsMatch();
            evnt.Matches.Add(match);
            match.PlayersPerTeam += modification;
            
            this.ModifyEventTeamStatsPlayerCounts(roundIndex, evnt, modification);
        }

        private void DecrementRandomSportsMatchPlayerAmount()
        {
            this.ModifyRandomSportsMatchPlayerAmount(-1);
        }
        
        private void IncrementRandomSportsMatchPlayerAmount()
        {
            this.ModifyRandomSportsMatchPlayerAmount(1);
        }

        public void AddSportsMatchFromPool(List<SportsMatch> matchPool)
        {
            // Add a random SportsMatch from the pool to a random event in the schedule
            (Event evnt, int roundIndex) = this.GetRandomEventWithRoundIndex();
            var match = SportsMatch.Random(matchPool);
            evnt.Matches.Add(match);
            
            // Update the involved teams' statistics
            this.UpdateEventTeamStatsAfterSportsMatchAddition(roundIndex, evnt, match);
        }

        private RoundPlanning GetRandomRound()
        {
            return this.Rounds[Globals.Rand.Next(this.Rounds.Length)];
        }

        private Event GetRandomEvent()
        {
            return this.GetRandomRound().GetRandomEvent();
        }

        private (Event, int) GetRandomEventWithRoundIndex()
        {
            int i = Globals.Rand.Next(this.Rounds.Length);
            Event e = this.Rounds[i].GetRandomEvent();

            return (e, i);
        }

        private SportsMatch GetRandomSportsMatch()
        {
            return this.GetRandomRound().GetRandomEvent().GetRandomSportsMatch();
        }

        #region statusUpdates

        /// <summary>
        /// Updates the round player counts and sports played for both teams of a given event based on the addition of a
        /// given SportsMatch.
        /// </summary>
        /// <param name="roundIndex">The round in which the SportsMatch was added.</param>
        /// <param name="evnt">The event to which the SportsMatch was added.</param>
        /// <param name="match">The SportsMatch that was just added to the given event in the given round.</param>
        private void UpdateEventTeamStatsAfterSportsMatchAddition(int roundIndex, Event evnt, SportsMatch match)
        {
            _teamStats[evnt.TeamOneId].UpdateAfterSportsMatchAddition(match, roundIndex);
            _teamStats[evnt.TeamTwoId].UpdateAfterSportsMatchAddition(match, roundIndex);
        }
        
        /// <summary>
        /// Updates the round player counts and sports played for both teams of a given event based on the removal of a
        /// given SportsMatch.
        /// </summary>
        /// <param name="roundIndex">The round in which the SportsMatch was removed.</param>
        /// <param name="evnt">The event to which the SportsMatch was removed.</param>
        /// <param name="match">The SportsMatch that was just removed from the given event in the given round.</param>
        private void UpdateEventTeamStatsAfterSportsMatchRemoval(int roundIndex, Event evnt, SportsMatch match)
        {
            _teamStats[evnt.TeamOneId].UpdateAfterSportsMatchRemoval(match, roundIndex);
            _teamStats[evnt.TeamTwoId].UpdateAfterSportsMatchRemoval(match, roundIndex);
        }

        /// <summary>
        /// Adds or removes a given amount to the count of events of a given round for both teams of a given event.
        /// </summary>
        /// <param name="evnt">The event of which the teams' statistics will be updated.</param>
        /// <param name="roundIndex">The round in which an event was just added or removed.</param>
        /// <param name="modification">The (possibly negative) amount of events added to the given round.</param>
        private void ModifyEventTeamsEventsPerRound(Event evnt, int roundIndex, int modification)
        {
            _teamStats[evnt.TeamOneId].EventsPerRound[roundIndex] += modification;
            _teamStats[evnt.TeamTwoId].EventsPerRound[roundIndex] += modification;
        }

        private void AddCategoryToEventTeamStats(Event evnt, SportsMatchCategory category)
        {
            _teamStats[evnt.TeamOneId].AddSportsCategoryPlayed(category);
            _teamStats[evnt.TeamTwoId].AddSportsCategoryPlayed(category);
        }
        
        private void RemoveCategoryFromEventTeamStats(Event evnt, SportsMatchCategory category)
        {
            _teamStats[evnt.TeamOneId].RemoveSportsCategoryPlayed(category);
            _teamStats[evnt.TeamTwoId].RemoveSportsCategoryPlayed(category);
        }

        private void ModifyEventTeamStatsPlayerCounts(int roundIndex, Event evnt, int modification)
        {
            _teamStats[evnt.TeamOneId].RoundPlayerCounts[roundIndex] += modification;
            _teamStats[evnt.TeamTwoId].RoundPlayerCounts[roundIndex] += modification;
        }
        
        #endregion

        public override string ToString()
        {
            return base.ToString();
        }

        public void SaveToCsv(string outputFile)
        {
            throw new NotImplementedException();
        }
    }
}