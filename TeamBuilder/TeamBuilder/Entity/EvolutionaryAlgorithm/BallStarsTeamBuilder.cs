using System;
using System.Collections.Generic;
using System.Linq;
using TeamBuilder.Entity.Individual;

namespace TeamBuilder.Entity.EvolutionaryAlgorithm
{
    class BallStarsTeamBuilder : EvolutionaryAlgorithm
    {
        private BallStarsTeamSet _initialTeams;

        private List<Player> _players = new List<Player>(); // TODO: initialise _players using given input file

        private readonly int _teamSize;

        public BallStarsTeamBuilder(string fileName, int teamSize)
        {
            _initialTeams = new BallStarsTeamSet(fileName);
            _teamSize = teamSize;
        }

        public override void Run()
        {
            // Construct n random solutions, which are permutations of one original random set
            List<Individual.Individual> population = InitRandomPopulation(65536);

            // Evolve over generations until a sufficiently good solution is found or time runs out.
            float bestFitness = _initialTeams.Evaluate();
            BallStarsTeamSet bestSolution = _initialTeams;
            while (bestFitness != 0f) // TODO: Include timer
            {
                // Create offspring using recombination and mutation so we'll have 2n individuals
                // Select best n individuals or do tournament select
                
                // Evaluate the selected individuals and update bestFitness
                foreach (var individual in population)
                {
                    float fitness = individual.Evaluate();
                    if (fitness < bestFitness)
                    {
                        bestFitness = fitness;
                        bestSolution = individual as BallStarsTeamSet;

                        Console.WriteLine($"New best fitness: {bestFitness}");

                        // TODO: Save the solution in a file
                        bestSolution.Print();
                        
                    }
                }
            }

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
