using MediatR;
using Microsoft.EntityFrameworkCore;
using TopScoreApi.Application.Common.Interfaces;
using TopScoreApi.Application.Models;

namespace TopScoreApi.Application.Queries;

public record GetPersonScoreQuery(string FirstName, string LastName) : IRequest<ScoreDto?>;

public class GetPersonScoreQueryHandler(IApplicationDbContext context) : IRequestHandler<GetPersonScoreQuery, ScoreDto?>
{
    public async Task<ScoreDto?> Handle(GetPersonScoreQuery request, CancellationToken cancellationToken)
        => await context.TestScores
                .AsNoTracking()
                .Where(s => s.FirstName == request.FirstName && s.LastName == request.LastName)
                .Select(s => new ScoreDto
                {
                    FirstName = s.FirstName,
                    LastName = s.LastName,
                    Score = s.Score
                }).FirstOrDefaultAsync(cancellationToken: cancellationToken);
}
