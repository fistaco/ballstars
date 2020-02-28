using System;
using System.Collections.Generic;
using TeamBuilder.Entity.Individual;

namespace TeamBuilder.Entity.EvolutionaryAlgorithm
{
    internal class BallStarsSchedulePlanner : EvolutionaryAlgorithm
    {
        private readonly int _amountOfRounds;
        
        private readonly string[] _teamNames;
        private readonly int _amountOfTeams;
        private readonly int _maxPlayersPerTeam;

        private List<SportsMatch> _matchPool;

        public BallStarsSchedulePlanner(int amountOfRounds, string[] teamNames, int maxPlayersPerTeam)
        {
            _amountOfRounds = amountOfRounds;
            _teamNames = teamNames;
            _amountOfTeams = teamNames.Length;
            _maxPlayersPerTeam = maxPlayersPerTeam;

            _matchPool = this.InitialiseMatchPool();
        }
        
        public override void Run()
        {
            throw new System.NotImplementedException();
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
            // Each match has a category and an amount of players to be allotted per team
            return new List<SportsMatch>()
            {
                new SportsMatch(SportsMatchCategory.Badminton, 1),
                new SportsMatch(SportsMatchCategory.Badminton, 2),
                new SportsMatch(SportsMatchCategory.BadmintonDoubles, 2),
                new SportsMatch(SportsMatchCategory.Basketball, 5),
                new SportsMatch(SportsMatchCategory.Basketball, 6), // 1 reserve
                new SportsMatch(SportsMatchCategory.Floorball, 4),
                new SportsMatch(SportsMatchCategory.Floorball, 5),
                new SportsMatch(SportsMatchCategory.Korfball, 8),
                new SportsMatch(SportsMatchCategory.Squash, 1),
                new SportsMatch(SportsMatchCategory.Squash, 2),
                new SportsMatch(SportsMatchCategory.Squash, 3),
                new SportsMatch(SportsMatchCategory.TableTennis, 1),
                new SportsMatch(SportsMatchCategory.TableTennis, 2),
                new SportsMatch(SportsMatchCategory.TableTennis, 3),
                new SportsMatch(SportsMatchCategory.TableTennisDoubles, 2),
                new SportsMatch(SportsMatchCategory.Volleyball, 6),
                new SportsMatch(SportsMatchCategory.Referee, 1)
            };
        }

        protected override List<Individual.Individual> InitRandomPopulation(int amountOfIndividuals)
        {
            var population = new List<Individual.Individual>();
            
            // If there is an odd amount of teams, add a break event to every round.
            bool addBreakRound = _amountOfTeams % 2 == 1;
            int eventsPerRound = addBreakRound ? _amountOfTeams / 2 + 1 : _amountOfTeams / 2;
            int regularEventsPerRound = addBreakRound ? eventsPerRound - 1 : eventsPerRound;
            
            for (int i = 0; i < amountOfIndividuals; i++)
            {
                population.Add(BallStarsSchedule.Random(
                    _amountOfTeams, eventsPerRound, regularEventsPerRound, _amountOfRounds, _matchPool, addBreakRound
                ));
            }

            return population;
        }

        protected override void SelectSurvivors(List<Individual.Individual> population)
        {
            throw new System.NotImplementedException();
        }
    }
}