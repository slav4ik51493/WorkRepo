using Api.Models;

namespace Api.Repositories.Abstractions;

public interface IProjectRepository : IRepository<Project>
{
    Task<Project?> FindWithManagerAsync(string publicId);

    Task<IReadOnlyList<Employee>> GetEmployeesAsync(int projectId);
}
