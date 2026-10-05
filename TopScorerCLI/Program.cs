class Program
{
    static void Main(string[] args)
    {
        try
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Please provide the path to the CSV file as an argument.\ne.g. dotnet run <path-to-csv>");
                return;
            }

            string filePath = args[0];

            if (!File.Exists(filePath))
            {
                Console.WriteLine($"ERROR: File not found at '{filePath}'");
                return;
            }

            Console.WriteLine($"Reading file: {filePath}\n");

            using StreamReader reader = new(filePath);

            string? headerLine = reader.ReadLine();

            if (string.IsNullOrWhiteSpace(headerLine)) throw new InvalidDataException("Invalid header row.");

            var headers = headerLine.Split(',');
            var columnCount = headers.Length;

            string? line;
            int rowNumber = 2;

            List<DataRow> rows = [];

            while ((line = reader.ReadLine()) != null)
            {
                string[] values = line.Split(',');
                if (values.Length != 3) throw new InvalidDataException("Invalid CSV row.");
                if (int.TryParse(values[2], out int result)) rows.Add(new DataRow(values[0], values[1], result));
                else Console.WriteLine($"Invalid score on row {rowNumber}");
                rowNumber++;
            }

            var topScore = rows.Max(r => r.Score);
            var topScorers = rows.Where(r => r.Score == topScore).OrderBy(r => r.FirstName).ThenBy(r => r.LastName);

            foreach (var scorer in topScorers)
            {
                Console.WriteLine($"{scorer.FirstName} {scorer.LastName}");
            }
            Console.WriteLine($"Score: {topScore}");
        }
        catch (Exception e) { Console.WriteLine($"ERROR: {e.Message}"); }
    }
}

public record DataRow(string FirstName, string LastName, int Score);
