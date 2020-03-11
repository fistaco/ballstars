using System.ComponentModel;

namespace TeamBuilder.Entity
{
    public enum SportsMatchCategory
    {
        // Regular categories
        [Description("Badminton")]
        Badminton,
        [Description("Basketball")]
        Basketball,
        [Description("Floorball")]
        Floorball,
        [Description("Korfball")]
        Korfball,
        [Description("Squash")]
        Squash,
        [Description("Table tennis")]
        TableTennis,
        [Description("Volleyball")]
        Volleyball,
        // Doubles
        [Description("Badminton (doubles)")]
        BadmintonDoubles,
        [Description("Table tennis (doubles)")]
        TableTennisDoubles,
        // Misc
        [Description("Referee")]
        Referee,
        [Description("Break")]
        Break
    }
}