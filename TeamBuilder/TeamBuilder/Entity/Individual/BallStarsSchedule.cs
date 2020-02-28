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
        /// compete against each other in random events without exceeding the player limit per team.
        /// </summary>
        /// <param name="amountOfRounds"></param>
        /// <param name="playersPerTeam"></param>
        public static BallStarsSchedule Random(int amountOfRounds, int playersPerTeam)
        {
            var schedule = new BallStarsSchedule(amountOfRounds);
            
            // TODO: Create random rounds
            
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