using Microsoft.Data.Sqlite;

namespace TopScorerCLI;

public class DatabaseManager
{
    private readonly string _connectionString;

    private readonly string TABLE_NAME = "TestScores";

    public DatabaseManager(string databaseFile = "database.db")
    {
        _connectionString = $"Data Source={databaseFile}";
    }

    public void InitialiseDatabase()
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        var createTableCmd = connection.CreateCommand();
        createTableCmd.CommandText = $@"
                CREATE TABLE IF NOT EXISTS {TABLE_NAME} (
                    FirstName TEXT,
                    LastName TEXT,
                    Score INTEGER,
                    PRIMARY KEY (FirstName, LastName)
                );";
        createTableCmd.ExecuteNonQuery();
    }

    public void ImportScores(string filePath)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        using var transaction = connection.BeginTransaction();
        var insertCommand = connection.CreateCommand();
        insertCommand.CommandText = $@"
                    INSERT INTO {TABLE_NAME} (FirstName, LastName, Score)
                    VALUES ($firstName, $lastName, $score)
                    ON CONFLICT(FirstName, LastName)
                    DO UPDATE SET Score = excluded.Score;
                    ";
        var pFirst = insertCommand.CreateParameter(); pFirst.ParameterName = "$firstName"; insertCommand.Parameters.Add(pFirst);
        var pLast = insertCommand.CreateParameter(); pLast.ParameterName = "$lastName"; insertCommand.Parameters.Add(pLast);
        var pScore = insertCommand.CreateParameter(); pScore.ParameterName = "$score"; insertCommand.Parameters.Add(pScore);

        using StreamReader reader = new(filePath);

        _ = reader.ReadLine(); // Discard header row
        string? line;
        while ((line = reader.ReadLine()) != null)
        {
            var values = line.Split(',');
            pFirst.Value = values[0].Split();
            pLast.Value = values[1].Split();
            if (int.TryParse(values[2].Trim(), out int scoreValue))
            {
                pScore.Value = scoreValue;
            }
            else
            {
                pScore.Value = 0; // TODO: Decide on setting to zero or failing
            }

            insertCommand.ExecuteNonQuery();
        }

        transaction.Commit();
    }

    public void ImportScores(List<DataRow> rows)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        using var transaction = connection.BeginTransaction();
        var insertCommand = connection.CreateCommand();
        insertCommand.CommandText = $@"
                    INSERT INTO {TABLE_NAME} (FirstName, LastName, Score)
                    VALUES ($firstName, $lastName, $score)
                    ON CONFLICT(FirstName, LastName)
                    DO UPDATE SET Score = excluded.Score;
                    ";
        var pFirst = insertCommand.CreateParameter(); pFirst.ParameterName = "$firstName"; insertCommand.Parameters.Add(pFirst);
        var pLast = insertCommand.CreateParameter(); pLast.ParameterName = "$lastName"; insertCommand.Parameters.Add(pLast);
        var pScore = insertCommand.CreateParameter(); pScore.ParameterName = "$score"; insertCommand.Parameters.Add(pScore);

        foreach (var row in rows)
        {
            pFirst.Value = row.FirstName;
            pLast.Value = row.LastName;
            pScore.Value = row.Score;

            insertCommand.ExecuteNonQuery();
        }

        transaction.Commit();
    }

    public void OutputTopScorers()
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var selectCommand = connection.CreateCommand();
        selectCommand.CommandText = $@"
                SELECT FirstName, LastName, Score
                FROM {TABLE_NAME}
                WHERE Score = (SELECT MAX(Score) FROM {TABLE_NAME})
                ORDER BY FirstName, LastName
            ";

        using var dbReader = selectCommand.ExecuteReader();
        int? topScore = null;
        while (dbReader.Read())
        {
            topScore ??= dbReader.GetInt32(2);
            Console.WriteLine($"{dbReader.GetString(0)} {dbReader.GetString(1)}");
        }
        Console.WriteLine($"Score: {topScore}");
    }
}