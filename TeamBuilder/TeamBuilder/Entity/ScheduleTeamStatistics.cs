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
        /// Tracks this team's amount of players allotted to each round by Events/SportsMatches.
        /// </summary>
        public int[] RoundPlayerCounts;
        
        /// <summary>
        /// Tracks the sports played by the team in the schedule.
        /// </summary>
        public HashSet<Sport> SportsPlayed;

        /// <summary>
        /// Tracks the sport imbalance of this team. The sport imbalance is represented as the maximum amount of times
        /// a sport is played in the schedule minus the minimum amount any sport is played.
        /// </summary>
        public int SportImbalance;

        private int _maxSportsPlayedCount = 0;
        private int _minSportsPlayedCount = 0;
        
        /// <summary>
        /// Track how many times this team plays each sport within the schedule.
        /// </summary>
        public Dictionary<SportsMatchCategory, int> SportsCategoryCounts = new Dictionary<SportsMatchCategory, int>()
        {
            {SportsMatchCategory.Badminton, 0},
            {SportsMatchCategory.BadmintonDoubles, 0},
            {SportsMatchCategory.Basketball, 0},
            {SportsMatchCategory.Floorball, 0},
            {SportsMatchCategory.Korfball, 0},
            // {Sport.Squash, 0},
            {SportsMatchCategory.TableTennis, 0},
            {SportsMatchCategory.TableTennisDoubles, 0},
            // {Sport.Volleyball, 0}
        };

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
            RoundPlayerCounts = new int[amountOfRounds];
            SportsPlayed = new HashSet<Sport>();
            TeamsPlayed = new HashSet<int>();
            
            // Initialise counters formally
            Breaks = 0;
            AmountOfTeamsPlayed = 0;
            AmountOfSportsPlayed = 0;
        }

        public void AddSportsCategoryPlayed(SportsMatchCategory category)
        {
            this.SportsCategoryCounts[category]++;
            
            int newCount = this.SportsCategoryCounts[category];
            if (newCount > _maxSportsPlayedCount)
            {
                _maxSportsPlayedCount = newCount;
            }
            
            this.UpdateSportImbalance();
        }

        public void RemoveSportsCategoryPlayed(SportsMatchCategory category)
        {
            this.SportsCategoryCounts[category]--;

            int newCount = this.SportsCategoryCounts[category];
            if (newCount < _minSportsPlayedCount)
            {
                _minSportsPlayedCount = newCount;
            }

            this.UpdateSportImbalance();
        }

        private void UpdateSportImbalance()
        {
            this.SportImbalance = _maxSportsPlayedCount - _minSportsPlayedCount;
        }
    }
}