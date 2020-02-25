using System;
using TeamBuilder.Entity.EvolutionaryAlgorithm;

namespace TeamBuilder
{
    class Program
    {
        static void Main(string[] args)
        {
            // Require either 2 or 3 arguments
            if (args.Length < 2 || args.Length > 3)
            {
                // // TODO: remove this test stuff later
                // string testcsvFilename = "./../../../tests/test-cases/test-input-64-player-converted-sport-names.csv";
                // int testteamSize = 8;
                // string testoutputFile = "test_ballstars_teams.csv";
                //
                // var testteamBuilder = new BallStarsTeamBuilder(testcsvFilename, testteamSize, testoutputFile);
                // testteamBuilder.Run();

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
