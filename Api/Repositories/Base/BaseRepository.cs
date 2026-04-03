using Microsoft.EntityFrameworkCore;

using Api.Data;
using Api.Models.Base;
using Api.Repositories.Abstractions;

namespace Api.Repositories.Base;

public abstract class BaseRepository<T> : IRepository<T> where T : BaseEntity
{
    protected readonly AppDbContext database;

    protected BaseRepository(AppDbContext database)
    {
        this.database = database;
    }

    public async Task<T?> FindByPublicIdAsync(string publicId)
    {
        return await this.database.Set<T>()
            .FirstOrDefaultAsync(entity => string.Equals(entity.PublicId, publicId, StringComparison.Ordinal));
    }

    public virtual async Task<PagedResult<T>> GetPageAsync(int page, int pageSize)
    {
        var total = await this.database.Set<T>().CountAsync();
        var items = await this.database.Set<T>()
            .OrderBy(entity => entity.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<T>(items, total);
    }

    public void Add(T entity)
    {
        this.database.Set<T>().Add(entity);
    }

    public void Remove(T entity)
    {
        this.database.Set<T>().Remove(entity);
    }

    public async Task SaveAsync()
    {
        await this.database.SaveChangesAsync();
    }
}
