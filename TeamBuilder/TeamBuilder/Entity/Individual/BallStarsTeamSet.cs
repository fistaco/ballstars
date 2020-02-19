using System;
using System.Collections.Generic;
using System.Linq;

namespace TeamBuilder.Entity.Individual
{
    class BallStarsTeamSet : Individual
    {
        public List<Team> Teams = new List<Team>();

        /// <summary>
        /// Constructs a random set of teams based on a list of players given in a CSV file.
        /// </summary>
        /// <param name="fileName"></param>
        public BallStarsTeamSet(string fileName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Constructs a random set of teams based on a given list of players and a desired team size.
        /// </summary>
        /// <param name="players"></param>
        /// <param name="teamSize"></param>
        public BallStarsTeamSet(List<Player> players, int teamSize)
        {
            // Shuffle the players and put them in a queue
            Queue<Player> playerQueue = new Queue<Player>(players.Shuffle());

            // Distribute the players over teams of the given size
            Team currentTeam = new Team();
            while (playerQueue.Count > 0)
            {
                Player p = playerQueue.Dequeue();
                currentTeam.AddPlayer(p);

                if (currentTeam.Members.Count == teamSize)
                {
                    this.Teams.Add(currentTeam);
                    currentTeam = new Team();
                }
            }
        }

        /// <summary>
        /// Constructs a BallStarsTeamSet with an empty list of teams.
        /// </summary>
        public BallStarsTeamSet() { }

        /// <summary>
        /// Evaluates and sets this team set's fitness value. Its fitness, which should be minimised, is defined as the
        /// sum of all teams' gender and sport imbalances plus 999 for each team that has more than one organiser.
        /// </summary>
        /// <returns></returns>
        public override float Evaluate()
        {
            int fitness = this.Teams.Sum(
                team => team.GenderImbalance +
                        team.SportImbalance +
                        (team.AmountOfOrganisers <= 1 ? 0 : 999) // Heavily penalise teams with more than one organiser
            );

            this.Fitness = fitness;
            return fitness;
        }
        
        public override Individual Crossover(Individual other)
        {
            throw new NotImplementedException();
        }

        public override void Mutate()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns a random team from this BallStarsTeamSet.
        /// </summary>
        /// <returns>A random Team object from this BallStarsTeamSet</returns>
        private Team RandomTeam()
        {
            return this.Teams[Globals.Rand.Next(0, this.Teams.Count)];
        }

        public void Print()
        {
            Console.WriteLine($"Fitness: {this.Fitness}");
            for (int i = 0; i < this.Teams.Count; i++)
            {
                Console.WriteLine($"Team {i}:");
                this.Teams[i].Print();
                Console.WriteLine();
            }
        }

        public BallStarsTeamSet Clone()
        {
            var clone = new BallStarsTeamSet();
            clone.Teams = this.Teams.Select(t => t.Clone()).ToList();
            clone.Fitness = this.Fitness;

            return clone;
        }
    }
}
