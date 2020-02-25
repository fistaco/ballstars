using System;
using System.Collections.Generic;
using System.Linq;

namespace TeamBuilder.Entity
{
    internal class Team
    {
        public List<Player> Members = new List<Player>();

        public int GenderImbalance;
        public int SportImbalance;
        public int AmountOfOrganisers;

        private readonly Dictionary<Gender, int> _genderCounts = new Dictionary<Gender, int>()
        {
            { Gender.Male, 0 },
            { Gender.Female, 0 }
        };

        private readonly Dictionary<Sport, int> _sportCounts = new Dictionary<Sport, int>()
        {
            {Sport.Badminton, 0},
            {Sport.Basketball, 0},
            {Sport.Floorball, 0},
            {Sport.Korfball, 0},
            {Sport.Squash, 0},
            {Sport.TableTennis, 0},
            {Sport.Volleyball, 0}
        };

        public Team(List<Player> members)
        {
            foreach (Player p in members)
            {
                this.AddPlayer(p);
            }
        }

        public Team() { }

        public void AddPlayer(Player p)
        {
            this.Members.Add(p);
            this.UpdateTeamProperties(p, true);
        }

        public Player RemoveRandomPlayer()
        {
            int removeIndex = Globals.Rand.Next(0, this.Members.Count);
            Player toRemove = this.Members[removeIndex];
            this.Members.RemoveAt(removeIndex);

            this.UpdateTeamProperties(toRemove, false);

            return toRemove;
        }

        private void UpdateTeamProperties(Player p, bool added)
        {
            // Add or subtract 1 from the counts for a player addition/removal
            int countModification = added ? 1 : -1;
            _genderCounts[p.Gender] += countModification;
            _sportCounts[p.Sport] += countModification;
            if (p.InOrganisation)
            {
                this.AmountOfOrganisers += countModification;
            }

            // Update imbalances by using the gender and sport counts of which we're keeping track
            this.GenderImbalance = Math.Abs(_genderCounts[Gender.Male] - _genderCounts[Gender.Female]);
            this.SportImbalance = this._sportCounts.Count(pair => pair.Value != 1);
        }

        public Team Clone()
        {
            Team teamClone = new Team();
            foreach (Player p in this.Members)
            {
                teamClone.AddPlayer(p.Clone());
            }

            return teamClone;
        }
    }
}
