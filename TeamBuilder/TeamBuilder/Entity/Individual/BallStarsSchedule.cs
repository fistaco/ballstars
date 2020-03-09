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
            Event eventOne = this.GetRandomEvent();
            int i1 = Globals.Rand.Next(eventOne.Matches.Count);
            SportsMatch matchOne = eventOne.Matches[i1];
            Event eventTwo = this.GetRandomEvent();
            int i2 = Globals.Rand.Next(eventTwo.Matches.Count);
            SportsMatch matchTwo = eventTwo.Matches[i2];
            
            // Only swap if the two matches have the same amount of players
            if (matchOne.PlayersPerTeam != matchTwo.PlayersPerTeam)
            {
                return;
            }

            // Swap the SportsMatch objects
            eventOne.Matches[i1] = eventTwo.Matches[i2];
            eventTwo.Matches[i2] = matchOne;
            
            // Update stats
            // Remove the current SportsMatches
            this.RemoveCategoryFromEventTeamStats(eventOne, matchOne.MatchType);
            this.RemoveCategoryFromEventTeamStats(eventTwo, matchTwo.MatchType);
            // Add new SportsMatches
            this.AddCategoryToEventTeamStats(eventOne, matchTwo.MatchType);
            this.AddCategoryToEventTeamStats(eventTwo, matchOne.MatchType);
        }

        private void SwapEvents()
        {
            RoundPlanning r0 = this.GetRandomRound();
            int i0 = Globals.Rand.Next(r0.Events.Length);
            Event e0 = r0.Events[i0];
            RoundPlanning r1 = this.GetRandomRound();
            int i1 = Globals.Rand.Next(r1.Events.Length);

            r0.Events[i0] = r1.Events[i1];
            r1.Events[i1] = e0;
        }

        private void AddSportsMatch(SportsMatch match)
        {
            
        }

        private void RemoveSportsMatch()
        {
            Event e = this.GetRandomEvent();
            int i = Globals.Rand.Next(e.Matches.Count);

            this.RemoveCategoryFromEventTeamStats(e, e.Matches[i].MatchType);
            e.Matches[i] = null;
        }

        private void ModifyRandomSportsMatchPlayerAmount(int modification)
        {
            Event evnt = this.GetRandomEvent();
            SportsMatch match = evnt.GetRandomSportsMatch();
            evnt.Matches.Add(match);
            match.PlayersPerTeam -= modification;
            
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
            Event evnt = this.GetRandomEvent();
            var match = SportsMatch.Random(matchPool);
            evnt.Matches.Add(match);
            
            // Update the involved teams' statistics
            _teamStats[evnt.TeamOneId].AddSportsCategoryPlayed(match.MatchType);
            _teamStats[evnt.TeamTwoId].AddSportsCategoryPlayed(match.MatchType);
        }

        private RoundPlanning GetRandomRound()
        {
            return this.Rounds[Globals.Rand.Next(this.Rounds.Length)];
        }

        private Event GetRandomEvent()
        {
            return this.GetRandomRound().GetRandomEvent();
        }

        private SportsMatch GetRandomSportsMatch()
        {
            return this.GetRandomRound().GetRandomEvent().GetRandomSportsMatch();
        }

        #region statusUpdates

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