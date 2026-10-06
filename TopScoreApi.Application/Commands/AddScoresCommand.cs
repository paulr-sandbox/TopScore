using MediatR;
using TopScoreApi.Application.Common.Interfaces;
using TopScoreApi.Application.Models;
using TopScoreApi.Domain.Entities;

namespace TopScoreApi.Application.Command;

public record AddScoresCommand(List<ScoreDto> Scores) : IRequest<int>;

public class AddScoresCommandHandler(IApplicationDbContext context) : IRequestHandler<AddScoresCommand, int>
{
    public async Task<int> Handle(AddScoresCommand request, CancellationToken cancellationToken)
    {
        var scoresToInsert = new List<TestScore>();

        foreach (var score in request.Scores)
        {
            scoresToInsert.Add(new TestScore
            {
                FirstName = score.FirstName,
                LastName = score.LastName,
                Score = score.Score
            });
        }

        if (scoresToInsert.Count != 0)
        {
            context.TestScores.AddRange(scoresToInsert);
            await context.SaveChangesAsync(cancellationToken);
        }

        return scoresToInsert.Count;
    }
}