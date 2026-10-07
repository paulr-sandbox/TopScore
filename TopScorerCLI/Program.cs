namespace TopScorerCLI;

class Program
{
    static void Main(string[] args)
    {
        try
        {
            if (args.Length == 0)
            {
                throw new ArgumentException("Please provide the path to the CSV file as an argument.\ne.g. dotnet run <path-to-csv>");
            }

            string filePath = args[0];

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"File not found at '{filePath}'");
            }

            Console.WriteLine($"Reading file: {filePath}\n");

            using StreamReader reader = new(filePath);

            var testScores = new TestScores(reader);

            // foreach (var scorer in testScores.TopScorers())
            // {
            //     Console.WriteLine($"{scorer.FirstName} {scorer.LastName}");
            // }
            // Console.WriteLine($"Score: {testScores.TopScore}");

            Console.WriteLine(testScores.TopScorersResult());

            var databaseManager = new DatabaseManager();
            databaseManager.InitialiseDatabase();

            databaseManager.ImportScores(testScores.Scores);
        }
        catch (Exception e)
        {
            Console.WriteLine($"ERROR: {e.Message}");
        }
    }
}
