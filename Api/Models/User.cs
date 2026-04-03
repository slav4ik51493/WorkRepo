using Api.Models.Base;

namespace Api.Models;

public sealed class User : BaseEntity
{
    public string Name { get; set; } = default!;

    public string Email { get; set; } = default!;
}
