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

            List<string> csvRows = [];
            string? line;

            while ((line = reader.ReadLine()) != null)
            {
                csvRows.Add(line);
            }

            var testScores = new TestScores(csvRows);

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
