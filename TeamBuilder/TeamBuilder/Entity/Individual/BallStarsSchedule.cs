using System.Collections.Generic;

namespace TeamBuilder.Entity.Individual
{
    internal class BallStarsSchedule : Individual
    {
        public RoundPlanning[] Rounds;
        
        /// <summary>
        /// Constructs an empty BallStarsSchedule with an initialised, but undefined array of a given number of rounds.
        /// </summary>
        /// <param name="amountOfRounds"></param>
        public BallStarsSchedule(int amountOfRounds)
        {
            this.Rounds = new RoundPlanning[amountOfRounds];
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
            var schedule = new BallStarsSchedule(amountOfRounds);

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
            throw new System.NotImplementedException();
        }

        public override Individual Crossover(Individual other)
        {
            throw new System.NotImplementedException();
        }

        public override void Mutate()
        {
            throw new System.NotImplementedException();
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}