using System.Collections.Generic;

namespace TeamBuilder.Entity
{
    internal class Team
    {
        public List<Player> Members;

        public Team(List<Player> members)
        {
            this.Members = members;
        }
    }
}
