using Microsoft.EntityFrameworkCore;

using Api.Data;
using Api.Models;
using Api.Repositories.Abstractions;
using Api.Repositories.Base;

namespace Api.Repositories;

public sealed class ProjectRepository : BaseRepository<Project>, IProjectRepository
{
    public ProjectRepository(AppDbContext database)
        : base(database)
    {
    }

    public override async Task<PagedResult<Project>> GetPageAsync(int page, int pageSize)
    {
        var total = await this.database.Projects.CountAsync();
        var items = await this.database.Projects
            .Include(project => project.Manager)
            .OrderBy(project => project.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<Project>(items, total);
    }

    public async Task<Project?> FindWithManagerAsync(string publicId)
    {
        return await this.database.Projects
            .Include(project => project.Manager)
            .FirstOrDefaultAsync(project => string.Equals(project.PublicId, publicId, StringComparison.Ordinal));
    }

    public async Task<IReadOnlyList<Employee>> GetEmployeesAsync(int projectId)
    {
        return await this.database.Employees
            .Where(employee => employee.ProjectId == projectId)
            .ToListAsync();
    }
}
