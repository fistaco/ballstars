
namespace TeamBuilder.Entity
{
    internal class Player
    {
        public readonly int ID;
        public readonly Gender Gender;
        public readonly Sport Sport;
        public readonly bool InOrganisation;

        public Player(int id, string gender, string sportName, bool inOrganisation)
        {
            this.ID = id;
            this.Gender = gender == "Male" ? Gender.Male : Gender.Female;
            this.Sport = Globals.SportsMap[sportName];
            this.InOrganisation = inOrganisation;
        }

        public Player(int id, Gender gender, Sport sport, bool inOrganisation)
        {
            this.ID = id;
            this.Gender = gender;
            this.Sport = sport;
        }

        public bool IsMale()
        {
            return this.Gender == Gender.Male;
        }

        public bool IsFemale()
        {
            return this.Gender == Gender.Female;
        }

        public Player Clone()
        {
            return new Player(this.ID, this.Gender, this.Sport, this.InOrganisation);
        }
    }
}
