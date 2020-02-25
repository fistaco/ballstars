using System.Collections.Generic;

namespace TeamBuilder.Entity.Individual
{
    internal class BallStarsSchedule : Individual
    {
        public RoundPlanning[] Rounds;
        
        public BallStarsSchedule(int timeSlotAmount)
        {
            this.Rounds = new RoundPlanning[timeSlotAmount];
        }

        public List<SportsMatch> InitialiseMatchPool()
        {
            // Each match has a category and an amount of players to be allotted per team
            return new List<SportsMatch>()
            {
                new SportsMatch(SportsMatchCategory.Badminton, 1),
                new SportsMatch(SportsMatchCategory.Badminton, 2),
                new SportsMatch(SportsMatchCategory.BadmintonDoubles, 2),
                new SportsMatch(SportsMatchCategory.Basketball, 5),
                new SportsMatch(SportsMatchCategory.Basketball, 6), // 1 reserve
                new SportsMatch(SportsMatchCategory.Floorball, 4),
                new SportsMatch(SportsMatchCategory.Floorball, 5),
                new SportsMatch(SportsMatchCategory.Korfball, 8),
                new SportsMatch(SportsMatchCategory.Squash, 1),
                new SportsMatch(SportsMatchCategory.Squash, 2),
                new SportsMatch(SportsMatchCategory.Squash, 3),
                new SportsMatch(SportsMatchCategory.TableTennis, 1),
                new SportsMatch(SportsMatchCategory.TableTennis, 2),
                new SportsMatch(SportsMatchCategory.TableTennis, 3),
                new SportsMatch(SportsMatchCategory.TableTennisDoubles, 2),
                new SportsMatch(SportsMatchCategory.Volleyball, 6),
                new SportsMatch(SportsMatchCategory.Referee, 1)
            };
        }
        
        public override float Evaluate()
        {
            throw new System.NotImplementedException();
        }

        public override Individual Crossover(Individual other)
        {
            throw new System.NotImplementedException();
        }

        public override void Mutate()
        {
            throw new System.NotImplementedException();
        }
    }
}