using System;
using TeamBuilder.Entity.EvolutionaryAlgorithm;

namespace TeamBuilder
{
    class Program
    {
        static void Main(string[] args)
        {
            RunSchedulePlanner(args);
            // RunTeamBuilder(args);
        }

        private static void RunTeamBuilder(string[] allArgs)
        {
            // Require either 2 or 3 arguments
            if (allArgs.Length < 2 || allArgs.Length > 3)
            {
                Console.WriteLine("Usage: teambuilder players_file avg_team_size [output_file]");
                Environment.Exit(0);
            }

            string csvFilename = allArgs[0];
            int teamSize = int.Parse(allArgs[1]);
            string outputFile = allArgs.Length == 3 ? allArgs[2] : "ballstars_teams.csv";

            var teamBuilder = new BallStarsTeamBuilder(csvFilename, teamSize, outputFile);
            teamBuilder.Run();
        }

        private static void RunSchedulePlanner(string[] allArgs)
        {
            // Require at least 3 arguments
            if (allArgs.Length < 3)
            {
                Console.WriteLine("Usage: scheduler #rounds #teams #players_per_team [output_file]");
                Environment.Exit(0);   
            }
            
            int amountOfRounds = int.Parse(allArgs[0]);
            int amountOfTeams = int.Parse(allArgs[1]);
            int avgPlayersPerTeam = int.Parse(allArgs[2]);
            string outputFile = allArgs.Length > 3 ?
                allArgs[3] :
                $"schedule_{amountOfRounds}-rounds_{amountOfTeams}-teams_{avgPlayersPerTeam}-ppt.csv";
            string[] teamNames = new string[amountOfTeams];

            new BallStarsSchedulePlanner(
                    amountOfRounds,
                    amountOfTeams,
                    avgPlayersPerTeam,
                    outputFile,
                    usePredefinedMatchUps: true,
                    useLocalSearch: false,
                    useCrossover: false,
                    teamNames)
                .Run();
        }
    }
}
