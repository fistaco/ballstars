using System.Collections.Generic;

namespace TeamBuilder.Entity.Individual
{
    internal class BallStarsSchedule : Individual
    {
        public RoundPlanning[] Rounds;
        
        public BallStarsSchedule(int timeSlotAmount)
        {
            this.Rounds = new RoundPlanning[timeSlotAmount];
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
    }
}