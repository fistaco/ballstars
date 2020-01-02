using System;
using System.Collections.Generic;

namespace TeamBuilder.Entity.Individual
{
    class BallStarsTeamSet : Individual
    {
        public List<Team> teams;

        /// <summary>
        /// Constructs a random set of teams based on a list of players given in a CSV file.
        /// </summary>
        /// <param name="fileName"></param>
        public BallStarsTeamSet(string fileName)
        {

        }

        public override Individual Combine(Individual other)
        {
            throw new NotImplementedException();
        }

        public override void Mutate()
        {
            throw new NotImplementedException();
        }
    }
}
