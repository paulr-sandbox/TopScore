namespace TopScorerCLI.Tests;

public class TestScoresTest
{
    [Fact]
    public void WhenValidRowsProvidedWithSingleTopScore_ReturnSingleNameAndScore()
    {
        List<string> testInput = [
            "First Name, Last Name, Score",
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
        List<string> testInput = [
            "First Name, Last Name, Score",
            "Dee,Moore,56",
            "Sipho,Lolo,78",
            "Noosrat,Hoosain,64",
            "George,Of The Jungle,78"
        ];

        var scores = new TestScores(testInput);

        var result = scores.TopScorersResult();

        Assert.Equal(78, scores.TopScore);
        Assert.Equal(2, scores.TopScorers().Count);

        Assert.Equal("George Of The Jungle\nSipho Lolo\nScore: 78", result);
    }

    [Fact]
    public void WhenNoValuesProvided_ThrowError()
    {
        Assert.Throws<InvalidDataException>(() => new TestScores([]));
    }

    [Fact]
    public void WhenNoDataValuesProvided_ThrowError()
    {
        Assert.Throws<InvalidDataException>(() => new TestScores(["First Name, Last Name, Score"]));
    }

    [Fact]
    public void WhenInvalidHeaderLine_ThrowError()
    {
        Assert.Throws<InvalidDataException>(() => new TestScores([
            "First Name, Last Name",
            "a,a"
        ]));
    }
}
