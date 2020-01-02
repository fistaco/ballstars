using System;
using System.Collections.Generic;

namespace TeamBuilder.Entity
{
    public static class Globals
    {
        public static Random Rand = new Random(77);

        public static Dictionary<string, Sport> SportsMap = new Dictionary<string, Sport>()
        {
            { "Badminton", Sport.Badminton },
            { "Basketball", Sport.Basketball },
            { "Floorball", Sport.Floorball },
            { "Korfball", Sport.Korfball },
            { "Squash", Sport.Squash },
            { "TableTennis", Sport.TableTennis },
            { "Volleyball", Sport.Volleyball }
        };
    }
}
