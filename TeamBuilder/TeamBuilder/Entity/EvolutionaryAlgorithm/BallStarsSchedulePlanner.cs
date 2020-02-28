using System;
using System.Collections.Generic;

namespace TeamBuilder.Entity.EvolutionaryAlgorithm
{
    internal class BallStarsSchedulePlanner : EvolutionaryAlgorithm
    {
        private int _amountOfRounds;
        
        private readonly string[] _teamNames;
        private int _maxPlayersPerTeam;

        public BallStarsSchedulePlanner(int amountOfRounds, string[] teamNames, int maxPlayersPerTeam)
        {
            _amountOfRounds = amountOfRounds;
            _teamNames = teamNames;
            _maxPlayersPerTeam = maxPlayersPerTeam;
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