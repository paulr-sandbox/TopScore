namespace TopScoreApi.Application.Models;

public class TopScoreDto
{
    public List<PersonDto> Scorers { get; set; } = [];
    public int Score { get; set; }
}