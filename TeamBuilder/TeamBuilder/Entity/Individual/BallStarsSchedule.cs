using System;
using System.Collections.Generic;

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

        public void AddSportsMatchFromPool(List<SportsMatch> matchPool)
        {
            // Add a random SportsMatch from the pool to a random event in the schedule
            Event evnt = this.GetRandomRound().GetRandomEvent();
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

        private SportsMatch GetRandomSportsMatch()
        {
            return this.GetRandomRound().GetRandomEvent().GetRandomSportsMatch();
        }

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