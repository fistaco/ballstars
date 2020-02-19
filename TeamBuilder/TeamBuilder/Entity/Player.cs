
namespace TeamBuilder.Entity
{
    internal class Player
    {
        public readonly string Name;
        public readonly Gender Gender;
        public readonly Sport Sport;
        public readonly bool InOrganisation;

        public Player(string name, string gender, string sportName, bool inOrganisation)
        {
            this.Name = name;
            this.Gender = gender == "Male" ? Gender.Male : Gender.Female;
            this.Sport = Globals.SportsMap[sportName];
            this.InOrganisation = inOrganisation;
        }

        public bool IsMale()
        {
            return this.Gender == Gender.Male;
        }

        public bool IsFemale()
        {
            return this.Gender == Gender.Female;
        }
    }
}
