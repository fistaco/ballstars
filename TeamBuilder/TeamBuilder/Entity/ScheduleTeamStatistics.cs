using System.Collections.Generic;
using System.Linq;

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
        /// Tracks the amount of events this team plays in for each round.
        /// </summary>
        public int[] EventsPerRound;

        // TODO: Incrementally update the penalty
        /// <summary>
        /// Tracks this team's penalty incurred due to not having exactly 1 event per round.
        /// </summary>
        public int EventLimitPenalty => EventsPerRound.Sum(eventsInRound => (eventsInRound - 1).Abs());
        
        /// <summary>
        /// Tracks the sports played by the team in the schedule.
        /// </summary>
        public HashSet<Sport> SportsPlayed;

        /// <summary>
        /// Tracks the sport imbalance of this team. The sport imbalance is represented as the maximum amount of times
        /// a sport is played in the schedule minus the minimum amount any sport is played.
        /// </summary>
        public int SportImbalance;

        /// <summary>
        /// The largest amount of times any sport is played by this team within the schedule.
        /// </summary>
        private int _maxSportsPlayedCount = 0;

        private SportsMatchCategory _maxPlayedCategory = SportsMatchCategory.Badminton;

        /// <summary>
        /// The smallest amount of times any sport is played by this team within the schedule.
        /// </summary>
        private int _minSportsPlayedCount = 0;

        private SportsMatchCategory _minPlayedCategory = SportsMatchCategory.Basketball;

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
        /// Tracks how many times this team has played against each other team.
        /// </summary>
        public int[] MatchUpCountsVersusTeams;

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

        /// <summary>
        /// The penalty this team receives due to not having played enough other unique teams. This is equal to
        /// _teamsToPlay - AmountOfTeamsPlayed.
        /// </summary>
        public int TeamCoveragePenalty;

        private int _teamsToPlay;

        /// <summary>
        /// The penalty this team receives due to not having played all sports at least once. This is equal to
        /// _sportsToPlay - AmountOfSportsPlayed.
        /// </summary>
        public int SportsCoveragePenalty;

        private int _sportsToPlay;
        
        public ScheduleTeamStatistics(int amountOfTeams, int amountOfRounds)
        {
            // Initialise all data structures based on the amount of teams and rounds
            MatchesPerRound = new int[amountOfRounds];
            RoundPlayerCounts = new int[amountOfRounds];
            EventsPerRound = new int[amountOfRounds];
            SportsPlayed = new HashSet<Sport>();
            MatchUpCountsVersusTeams = new int[amountOfTeams];
            
            // Initialise counters formally
            Breaks = 0;
            AmountOfTeamsPlayed = 0;

            _teamsToPlay = amountOfTeams - 1;
            _sportsToPlay = SportsCategoryCounts.Count;
        }

        /// <summary>
        /// Updates this team's sports played and player count per round based on the addition of a given SportsMatch in
        /// a given round.
        /// </summary>
        /// <param name="match">The SportsMatch that has just been scheduled for this team.</param>
        /// <param name="roundIndex">The number of the round in which the given match was added.</param>
        public void UpdateAfterSportsMatchAddition(SportsMatch match, int roundIndex)
        {
            this.AddSportsCategoryPlayed(match.MatchType);
            this.RoundPlayerCounts[roundIndex] += match.PlayersPerTeam;
        }

        /// <summary>
        /// Updates this team's sports played and player count per round based on the removal of a given SportsMatch in
        /// a given round.
        /// </summary>
        /// <param name="match">The SportsMatch that has just been scheduled for this team.</param>
        /// <param name="roundIndex">The number of the round in which the given match was added.</param>
        public void UpdateAfterSportsMatchRemoval(SportsMatch match, int roundIndex)
        {
            this.RemoveSportsCategoryPlayed(match.MatchType);
            this.RoundPlayerCounts[roundIndex] -= match.PlayersPerTeam;
        }

        public void UpdateAfterEventAddition(int opponent, int roundIndex)
        {
            this.IncrementMatchUpCount(opponent);
            this.EventsPerRound[roundIndex]++;
        }

        public void UpdateAfterEventRemoval(int opponent, int roundIndex)
        {
            this.DecrementMatchUpCount(opponent);
            this.EventsPerRound[roundIndex]--;
        }

        public void AddSportsCategoryPlayed(SportsMatchCategory category)
        {
            // Update amount of sports played if this is the first time this sport is played by this team
            if (this.SportsCategoryCounts[category] == 0)
            {
                AmountOfSportsPlayed++;
                UpdateSportsCoveragePenalty();
            }
            
            this.SportsCategoryCounts[category]++;
            
            // If this category was the least played one, we may have to find a new smallest element
            if (category == _minPlayedCategory)
            {
                UpdateLeastPlayedCategory();
            }
            
            // Update _maxSportsPlayedCount if this category is now played more than any other category
            int newCount = this.SportsCategoryCounts[category];
            if (newCount > _maxSportsPlayedCount)
            {
                _maxSportsPlayedCount = newCount;
                _maxPlayedCategory = category;
            }

            this.UpdateSportImbalance();
        }

        public void RemoveSportsCategoryPlayed(SportsMatchCategory category)
        {
            // Update amount of sports played if this sport was only played once
            if (this.SportsCategoryCounts[category] == 1)
            {
                AmountOfSportsPlayed--;
                UpdateSportsCoveragePenalty();
            }
            
            this.SportsCategoryCounts[category]--;
            
            // If this category was the most played one, we may have to find a new largest element
            if (category == _maxPlayedCategory)
            {
                UpdateMostPlayedCategory();
            }

            int newCount = this.SportsCategoryCounts[category];
            if (newCount < _minSportsPlayedCount)
            {
                _minSportsPlayedCount = newCount;
                _minPlayedCategory = category;
            }

            this.UpdateSportImbalance();
        }

        public void IncrementMatchUpCount(int opponent)
        {
            if (MatchUpCountsVersusTeams[opponent] == 0)
            {
                AmountOfTeamsPlayed++;
                UpdateTeamCoveragePenalty();
            }

            MatchUpCountsVersusTeams[opponent]++;
        }
        
        public void DecrementMatchUpCount(int opponent)
        {
            if (this.MatchUpCountsVersusTeams[opponent] == 1)
            {
                AmountOfTeamsPlayed--;
                UpdateTeamCoveragePenalty();
            }

            MatchUpCountsVersusTeams[opponent]--;
        }

        private void UpdateSportImbalance()
        {
            this.SportImbalance = _maxSportsPlayedCount - _minSportsPlayedCount;
        }

        private void UpdateTeamCoveragePenalty()
        {
            TeamCoveragePenalty = _teamsToPlay - AmountOfTeamsPlayed;
        }

        private void UpdateSportsCoveragePenalty()
        {
            SportsCoveragePenalty = _sportsToPlay - AmountOfSportsPlayed;
        }

        private void UpdateLeastPlayedCategory()
        {
            int smallest = 999;
            foreach (KeyValuePair<SportsMatchCategory, int> pair in SportsCategoryCounts)
            {
                if (pair.Value < smallest)
                {
                    _minSportsPlayedCount = pair.Value;
                    _minPlayedCategory = pair.Key;
                }
            }
        }
        
        private void UpdateMostPlayedCategory()
        {
            int largest = -1;
            foreach (KeyValuePair<SportsMatchCategory, int> pair in SportsCategoryCounts)
            {
                if (pair.Value > largest)
                {
                    _maxSportsPlayedCount = pair.Value;
                    _maxPlayedCategory = pair.Key;
                }
            }
        }
    }
}