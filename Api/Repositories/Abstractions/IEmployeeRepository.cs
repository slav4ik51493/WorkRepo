using Api.Models;

namespace Api.Repositories.Abstractions;

public interface IEmployeeRepository : IRepository<Employee>
{
    Task<Employee?> FindWithProjectAsync(string publicId);

    Task<PagedResult<Employee>> GetPageByProjectAsync(int page, int pageSize, int projectId);

    Task<decimal> GetTotalSalaryByProjectAsync(int projectId);
}
