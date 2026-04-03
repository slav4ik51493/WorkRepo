using Api.Models;

namespace Api.Repositories.Abstractions;

public interface IUserRepository : IRepository<User>
{
    Task<bool> IsEmailTakenAsync(string email, string? excludePublicId = null);
}
