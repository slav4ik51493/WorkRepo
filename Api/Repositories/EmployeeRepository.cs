using Microsoft.EntityFrameworkCore;

using Api.Data;
using Api.Models;
using Api.Repositories.Abstractions;
using Api.Repositories.Base;

namespace Api.Repositories;

public sealed class EmployeeRepository : BaseRepository<Employee>, IEmployeeRepository
{
    public EmployeeRepository(AppDbContext database)
        : base(database)
    {
    }

    public override async Task<PagedResult<Employee>> GetPageAsync(int page, int pageSize)
    {
        var total = await this.database.Employees.CountAsync();
        var items = await this.database.Employees
            .Include(employee => employee.Project)
            .OrderBy(employee => employee.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<Employee>(items, total);
    }

    public async Task<Employee?> FindWithProjectAsync(string publicId)
    {
        return await this.database.Employees
            .Include(employee => employee.Project)
            .FirstOrDefaultAsync(employee => string.Equals(employee.PublicId, publicId, StringComparison.Ordinal));
    }

    public async Task<PagedResult<Employee>> GetPageByProjectAsync(int page, int pageSize, int projectId)
    {
        var query = this.database.Employees
            .Include(employee => employee.Project)
            .Where(employee => employee.ProjectId == projectId);

        var total = await query.CountAsync();
        var items = await query
            .OrderBy(employee => employee.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<Employee>(items, total);
    }

    public async Task<decimal> GetTotalSalaryByProjectAsync(int projectId)
    {
        return await this.database.Employees
            .Where(employee => employee.ProjectId == projectId)
            .SumAsync(employee => employee.Salary);
    }
}
