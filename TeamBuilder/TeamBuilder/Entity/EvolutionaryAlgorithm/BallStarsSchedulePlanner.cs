using System;
using System.Collections.Generic;
using System.Linq;
using TeamBuilder.Entity.Individual;

namespace TeamBuilder.Entity.EvolutionaryAlgorithm
{
    internal class BallStarsSchedulePlanner : EvolutionaryAlgorithm
    {
        private readonly int _amountOfRounds;
        
        private readonly string[] _teamNames;
        private readonly int _amountOfTeams;
        private readonly int _avgPlayersPerTeam;

        private readonly List<SportsMatch> _matchPool;

        private readonly string _outputFile;

        public BallStarsSchedulePlanner(int amountOfRounds, string[] teamNames, int avgPlayersPerTeam,
            string outputFile)
        {
            _amountOfRounds = amountOfRounds;
            _teamNames = teamNames;
            _amountOfTeams = teamNames.Length;
            _avgPlayersPerTeam = avgPlayersPerTeam;

            _matchPool = this.InitialiseMatchPool();

            _outputFile = outputFile;
        }

        public override void Run()
        {
            Console.WriteLine("Initiating random population of schedules...");
            // Construct n random solutions, which are permutations of one original random set
            List<BallStarsSchedule> population = InitRandomPopulation(8192)
                .Select(indiv => indiv as BallStarsSchedule).ToList();

            // Evolve over generations until a sufficiently good solution is found or time runs out.
            BallStarsSchedule bestSolution = population[0];
            float bestFitness = bestSolution.Evaluate();
            int currentGen = 0;
            while (bestFitness != 0f && currentGen < 100) // TODO: Include timer if necessary
            {
                Console.WriteLine($"Commencing generation {currentGen}.");

                Console.WriteLine("Cloning...");
                // Create offspring by applying crossover to the existing population
                var offspring = new List<BallStarsSchedule>();
                foreach (var individual in population)
                {
                    // TODO: Use crossover to create offspring instead of cloning
                    BallStarsSchedule clone = individual.Clone();
                    offspring.Add(clone);
                }
                Console.WriteLine("Mutating...");
                // Mutate the offspring
                foreach (var schedule in offspring)
                {
                    if (Globals.Rand.NextDouble() < 0.5)
                    {
                        schedule.AddSportsMatchFromPool(_matchPool);
                    }
                    schedule.Mutate();
                }
                
                Console.WriteLine("Evaluating...");
                // Evaluate both the population and the offspring
                population.ForEach(schedule => schedule.Evaluate());
                offspring.ForEach(schedule => schedule.Evaluate());
                
                Console.WriteLine("Selecting best individuals...");
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
                        // bestSolution.Print(_playerNames);
                    }
                }

                currentGen++;
            }

            // // Save the best solution to a file // TODO
            // bestSolution.SaveToCsv(_outputFile);
            // Console.WriteLine($"Algorithm finished. Saving result to {_outputFile}.");
            Console.WriteLine($"Algorithm finished. The best schedule is as follows:\n{bestSolution}");
        }

        /// <summary>
        /// Generates a list of team match-ups where each team plays against each other team exactly once.
        /// </summary>
        /// <returns></returns>
        private HashSet<Tuple<int, int>> GenerateMinimalTeamMatchUps(int amountOfTeams)
        {
            var matchUps = new HashSet<Tuple<int, int>>();
            
            // Iterate over all pairs of team IDs and add all pairs except for duplicates.
            for (int t0 = 0; t0 < amountOfTeams; t0++)
            {
                for (int t1 = 1; t1 < amountOfTeams; t1++)
                {
                    var mu = new Tuple<int, int>(t0, t1);
                    if (t0 == t1 || matchUps.Contains(mu))
                    {
                        continue;
                    }

                    matchUps.Add(mu);
                }
            }

            return matchUps;
        }
        
        private List<SportsMatch> InitialiseMatchPool()
        {
            int badmintonMin = 1;
            int badmintonDoublesMin = 2;
            int basketballMin = 5;
            int floorballMin = 4;
            int tableTennisMin = 1;
            int tableTennisDoublesMin = 2;
            // int squashMin = 1;
            // int volleyballMin = 6;
            int korfballMin = 4;
            
            int badmintonMax = 2;
            int badmintonDoublesMax = 4;
            int basketballMax = 6;
            int floorballMax = 6;
            int tableTennisMax = 3;
            int tableTennisDoublesMax = 4;
            // int squashMax = 3;
            // int volleyballMax = 7;
            int korfballMax = 6;

            // Each match has a category and an amount of players to be allotted per team
            return new List<SportsMatch>()
            {
                new SportsMatch(SportsMatchCategory.Badminton, 1, badmintonMin, badmintonMax),
                new SportsMatch(SportsMatchCategory.Badminton, 2, badmintonMin, badmintonMax),
                new SportsMatch(SportsMatchCategory.BadmintonDoubles, 2, badmintonDoublesMin, badmintonDoublesMax),
                new SportsMatch(SportsMatchCategory.Basketball, 5, basketballMin, basketballMax, true),
                new SportsMatch(SportsMatchCategory.Basketball, 6, basketballMin, basketballMax, true), // 1 reserve
                new SportsMatch(SportsMatchCategory.Floorball, 4, floorballMin, floorballMax, true),
                new SportsMatch(SportsMatchCategory.Floorball, 5, floorballMin, floorballMax, true),
                new SportsMatch(SportsMatchCategory.Korfball, 4, korfballMin, korfballMax, true),
                // new SportsMatch(SportsMatchCategory.Korfball, 6, korfballMin, korfballMax, true),
                // new SportsMatch(SportsMatchCategory.Korfball, 8, korfballMin, korfballMax, true),
                // new SportsMatch(SportsMatchCategory.Squash, 1, squashMin, squashMax),
                // new SportsMatch(SportsMatchCategory.Squash, 2, squashMin, squashMax),
                // new SportsMatch(SportsMatchCategory.Squash, 3, squashMin, squashMax),
                // new SportsMatch(SportsMatchCategory.TableTennis, 1),
                new SportsMatch(SportsMatchCategory.TableTennis, 2, tableTennisMin, tableTennisMax),
                new SportsMatch(SportsMatchCategory.TableTennis, 3, tableTennisMin, tableTennisMax),
                new SportsMatch(SportsMatchCategory.TableTennisDoubles, 2, tableTennisDoublesMin, tableTennisDoublesMax),
                // new SportsMatch(SportsMatchCategory.Volleyball, 6, volleyballMin, volleyballMax),
                // new SportsMatch(SportsMatchCategory.Referee, 1, 1, 1)
            };
        }

        protected override List<Individual.Individual> InitRandomPopulation(int amountOfIndividuals)
        {
            var population = new List<Individual.Individual>();
            
            // If there is an odd amount of teams, add a break event to every round.
            bool addBreakRound = _amountOfTeams % 2 == 1;
            int eventsPerRound = addBreakRound ? _amountOfTeams / 2 + 1 : _amountOfTeams / 2;
            int regularEventsPerRound = addBreakRound ? eventsPerRound - 1 : eventsPerRound;
            
            Console.WriteLine($"-> Creating {amountOfIndividuals} schedules with {_amountOfTeams} teams, " +
                              $"{_amountOfRounds} rounds, and {eventsPerRound} events per round.");
            for (int i = 0; i < amountOfIndividuals; i++)
            {
                population.Add(BallStarsSchedule.Random(
                    _amountOfTeams, eventsPerRound, regularEventsPerRound, _amountOfRounds, _matchPool, addBreakRound,
                    _avgPlayersPerTeam
                ));
            }

            return population;
        }

        protected override void SelectSurvivors(List<Individual.Individual> population)
        {
            throw new System.NotImplementedException();
        }
        
        private List<BallStarsSchedule> NaiveSelection(List<BallStarsSchedule> population,
            List<BallStarsSchedule> offspring)
        {
            // Sort the combined P+O list and return the top n
            population.AddRange(offspring);
            return population.OrderBy(indiv => indiv.Fitness).Take(offspring.Count).ToList();
        }
    }
}