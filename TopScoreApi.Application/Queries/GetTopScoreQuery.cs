using MediatR;
using Microsoft.EntityFrameworkCore;
using TopScoreApi.Application.Common.Interfaces;
using TopScoreApi.Application.Models;

namespace TopScoreApi.Application.Queries;

public record GetTopScoreQuery() : IRequest<TopScoreDto>;

public class GetTopScoreQueryHandler(IApplicationDbContext context) : IRequestHandler<GetTopScoreQuery, TopScoreDto>
{
    public async Task<TopScoreDto> Handle(GetTopScoreQuery request, CancellationToken cancellationToken)
    {
        var topScore = await context.TestScores.MaxAsync(s => s.Score, cancellationToken: cancellationToken);

        var topScorers = await context.TestScores
            .Where(s => s.Score == topScore)
            .Select(s => new PersonDto()
            {
                FirstName = s.FirstName,
                LastName = s.LastName
            }
            ).ToListAsync(cancellationToken: cancellationToken);

        return new TopScoreDto()
        {
            Scorers = topScorers,
            Score = topScore
        };
    }
}