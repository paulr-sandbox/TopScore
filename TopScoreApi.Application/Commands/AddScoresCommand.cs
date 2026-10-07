using MediatR;
using Microsoft.EntityFrameworkCore;
using TopScoreApi.Application.Common.Interfaces;
using TopScoreApi.Application.Models;
using TopScoreApi.Domain.Entities;

namespace TopScoreApi.Application.Commands;

public record UpsertScoresResultDto(int InsertedCount, int UpdatedCount);
public record AddScoresCommand(List<ScoreDto> Scores) : IRequest<UpsertScoresResultDto>;

public class AddScoresCommandHandler(IApplicationDbContext context) : IRequestHandler<AddScoresCommand, UpsertScoresResultDto>
{
    public async Task<UpsertScoresResultDto> Handle(AddScoresCommand request, CancellationToken cancellationToken)
    {
        if (request.Scores.Count == 0)
        {
            return new UpsertScoresResultDto(0, 0);
        }

        var requestFirstNames = request.Scores.Select(s => s.FirstName).Distinct().ToList();
        var requestLastNames = request.Scores.Select(s => s.LastName).Distinct().ToList();

        var existingPersons = await context.TestScores
            .Where(s => requestFirstNames.Contains(s.FirstName) && requestLastNames.Contains(s.LastName))
            .ToListAsync();

        var existingPersonsLookup = existingPersons.ToDictionary(
            s => $"{s.FirstName} {s.LastName}",
            s => s
        );

        int insertedCount = 0;
        int updatedCount = 0;

        foreach (var scoreDto in request.Scores)
        {
            var firstName = scoreDto.FirstName;
            var lastName = scoreDto.LastName;
            var score = scoreDto.Score;
            var lookupKey = $"{firstName} {lastName}";

            if (existingPersonsLookup.TryGetValue(lookupKey, out var existingScore))
            {
                existingScore.Score = score;
                updatedCount++;
            }
            else
            {
                var newScore = new TestScore()
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Score = score
                };
                context.TestScores.Add(newScore);
                insertedCount++;

                existingPersonsLookup[lookupKey] = newScore;
            }
        }
        await context.SaveChangesAsync(cancellationToken);

        return new UpsertScoresResultDto(insertedCount, updatedCount);
    }
}