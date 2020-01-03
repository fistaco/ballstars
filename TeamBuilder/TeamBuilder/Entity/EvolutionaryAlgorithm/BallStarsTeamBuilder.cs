using System;
using System.Collections.Generic;
using System.Linq;
using TeamBuilder.Entity.Individual;

namespace TeamBuilder.Entity.EvolutionaryAlgorithm
{
    class BallStarsTeamBuilder : EvolutionaryAlgorithm
    {
        public override void Run()
        {
            // Construct n random solutions, which are permutations of one original random set
            List<Individual.Individual> population = InitRandomPopulation(65536);

            // Evolve over generations until a sufficiently good solution is found or time runs out.

            throw new NotImplementedException();
        }

        /// <summary>
        /// Constructs the desired amount of initial random team permutations and evaluates their fitness.
        /// </summary>
        /// <param name="amountOfIndividuals"></param>
        /// <returns></returns>
        protected override List<Individual.Individual> InitRandomPopulation(int amountOfIndividuals)
        {
            var population = new List<Individual.Individual>();
            for (int i = 0; i < amountOfIndividuals; i++)
            {
                population.Add(new BallStarsTeamSet(_players, 8));
            }

            return population;
        }

        protected override void SelectSurvivors(List<Individual.Individual> population)
        {
            throw new NotImplementedException();
        }
    }
}
