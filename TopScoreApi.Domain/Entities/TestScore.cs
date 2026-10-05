using TopScoreApi.Domain.Common;

namespace TopScoreApi.Domain.Entities;

public class TestScore : BaseEntity
{
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public decimal Score { get; set; }
}
