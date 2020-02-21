using System;
using System.Collections.Generic;

namespace TeamBuilder.Entity
{
    public static class Globals
    {
        public static readonly Random Rand = new Random(77);

        public static readonly Dictionary<string, Sport> SportsMap = new Dictionary<string, Sport>()
        {
            { "Badminton", Sport.Badminton },
            { "Basketball", Sport.Basketball },
            { "Floorball", Sport.Floorball },
            { "Korfball", Sport.Korfball },
            { "Squash", Sport.Squash },
            { "TableTennis", Sport.TableTennis },
            { "Volleyball", Sport.Volleyball }
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
    }
}
