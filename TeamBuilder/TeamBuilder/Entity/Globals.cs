using System;
using System.Collections.Generic;

namespace TeamBuilder.Entity
{
    public static class Globals
    {
        public static readonly Random Rand = new Random(77);

        public static readonly Dictionary<string, Sport> SportsMap = new Dictionary<string, Sport>()
        {
            // Sport names (English)
            { "Badminton", Sport.Badminton },
            { "Basketball", Sport.Basketball },
            { "Floorball", Sport.Floorball },
            { "Korfball", Sport.Korfball },
            { "Squash", Sport.Squash },
            { "Table tennis", Sport.TableTennis },
            { "Volleyball", Sport.Volleyball }
        };
        
        public static readonly Dictionary<SportsMatchCategory, int> MatchTypePlayerLimits =
            new Dictionary<SportsMatchCategory, int>()
        {
            { SportsMatchCategory.Badminton, 4 }, // 2 singles
            { SportsMatchCategory.Basketball, 24 }, // 5v5 + reserves on 2 courts
            { SportsMatchCategory.BadmintonDoubles, 4 },
            { SportsMatchCategory.Floorball, 10 }, // 4v4 on mini-court + reserves
            { SportsMatchCategory.Korfball, 16 }, // 8v8
            // { SportsMatchCategory.Referee, 3 } 
            { SportsMatchCategory.Squash, 8 }, // 4 singles
            { SportsMatchCategory.TableTennis, 10 }, // 3 singles + 1 doubles
            { SportsMatchCategory.Volleyball, 12 }, // 6v6
            { SportsMatchCategory.Basketball, 12 }, // 5v5 + reserve
            { SportsMatchCategory.Break, 10 }, // One team can take a break at a time.
        };

        /// <summary>
        /// Randomly shuffles a given list in-place.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static List<T> Shuffle<T>(this List<T> list)
        {
            int n = list.Count;
            for (int i = 0; i < n - 1; i++)
            {
                // Swap the elem at index i with randomly chosen index j
                int j = Rand.Next(i, n);
                T elem1 = list[i];
                T elem2 = list[j];

                list[i] = elem2;
                list[j] = elem1;
            }

            return list;
        }

        /// <summary>
        /// Returns the absolute value of given integer.
        /// </summary>
        /// <param name="i">The integer of which the caller wants the absolute value.</param>
        /// <returns>The absolute value of the given integer.</returns>
        public static int Abs(this int i)
        {
            return i < 0 ? i * -1 : i;
        }
    }
}
