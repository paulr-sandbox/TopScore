namespace TopScorerCLI;

public record TestScore(string FirstName, string LastName, int Score);

public class TestScores
{
    public List<TestScore> Scores { get; } = [];

    public int TopScore => Scores.Count != 0 ? Scores.Max(s => s.Score) : 0;

    public TestScores(List<string> lines)
    {
        if (lines.Count < 2)
        {
            throw new InvalidDataException("Input does not contain enough lines.");
        }
        var headerLine = lines[0];
        lines.RemoveAt(0);
        if (string.IsNullOrWhiteSpace(headerLine) || headerLine.Split(',').Length != 3)
        {
            throw new InvalidDataException("Invalid header row.");
        }

        var rowNumber = 2;

        foreach (var line in lines)
        {
            string[] values = line.Split(',');

            if (values.Length == 3 && int.TryParse(values[2], out int score))
            {
                Scores.Add(new TestScore(values[0].Trim(), values[1].Trim(), score));
            }
            else
            {
                Console.WriteLine($"Invalid row on line #{rowNumber}. Skipping...");
            }
            rowNumber++;
        }
    }

    public List<TestScore> TopScorers()
    {
        return [.. Scores.Where(s => s.Score == TopScore)
            .OrderBy(s => s.FirstName)
            .ThenBy(s => s.LastName)];
    }

    public string TopScorersResult()
    {
        var result = "";
        foreach (var person in TopScorers())
        {
            result += $"{person.FirstName} {person.LastName}\n";
        }
        result += $"Score: {TopScore}";
        return result;
    }
}