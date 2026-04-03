namespace Api.Models;

public sealed class Employee
{
    public int Id { get; set; }

    public string PublicId { get; set; } = default!;

    public string Name { get; set; } = default!;

    public string Position { get; set; } = default!;

    public DateTime CreatedAt { get; set; }

    public int? ProjectId { get; set; }

    public Project? Project { get; set; }
}
