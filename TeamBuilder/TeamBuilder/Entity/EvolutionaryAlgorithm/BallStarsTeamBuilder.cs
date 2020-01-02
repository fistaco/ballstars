using System;
using System.Collections.Generic;

namespace TeamBuilder.Entity.EvolutionaryAlgorithm
{
    class BallStarsTeamBuilder : EvolutionaryAlgorithm
    {
        public override void Run()
        {
            // Construct n random solutions
            // Evolve over generations until a sufficiently good solution is found or time runs out.

            throw new NotImplementedException();
        }

        /// <summary>
        /// Constructs the desired amount of initial random team constructions and evaluates their fitness.
        /// </summary>
        /// <param name="amountOfIndividuals"></param>
        /// <returns></returns>
        protected override List<Individual.Individual> InitRandomPopulation(int amountOfIndividuals)
        {
            throw new NotImplementedException();
        }

        protected override void SelectSurvivors(List<Individual.Individual> population)
        {
            throw new NotImplementedException();
        }
    }
}
