namespace TopScorerCLI.Tests;

public class TestScoresTest
{
    [Fact]
    public void WhenValidRowsProvidedWithSingleTopScore_ReturnSingleNameAndScore()
    {
        string[] testInput = [
            "a,a,1",
            "c,c,3",
            "b,b,2"
        ];

        var scores = new TestScores(testInput);

        var result = scores.TopScorersResult();

        Assert.Equal(3, scores.TopScore);
        Assert.Single(scores.TopScorers());

        Assert.Equal("c c\nScore: 3", result);
    }

    [Fact]
    public void WhenValidRowsProvidedWithMultipleTopScores_ReturnMultipleOrderedNamesAndScore()
    {
        string[] testInput = [
            "c,c,50",
            "a,a,1",
            "b,b,50",
            "d,d,25"
        ];

        var scores = new TestScores(testInput);

        var result = scores.TopScorersResult();

        Assert.Equal(50, scores.TopScore);
        Assert.Equal(2, scores.TopScorers().Count);

        Assert.Equal("b b\nc c\nScore: 50", result);
    }

    [Fact]
    public void WhenNoValuesProved_ThrowError()
    {
        var scores = new TestScores([]);

        var result = scores.TopScorersResult();

        Assert.Empty(scores.Scores);
        Assert.Equal("", result);
    }
}
