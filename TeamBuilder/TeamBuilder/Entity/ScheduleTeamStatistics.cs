using System.Collections.Generic;

namespace TeamBuilder.Entity
{
    /// <summary>
    /// Tracks several statistics for a team within a BallStarsSchedule for efficient fitness evaluation.
    /// </summary>
    internal class ScheduleTeamStatistics
    {
        /// <summary>
        /// Tracks this team's matches played during each round.
        /// </summary>
        public int[] MatchesPerRound;
        
        /// <summary>
        /// Tracks the sports played by the team in the schedule.
        /// </summary>
        public HashSet<Sport> SportsPlayed;

        /// <summary>
        /// Tracks the other unique teams played by this team.
        /// </summary>
        public HashSet<int> TeamsPlayed;

        /// <summary>
        /// Tracks the total amount of breaks this team has in the schedule.
        /// </summary>
        public int Breaks;

        /// <summary>
        /// Tracks the amount of unique teams this team plays against in the schedule.
        /// </summary>
        public int AmountOfTeamsPlayed;

        /// <summary>
        /// Tracks the amount of unique sports played by this team in the schedule.
        /// </summary>
        public int AmountOfSportsPlayed;
        
        public ScheduleTeamStatistics(int amountOfTeams, int amountOfRounds)
        {
            // Initialise all data structures based on the amount of teams and rounds
            MatchesPerRound = new int[amountOfRounds];
            SportsPlayed = new HashSet<Sport>();
            TeamsPlayed = new HashSet<int>();
            
            // Initialise counters formally
            Breaks = 0;
            AmountOfTeamsPlayed = 0;
            AmountOfSportsPlayed = 0;
        }

        public void AddSportPlayed(Sport sport)
        {
            if (!SportsPlayed.Contains(sport))
            {
                SportsPlayed.Add(sport);
                AmountOfSportsPlayed++;
            }
        }

        public void RemoveSportPlayed(Sport sport)
        {
        }
    }
}