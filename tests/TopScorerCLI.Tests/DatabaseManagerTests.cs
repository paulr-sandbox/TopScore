using Microsoft.Data.Sqlite;
using TopScorerCLI;

public class DatabaseManagerTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly DatabaseManager _dbManager;

    public DatabaseManagerTests()
    {
        _connection = new SqliteConnection("Data Source=TestInMemoryDb;Mode=Memory;Cache=Shared");
        _connection.Open();

        var connectionString = _connection.ConnectionString;
        _dbManager = new DatabaseManager(connectionString);

        _dbManager.InitialiseDatabase();
    }

    private static string CaptureConsoleOutput(Action action)
    {
        var originalOut = Console.Out;
        using var stringWriter = new StringWriter();

        try
        {
            Console.SetOut(stringWriter);
            action();
            return stringWriter.ToString();
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }

    [Fact]
    public void WhenImportScores_SavedCorrectly()
    {
        List<TestScore> sampleScores = [
            new TestScore("Dee", "Moore", 56),
            new TestScore("Sipho","Lolo",78),
            new TestScore("Noosrat","Hoosain",64),
            new TestScore("George","Of The Jungle",78)
        ];

        _dbManager.ImportScores(sampleScores);

        using var command = _connection.CreateCommand();
        command.CommandText = $"SELECT * FROM {_dbManager.TABLE_NAME}";

        List<TestScore> resultScores = [];
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            resultScores.Add(new TestScore(reader.GetString(0), reader.GetString(1), reader.GetInt32(2)));
        }

        Assert.Equal(sampleScores.Count, resultScores.Count);
        for (int i = 0; i < sampleScores.Count; i++)
        {
            Assert.Equal(sampleScores[i].FirstName, resultScores[i].FirstName);
            Assert.Equal(sampleScores[i].LastName, resultScores[i].LastName);
            Assert.Equal(sampleScores[i].Score, resultScores[i].Score);
        }
    }

    [Fact]
    public void WhenGetTopScorers_ResultStringCorrect()
    {
        List<TestScore> sampleScores = [
            new TestScore("Dee", "Moore", 56),
            new TestScore("Sipho","Lolo",78),
            new TestScore("Noosrat","Hoosain",64),
            new TestScore("George","Of The Jungle",78)
        ];

        _dbManager.ImportScores(sampleScores);
        string consolResult = CaptureConsoleOutput(_dbManager.OutputTopScorers);

        Assert.Equal("George Of The Jungle\nSipho Lolo\nScore: 78\n", consolResult);
    }

    [Fact]
    public void WhenImportExistingName_ScoreUpdatedToLatestValue()
    {
        List<TestScore> sampleScores = [
            new TestScore("Dee", "Moore", 56),
            new TestScore("Sipho","Lolo",78),
            new TestScore("Noosrat","Hoosain",64),
            new TestScore("George","Of The Jungle",78)
        ];

        _dbManager.ImportScores(sampleScores);

        _dbManager.ImportScores([
            new TestScore("Dee", "Moore", 90)
        ]);

        using var command = _connection.CreateCommand();
        command.CommandText = $@"
            SELECT Score 
            FROM {_dbManager.TABLE_NAME}
            WHERE FirstName = 'Dee' AND LastName = 'Moore'";
        var result = Convert.ToInt32(command.ExecuteScalar());

        Assert.Equal(90, result);
    }

    public void Dispose()
    {
        _connection.Close();
        _connection.Dispose();
    }
}