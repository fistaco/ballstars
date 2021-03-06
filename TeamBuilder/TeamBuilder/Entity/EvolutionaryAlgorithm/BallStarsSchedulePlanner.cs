using System;
using System.Collections.Generic;
using System.Linq;
using TeamBuilder.Entity.Individual;

namespace TeamBuilder.Entity.EvolutionaryAlgorithm
{
    internal class BallStarsSchedulePlanner : EvolutionaryAlgorithm
    {
        private readonly int _amountOfRounds;
        private readonly int _roundCrossoverCutoff;
        
        private readonly string[] _teamNames;
        private readonly int _amountOfTeams;
        private readonly int _avgPlayersPerTeam;

        private readonly List<SportsMatch> _matchPool;

        private readonly string _outputFile;

        private readonly List<Tuple<int, int>> _predefinedMatchUps;

        private readonly bool _useLocalSearch;

        private readonly bool _useCrossover;

        public BallStarsSchedulePlanner(int amountOfRounds, int amountOfTeams, int avgPlayersPerTeam,
            string outputFile, bool usePredefinedMatchUps = false, bool useLocalSearch = false,
            bool useCrossover = false, string[] teamNames = null)
        {
            _amountOfRounds = amountOfRounds;
            _roundCrossoverCutoff = amountOfRounds / 4;
            _teamNames = teamNames;
            _amountOfTeams = amountOfTeams;
            _avgPlayersPerTeam = avgPlayersPerTeam;

            _matchPool = this.InitialiseMatchPool();

            _outputFile = outputFile;

            _predefinedMatchUps = null;
            if (usePredefinedMatchUps)
            {
                _predefinedMatchUps = GenerateMinimalTeamMatchUps();
            }

            _useLocalSearch = useLocalSearch;
            _useCrossover = useCrossover;
        }

        public override void Run()
        {
            int n = 4096;
            Console.WriteLine($"Starting algorithm with {n} individuals, {_amountOfRounds} rounds, " +
                              $"{_amountOfTeams} teams, and {_avgPlayersPerTeam} players per team.");

            Console.WriteLine("Initiating random population of schedules...");
            // Construct n random solutions, which are permutations of one original random set
            List<BallStarsSchedule> population = InitRandomPopulation(n)
                .Select(indiv => indiv as BallStarsSchedule).ToList();

            // Evolve over generations until a sufficiently good solution is found or time runs out.
            BallStarsSchedule bestSolution = population[0];
            float bestFitness = bestSolution.Evaluate();
            int currentGen = 0;
            while (bestFitness != 0f && currentGen < 1000) // TODO: Include timer if necessary
            {
                Console.WriteLine($"Commencing generation {currentGen}.");

                // Create offspring by applying crossover to the existing population
                var offspring = new List<BallStarsSchedule>();

                if (_useLocalSearch)
                {
                    // Local search
                    for (int i = 0; i < population.Count; i++)
                    {
                        BallStarsSchedule clone = bestSolution.Clone();
                        offspring.Add(clone);
                    }
                }
                else if (_useCrossover)
                {
                    // Apply single-point crossover at a given cutoff point
                    for (int i = 0; i < population.Count; i += 2)
                    {
                        (BallStarsSchedule o0, BallStarsSchedule o1) =
                            population[i].Crossover(population[i + 1], _roundCrossoverCutoff);
                        offspring.Add(o0);
                        offspring.Add(o1);
                    }
                }
                else
                {
                    // Use cloning
                    foreach (var individual in population)
                    {
                        BallStarsSchedule clone = individual.Clone();
                        offspring.Add(clone);
                    }
                }

                // Mutate the offspring
                foreach (var schedule in offspring)
                {
                    if (Globals.Rand.NextDouble() < 0.8)
                    {
                        schedule.AddSportsMatchFromPool(_matchPool);
                    }
                    // schedule.GranularMutate();
                    schedule.Mutate();
                }
                
                // Evaluate both the population and the offspring
                population.ForEach(schedule => schedule.Evaluate());
                offspring.ForEach(schedule => schedule.Evaluate());
                
                // Select the best n individuals out of the population + offspring
                // population = NaiveSelection(population, offspring);
                population = TournamentSelection(population, offspring, 4);

                // Update bestFitness if possible
                foreach (var individual in population)
                {
                    float fitness = individual.Fitness;
                    if (fitness < bestFitness)
                    {
                        bestFitness = fitness;
                        bestSolution = individual;

                        Console.WriteLine($"New best fitness: {bestFitness} (found in generation {currentGen})");
                    }
                }

                currentGen++;
            }

            // Save the best solution to a file
            bestSolution.SaveToCsv(_outputFile);
            Console.WriteLine($"Algorithm finished. The best schedule is as follows:\n{bestSolution}");
            Console.WriteLine($"The final schedule has been saved to {_outputFile}.");
        }

        public void RunSimulatedAnnealing(double initialTemperature = 10, double alpha = 0.9999)
        {
            // Start off with a randomised schedule for an even amount of teams
            int eventsPerRound = _amountOfTeams / 2;
            BallStarsSchedule currentSchedule = BallStarsSchedule.Random(
                _amountOfTeams, eventsPerRound, eventsPerRound, _amountOfRounds, _matchPool, false,
                _avgPlayersPerTeam, _predefinedMatchUps
            );
            BallStarsSchedule bestSchedule = currentSchedule;
            double bestFitness = currentSchedule.Evaluate();
            double currentScheduleFitness = bestFitness;

            double minimumTemp = 0.000000001;
            double temperature = initialTemperature;
            int iters = 0;
            while (iters < 100000)
            {
                if (iters % 1000 == 0) { Console.WriteLine($"Running iteration {iters}."); }
                
                // Pick a neighbour by cloning and mutating
                BallStarsSchedule neighbour = currentSchedule.Clone();
                neighbour.LocalSearchMutate(_matchPool);
                neighbour.Evaluate();
                
                // Always accept better solutions
                if (neighbour.Fitness < currentScheduleFitness)
                {
                    // Update currently tracking schedule and fitness
                    currentScheduleFitness = neighbour.Fitness;
                    currentSchedule = neighbour;

                    // Update best found schedule and fitness if they improved
                    if (currentScheduleFitness < bestFitness)
                    {
                        bestFitness = currentScheduleFitness;
                        bestSchedule = currentSchedule.Clone();
                    
                        Console.WriteLine($"New best fitness: {bestFitness} (found in iteration {iters})");
                    }
                }
                else
                {
                    // Accept worse solutions often when starting out, but not as much near termination
                    double diff = neighbour.Fitness - currentSchedule.Fitness;
                    double acceptanceProb = 1 / (1 + Math.Exp(diff / temperature));
                    if (Globals.Rand.NextDouble() < acceptanceProb)
                    {
                        currentSchedule = neighbour;
                        currentScheduleFitness = neighbour.Fitness;
                    }
                }

                temperature *= alpha;
                iters++;
                // TODO: Maybe add a temperature reset to get out of a local minimum
            }
            
            Console.WriteLine($"Algorithm finished. The best schedule is as follows:\n{bestSchedule}");
        }

        /// <summary>
        /// Generates a list of team match-ups where each team plays against each other team exactly once.
        /// </summary>
        /// <returns></returns>
        private List<Tuple<int, int>> GenerateMinimalTeamMatchUps()
        {
            // Fill two arrays, each of which contains half of the team IDs
            int arraySize = _amountOfTeams / 2;
            int[] teamOneArray = new int[arraySize];
            int[] teamTwoArray = new int[arraySize];
            for (int i = 0; i < arraySize; i++)
            {
                teamOneArray[i] = i;
                teamTwoArray[i] = i + arraySize;
            }

            // Fill the match-up list by rotating every team but team 0 each round
            var matchUps = new List<Tuple<int, int>>();
            int maxId = _amountOfTeams - 1;
            for (int r = 0; r < _amountOfRounds; r++)
            {
                // Add the current match-ups
                for (int i = 0; i < arraySize; i++)
                {
                    matchUps.Add(new Tuple<int, int>(teamOneArray[i], teamTwoArray[i]));
                }
                
                // Rotate all indices except the first index of the first array.
                int arrayOneCarry = teamOneArray[arraySize - 1]; // Memorise the int we'll rotate to the second array.
                // Iterate backwards through the first array to avoid continuously copying the same int
                for (int i = arraySize - 1; i > 0; i--)
                {
                    teamOneArray[i] = i == 1 ? teamTwoArray[0] : teamOneArray[i - 1];
                }
                // Rotate the second array normally
                for (int i = 0; i < arraySize; i++)
                {
                    teamTwoArray[i] = i == arraySize - 1 ? arrayOneCarry : teamTwoArray[i + 1];
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
                new SportsMatch(SportsMatchCategory.TableTennis, 1),
                new SportsMatch(SportsMatchCategory.TableTennis, 2, tableTennisMin, tableTennisMax),
                new SportsMatch(SportsMatchCategory.TableTennis, 3, tableTennisMin, tableTennisMax),
                new SportsMatch(SportsMatchCategory.TableTennisDoubles, 2, tableTennisDoublesMin, tableTennisDoublesMax),
                // new SportsMatch(SportsMatchCategory.Volleyball, 6, volleyballMin, volleyballMax, true),
                new SportsMatch(SportsMatchCategory.Referee, 1, 1, 1)
            };
        }

        protected override List<Individual.Individual> InitRandomPopulation(int amountOfIndividuals)
        {
            var population = new List<Individual.Individual>();
            
            // If there is an odd amount of teams, add a break event to every round.
            bool addBreakRound = _amountOfTeams % 2 == 1;
            int eventsPerRound = addBreakRound ? _amountOfTeams / 2 + 1 : _amountOfTeams / 2;
            int regularEventsPerRound = addBreakRound ? eventsPerRound - 1 : eventsPerRound;
            
            Console.WriteLine($"-> Creating {amountOfIndividuals} schedules with {eventsPerRound} events per round.");
            for (int i = 0; i < amountOfIndividuals; i++)
            {
                population.Add(BallStarsSchedule.Random(
                    _amountOfTeams, eventsPerRound, regularEventsPerRound, _amountOfRounds, _matchPool, addBreakRound,
                    _avgPlayersPerTeam, _predefinedMatchUps
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

        /// <summary>
        /// Selects an amount of individuals equal to the given population's size by applying tournament selection,
        /// where the fittest individuals are taken from n random samples of a given tournament size.
        /// </summary>
        /// <param name="population"></param>
        /// <param name="offspring"></param>
        /// <param name="tournamentSize">The amount of individuals in each tournament. Larger tournament size results in
        /// a lower probability of selecting weaker individuals</param>
        /// <returns></returns>
        private List<BallStarsSchedule> TournamentSelection(List<BallStarsSchedule> population,
            List<BallStarsSchedule> offspring, int tournamentSize)
        {
            int n = population.Count;

            // Combine the population and offspring
            population.AddRange(offspring);

            BallStarsSchedule[] selected = new BallStarsSchedule[n];
            for (int i = 0; i < n; i++)
            {
                // Track the tournament winner and its fitness
                double bestFitness = Int32.MaxValue;
                BallStarsSchedule tournamentWinner = null;
                for (int j = 0; j < tournamentSize; j++)
                {
                    BallStarsSchedule randomSchedule = population[Globals.Rand.Next(population.Count)];
                    if (randomSchedule.Fitness < bestFitness)
                    {
                        bestFitness = randomSchedule.Fitness;
                        tournamentWinner = randomSchedule;
                    }
                }

                selected[i] = tournamentWinner;
            }

            return selected.ToList();
        }
    }
}