namespace Api.Models.Base;

public abstract class BaseEntity
{
    public int Id { get; set; }

    public string PublicId { get; set; } = default!;

    public DateTime CreatedAt { get; set; }
}
