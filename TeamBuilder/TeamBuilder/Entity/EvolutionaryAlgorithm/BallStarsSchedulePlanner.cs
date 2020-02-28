using System.Collections.Generic;

namespace TeamBuilder.Entity.EvolutionaryAlgorithm
{
    internal class BallStarsSchedulePlanner : EvolutionaryAlgorithm
    {
        private int _amountOfRounds;
        
        private readonly string[] _teamNames;

        public BallStarsSchedulePlanner(int amountOfRounds, string[] teamNames)
        {
            _amountOfRounds = amountOfRounds;
            _teamNames = teamNames;
        }
        
        public override void Run()
        {
            throw new System.NotImplementedException();
        }

        protected override List<Individual.Individual> InitRandomPopulation(int amountOfIndividuals)
        {
            throw new System.NotImplementedException();
        }

        protected override void SelectSurvivors(List<Individual.Individual> population)
        {
            throw new System.NotImplementedException();
        }
    }
}