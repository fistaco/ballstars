using System;
using TeamBuilder.Entity.EvolutionaryAlgorithm;

namespace TeamBuilder
{
    class Program
    {
        static void Main(string[] args)
        {
            // Require either 2 or 3 arguments
            if (args.Length > 3)
            {
                Console.WriteLine("Usage: teambuilder players_file avg_team_size [output_file]");
                Environment.Exit(0);
            }

            string csvFilename = args[0];
            int teamSize = int.Parse(args[1]);
            string outputFile = args.Length == 3 ? args[2] : "ballstars_teams.csv";

            var teamBuilder = new BallStarsTeamBuilder(csvFilename, teamSize, outputFile);
            teamBuilder.Run();
        }
    }
}
