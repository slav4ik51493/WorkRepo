using Microsoft.EntityFrameworkCore;

using Api.Data;
using Api.Models;
using Api.Repositories.Abstractions;
using Api.Repositories.Base;

namespace Api.Repositories;

public sealed class UserRepository : BaseRepository<User>, IUserRepository
{
    public UserRepository(AppDbContext database)
        : base(database)
    {
    }

    public async Task<bool> IsEmailTakenAsync(string email, string? excludePublicId = null)
    {
        return await this.database.Users
            .AnyAsync(user => string.Equals(user.Email, email, StringComparison.OrdinalIgnoreCase)
                           && (excludePublicId == null
                            || !string.Equals(user.PublicId, excludePublicId, StringComparison.Ordinal)));
    }
}
