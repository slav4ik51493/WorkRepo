using Api.Models.Base;

namespace Api.Models;

public sealed class Employee : BaseEntity
{
    public string Name { get; set; } = default!;

    public string Position { get; set; } = default!;

    public decimal Salary { get; set; }

    public int? ProjectId { get; set; }

    public Project? Project { get; set; }
}
