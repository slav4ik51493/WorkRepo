namespace Api.Models;

public sealed class Project
{
    public int Id { get; set; }

    public string PublicId { get; set; } = default!;

    public string Name { get; set; } = default!;

    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; }

    public ICollection<Employee> Employees { get; set; } = [];
}
