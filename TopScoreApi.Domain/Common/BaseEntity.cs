namespace TopScoreApi.Domain.Common;

public class BaseEntity
{
    public int Id { get; set; }
    public Guid Identifier { get; set; } = Guid.NewGuid();
}