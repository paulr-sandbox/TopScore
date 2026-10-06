using Microsoft.EntityFrameworkCore;
using TopScoreApi.Application.Common.Interfaces;
using TopScoreApi.Domain.Entities;

namespace TopScoreApi.Infrastructure.Persistence.DataAccess;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Score> TestScores { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Score>()
            .HasKey(s => new { s.FirstName, s.LastName });
    }
}
