using Microsoft.EntityFrameworkCore;
using TopScoreApi.Application.Common.Interfaces;
using TopScoreApi.Domain.Entities;

namespace TopScoreApi.Infrastructure.Persistence.DataAccess;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<TestScore> TestScores { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<TestScore>(entity =>
        {
            entity.Property(s => s.FirstName).IsRequired();
            entity.Property(s => s.LastName).IsRequired();

            entity.ToTable(t =>
            {
                t.HasCheckConstraint(name: "CK_TestScore_FirstName_NotEmpty", sql: "length(FirstName) > 0");
                t.HasCheckConstraint(name: "CK_TestScore_LastName_NotEmpty", sql: "length(LastName) > 0");
            });

            entity.HasKey(s => new { s.FirstName, s.LastName });
        });


    }
}

