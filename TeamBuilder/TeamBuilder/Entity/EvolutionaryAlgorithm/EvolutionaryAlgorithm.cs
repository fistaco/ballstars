using System.Collections.Generic;

namespace TeamBuilder.Entity.EvolutionaryAlgorithm
{
    internal abstract class EvolutionaryAlgorithm
    {
        /// <summary>
        /// Initialises random individuals and evolves them over generations until some termination criterion is met.
        /// </summary>
        public abstract void Run();

        protected abstract List<Individual.Individual> InitRandomPopulation(int amountOfIndividuals);

        protected abstract void SelectSurvivors(List<Individual.Individual> population);
    }
}
