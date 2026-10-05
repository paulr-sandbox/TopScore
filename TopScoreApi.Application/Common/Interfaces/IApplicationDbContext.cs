using Microsoft.EntityFrameworkCore;
using TopScoreApi.Domain.Entities;

namespace TopScoreApi.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<TestScore> TestScores { get; }
}