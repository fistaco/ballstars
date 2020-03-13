using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

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
        
        /// <summary>
        /// The per-team player limits for each sports category during each round.
        /// </summary>
        public static readonly Dictionary<SportsMatchCategory, int> MatchTypePlayerLimitsPerTeam =
            new Dictionary<SportsMatchCategory, int>()
        {
            { SportsMatchCategory.Badminton, 1 }, // 1 single
            { SportsMatchCategory.BadmintonDoubles, 4 }, // 2 doubles
            { SportsMatchCategory.Basketball, 12 }, // 5v5 + reserves on 2 courts
            { SportsMatchCategory.Floorball, 5 }, // 4v4 + reserve
            { SportsMatchCategory.Korfball, 6 }, // 4v4 + possible reserves or 6v6
            // { SportsMatchCategory.Referee, 3 },  
            { SportsMatchCategory.Squash, 8 }, // 4 singles
            { SportsMatchCategory.TableTennis, 3 }, // 3 singles
            { SportsMatchCategory.TableTennisDoubles, 2}, // 1 doubles
            { SportsMatchCategory.Volleyball, 6 }, // 6v6
            { SportsMatchCategory.Break, 10 }, // One team can take a break at a time.
        };
        
        /// <summary>
        /// Returns the Description string belonging to some Enum value.
        /// </summary>
        /// <param name="enumerationValue"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static string GetDescription<T>(this T enumerationValue)
            where T : struct
        {
            Type type = enumerationValue.GetType();
            if (!type.IsEnum)
            {
                throw new ArgumentException("EnumerationValue must be of Enum type", "enumerationValue");
            }

            //Tries to find a DescriptionAttribute for a potential friendly name
            //for the enum
            MemberInfo[] memberInfo = type.GetMember(enumerationValue.ToString());
            if (memberInfo != null && memberInfo.Length > 0)
            {
                object[] attrs = memberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

                if (attrs != null && attrs.Length > 0)
                {
                    //Pull out the description value
                    return ((DescriptionAttribute)attrs[0]).Description;
                }
            }
            //If we have no description attribute, just return the ToString of the enum
            return enumerationValue.ToString();
        }

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
