namespace Api.Models;

public sealed class User
{
    public int Id { get; set; }

    public string PublicId { get; set; } = default!;

    public string Name { get; set; } = default!;

    public string Email { get; set; } = default!;

    public DateTime CreatedAt { get; set; }
}
