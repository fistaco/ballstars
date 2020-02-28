using System;
using System.Collections.Generic;

namespace TeamBuilder.Entity.EvolutionaryAlgorithm
{
    internal class BallStarsSchedulePlanner : EvolutionaryAlgorithm
    {
        private int _amountOfRounds;
        
        private readonly string[] _teamNames;
        private int _maxPlayersPerTeam;

        public BallStarsSchedulePlanner(int amountOfRounds, string[] teamNames, int maxPlayersPerTeam)
        {
            _amountOfRounds = amountOfRounds;
            _teamNames = teamNames;
            _maxPlayersPerTeam = maxPlayersPerTeam;
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
        
        public List<SportsMatch> InitialiseMatchPool()
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
            throw new System.NotImplementedException();
        }

        protected override void SelectSurvivors(List<Individual.Individual> population)
        {
            throw new System.NotImplementedException();
        }
    }
}