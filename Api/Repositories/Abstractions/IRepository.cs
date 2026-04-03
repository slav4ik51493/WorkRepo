using Api.Models.Base;

namespace Api.Repositories.Abstractions;

public interface IRepository<T> where T : BaseEntity
{
    Task<T?> FindByPublicIdAsync(string publicId);

    Task<PagedResult<T>> GetPageAsync(int page, int pageSize);

    void Add(T entity);

    void Remove(T entity);

    Task SaveAsync();
}

public record PagedResult<T>(IReadOnlyList<T> Items, int Total);
