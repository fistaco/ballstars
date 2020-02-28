using System.Collections.Generic;
using TeamBuilder.Entity.Individual;

namespace TeamBuilder.Entity
{
    internal class RoundPlanning
    {
        public Event[] Events;

        /// <summary>
        /// Constructs a new Round with an empty Event array for a given amount of events.
        /// </summary>
        /// <param name="amountOfEvents"></param>
        public RoundPlanning(int amountOfEvents)
        {
            this.Events = new Event[amountOfEvents];
        }

        /// <summary>
        /// Creates an empty array for this rounds' events. The length is based on whether the amount of teams is even
        /// or odd.
        /// </summary>
        /// <param name="amountOfTeams"></param>
        /// <returns></returns>
        private Event[] CreateEmptyRoundEventArray(int amountOfTeams)
        {
            return amountOfTeams % 2 == 0 ? new Event[amountOfTeams / 2] : new Event[amountOfTeams / 2 + 1];
        }

        public static RoundPlanning Random(int amountOfTeams, int amountOfEvents, int amountOfRegularEvents,
            List<SportsMatch> matchPool, bool breakRound = false)
        {
            var round = new RoundPlanning(amountOfEvents);

            // Generate amountOfTeams/2 regular events.
            for (int i = 0; i < amountOfRegularEvents; i++)
            {
                int t0 = Globals.Rand.Next(0, amountOfTeams);
                int t1 = Globals.Rand.Next(0, amountOfTeams);

                round.Events[i] = Event.Random(t0, t1, matchPool);
            }
            // Generate a break event if the amount of teams is odd.
            if (breakRound)
            {
                int t = Globals.Rand.Next(0, amountOfTeams);
                round.Events[amountOfEvents - 1] = new BreakEvent(t);
            }
            
            return round;
        }
    }
}