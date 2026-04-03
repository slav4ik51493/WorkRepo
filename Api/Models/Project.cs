using Api.Models.Base;

namespace Api.Models;

public sealed class Project : BaseEntity
{
    public string Name { get; set; } = default!;

    public string? Description { get; set; }

    public ProjectStatus Status { get; set; } = ProjectStatus.Active;

    public decimal Budget { get; set; }

    public int? ManagerId { get; set; }

    public User? Manager { get; set; }

    public ICollection<Employee> Employees { get; set; } = [];
}
