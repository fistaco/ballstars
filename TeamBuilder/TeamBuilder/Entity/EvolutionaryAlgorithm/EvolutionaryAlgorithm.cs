using System.Collections.Generic;
using TeamBuilder.Entity.Individual;

namespace TeamBuilder.Entity.EvolutionaryAlgorithm
{
    internal abstract class EvolutionaryAlgorithm
    {
        /// <summary>
        /// Initializes random individuals and evolves them over generations until some termination criterion is met.
        /// </summary>
        public abstract void Run();

        protected abstract List<Individual.Individual> InitRandomPopulation();

        protected abstract void SelectSurvivors(List<Individual.Individual> population);
    }
}
