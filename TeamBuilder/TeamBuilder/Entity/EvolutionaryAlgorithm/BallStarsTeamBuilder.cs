﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TeamBuilder.Entity.Individual;

namespace TeamBuilder.Entity.EvolutionaryAlgorithm
{
    class BallStarsTeamBuilder : EvolutionaryAlgorithm
    {
        private BallStarsTeamSet _initialTeams;

        private List<Player> _players = new List<Player>(); // TODO: initialise _players using given input file

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
            // Construct n random solutions, which are permutations of one original random set
            Console.WriteLine("Initiating random population of team sets...");
            List<BallStarsTeamSet> population = InitRandomPopulation(65536)
                .Select(indiv => indiv as BallStarsTeamSet).ToList();

            // Evolve over generations until a sufficiently good solution is found or time runs out.
            float bestFitness = _initialTeams.Evaluate();
            BallStarsTeamSet bestSolution = _initialTeams;
            int currentGen = 0;
            Console.WriteLine($"Starting evolutionary algorithm at generation 0...");
            while (bestFitness != 0f && currentGen < 10000) // TODO: Include timer if necessary
            {
                // Create offspring by randomly mutating the existing population
                var offspring = new List<BallStarsTeamSet>();
                foreach (var individual in population)
                {
                    BallStarsTeamSet clone = individual.Clone();
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
                        bestSolution = individual as BallStarsTeamSet;

                        Console.WriteLine($"New best fitness: {bestFitness} (found in generation {currentGen}");
                        bestSolution.Print();
                    }
                }

                currentGen++;
            }

            // Save the best solution to a file
            bestSolution.SaveToCsv(_outputFile);
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
            for (int i = 1; i < lines.Length; i++)
            {
                // Assume fields are in the following order: First name, Last name, Gender, Sport
                string[] fields = lines[i].Split(",");
                players.Add(new Player($"{fields[0]} {fields[1]}", fields[2], fields[3], false));
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
                population.Add(new BallStarsTeamSet(_players, 8));
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
