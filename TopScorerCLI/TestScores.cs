namespace TopScorerCLI;

public record DataRow(string FirstName, string LastName, int Score);

public class TestScores
{
    public List<DataRow> Scores { get; } = [];

    public int TopScore => Scores.Max(s => s.Score);

    public TestScores(StreamReader reader)
    {
        string? headerLine = reader.ReadLine();

        if (string.IsNullOrWhiteSpace(headerLine))
        {
            throw new InvalidDataException("Invalid header row.");
        }

        string? line;
        int rowNumber = 2;

        while ((line = reader.ReadLine()) != null)
        {
            string[] values = line.Split(',');
            if (values.Length != 3)
            {
                throw new InvalidDataException("Invalid CSV row.");
            }

            if (int.TryParse(values[2], out int result))
            {
                Scores.Add(new DataRow(values[0].Trim(), values[1].Trim(), result));
            }
            else
            {
                Console.WriteLine($"Invalid score on row {rowNumber}");
            }
            rowNumber++;
        }
    }

    public List<DataRow> TopScorers()
    {
        return [.. Scores.Where(s => s.Score == TopScore)];
    }
}