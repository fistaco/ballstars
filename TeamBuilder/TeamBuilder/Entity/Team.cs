using System;
using System.Collections.Generic;

namespace TeamBuilder.Entity
{
    internal class Team
    {
        public List<Player> Members;

        public int GenderImbalance = 0;
        private readonly Dictionary<Gender, int> _genderCounts = new Dictionary<Gender, int>()
        {
            { Gender.Male, 0 },
            { Gender.Female, 0 }
        };

        public int AmountOfOrganisers = 0;

        public int SportImbalance = 0;

        public Team(List<Player> members)
        {
            foreach (Player p in members)
            {
                this.AddPlayer(p);
            }
        }

        public void AddPlayer(Player p)
        {
            this.Members.Add(p);

            _genderCounts[p.Gender]++;
            this.GenderImbalance = Math.Abs(_genderCounts[Gender.Male] - _genderCounts[Gender.Female]);
        }

        public Player RemoveRandomPlayer()
        {
            int removeIndex = Globals.Rand.Next(0, this.Members.Count);
            Player toRemove = this.Members[removeIndex];
            this.Members.RemoveAt(removeIndex);

            return toRemove;
        }
    }
}
