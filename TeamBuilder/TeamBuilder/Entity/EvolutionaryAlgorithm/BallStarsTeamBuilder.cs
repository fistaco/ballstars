using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TeamBuilder.Entity.Individual;

namespace TeamBuilder.Entity.EvolutionaryAlgorithm
{
    class BallStarsTeamBuilder : EvolutionaryAlgorithm
    {
        private BallStarsTeamSet _initialTeams;
        private List<Player> _players;

        /// <summary>
        /// Array of names that link Player.ID integers to the actual player names.
        /// </summary>
        private string[] _playerNames;

        private readonly int _teamSize;

        private readonly string _outputFile;
        
        public BallStarsTeamBuilder(string filename, int teamSize, string outputFile)
        {
            _players = this.ParsePlayers(filename);
            
            _initialTeams = new BallStarsTeamSet(_players, teamSize);
            _teamSize = teamSize;
            _outputFile = outputFile;
        }

        public override void Run()
        {
            Console.WriteLine("Initiating random population of team sets...");
            // Construct n random solutions, which are permutations of one original random set
            List<BallStarsTeamSet> population = InitRandomPopulation(8192)
                .Select(indiv => indiv as BallStarsTeamSet).ToList();

            // Evolve over generations until a sufficiently good solution is found or time runs out.
            float bestFitness = _initialTeams.Evaluate();
            BallStarsTeamSet bestSolution = _initialTeams;
            int currentGen = 0;
            while (bestFitness != 0f && currentGen < 100) // TODO: Include timer if necessary
            {
                Console.WriteLine($"Commencing generation {currentGen}.");

                // Create offspring by randomly mutating the existing population
                var offspring = new List<BallStarsTeamSet>();
                foreach (var individual in population)
                {
                    BallStarsTeamSet clone = individual.Clone();
                    // BallStarsTeamSet clone = individual.CloneBySerialisation();
                    clone.Mutate();
                    offspring.Add(clone);
                }
                
                // Evaluate both the population and the offspring
                population.ForEach(teamSet => teamSet.Evaluate());
                offspring.ForEach(teamSet => teamSet.Evaluate());
                
                // Select the best n individuals out of the population + offspring
                population = NaiveSelection(population, offspring);

                // Update bestFitness if possible
                foreach (var individual in population)
                {
                    float fitness = individual.Fitness;
                    if (fitness < bestFitness)
                    {
                        bestFitness = fitness;
                        bestSolution = individual;

                        Console.WriteLine($"New best fitness: {bestFitness} (found in generation {currentGen})");
                        // bestSolution.Print(_playerNames); // TODO: uncomment for practical use
                    }
                }

                currentGen++;
            }

            // Save the best solution to a file
            bestSolution.SaveToCsv(_outputFile, _playerNames);
            Console.WriteLine($"Algorithm finished. Saving result to {_outputFile}.");
        }

        /// <summary>
        /// Constructs a list of players given a correctly formatted CSV file.
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        private List<Player> ParsePlayers(string filename)
        {
            List<Player> players = new List<Player>();
            
            // Assume the first line contains the column names
            string[] lines = File.ReadAllLines(filename);
            _playerNames = new string[lines.Length - 1];
            for (int i = 1; i < lines.Length; i++)
            {
                // Assume fields are in the following order: First name, Last name, Gender, Sport.
                string[] fields = lines[i].Split(",");
                int id = i - 1;
                _playerNames[id] = $"{fields[0]} {fields[1]}";
                players.Add(new Player(id, fields[2], fields[3], false));
            }

            return players;
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
                population.Add(new BallStarsTeamSet(_players, _teamSize));
            }

            return population;
        }

        protected override void SelectSurvivors(List<Individual.Individual> population)
        {
            throw new NotImplementedException();
        }

        private List<BallStarsTeamSet> NaiveSelection(List<BallStarsTeamSet> population,
            List<BallStarsTeamSet> offspring)
        {
            // Sort the combined P+O list and return the top n
            population.AddRange(offspring);
            return population.OrderBy(indiv => indiv.Fitness).Take(offspring.Count).ToList();
        }
    }
}
