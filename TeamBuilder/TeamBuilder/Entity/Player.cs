
namespace TeamBuilder.Entity
{
    internal class Player
    {
        public string Name;
        public Gender Gender;
        public Sport Sport;
        public bool InOrganisation;


        public Player(string name, string gender, string sportName, bool inOrganisation)
        {
            this.Name = name;
            this.Gender = gender == "Male" ? Gender.Male : Gender.Female;
            this.Sport = Globals.SportsMap[sportName];
            this.InOrganisation = inOrganisation;
        }
    }
}
