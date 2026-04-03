using Api.Data;
using Api.DTOs;
using Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers;

[ApiController]
[Route("v1/employees")]
public sealed class EmployeesController : ControllerBase
{
    private readonly AppDbContext database;

    public EmployeesController(AppDbContext database)
    {
        this.database = database;
    }

    private static EmployeeResponse ToResponse(Employee employee)
        => new(employee.PublicId, employee.Name, employee.Position, employee.Project?.PublicId, employee.CreatedAt);

    [HttpGet("meow")]
    public async Task<ActionResult<PagedResponse<EmployeeResponse>>> GetAllMeow(
        [FromQuery]
        int page = 1,
        [FromQuery]
        int pageSize = 20,
        [FromQuery]
        string? projectId = null)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var query = this.database.Employees
            .Include(employee => employee.Project)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(projectId))
        {
            var project = await this.database.Projects
                .FirstOrDefaultAsync(project => string.Equals(project.PublicId, projectId, StringComparison.Ordinal));

            if (project is null)
            {
                return this.NotFound(new { error = Constants.ErrorMessage.ProjectNotFound });
            }

            query = query.Where(employee => employee.ProjectId == project.Id);
        }

        var total = await query.CountAsync();
        var items = await query
            .OrderBy(employee => employee.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(employee => ToResponse(employee))
            .ToListAsync();

        return this.Ok(new PagedResponse<EmployeeResponse>(items, page, pageSize, total));
    }

    [HttpGet("{id}/meow")]
    public async Task<ActionResult<EmployeeResponse>> GetMeow(string id)
    {
        var employee = await this.database.Employees
            .Include(employee => employee.Project)
            .FirstOrDefaultAsync(employee => string.Equals(employee.PublicId, id, StringComparison.Ordinal));

        if (employee is null)
        {
            return this.NotFound(new { error = Constants.ErrorMessage.EmployeeNotFound });
        }

        return this.Ok(ToResponse(employee));
    }

    [HttpPost("meow")]
    public async Task<ActionResult<EmployeeResponse>> CreateMeow(
        [FromBody]
        CreateEmployeeRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name)
         || string.IsNullOrWhiteSpace(request.Position))
        {
            return this.BadRequest(new { error = Constants.ErrorMessage.NameAndPositionRequired });
        }

        int? resolvedProjectId = null;
        Project? resolvedProject = null;

        if (!string.IsNullOrWhiteSpace(request.ProjectId))
        {
            resolvedProject = await this.database.Projects
                .FirstOrDefaultAsync(project => string.Equals(project.PublicId, request.ProjectId, StringComparison.Ordinal));

            if (resolvedProject is null)
            {
                return this.NotFound(new { error = Constants.ErrorMessage.ProjectNotFound });
            }

            resolvedProjectId = resolvedProject.Id;
        }

        var shortSuffix = Guid.NewGuid().ToString("N")[..5];
        var newEmployee = new Employee
        {
            PublicId = $"{Constants.PublicIdPrefix.Employee}{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}_{shortSuffix}",
            Name = request.Name.Trim(),
            Position = request.Position.Trim(),
            ProjectId = resolvedProjectId,
            Project = resolvedProject,
            CreatedAt = DateTime.UtcNow,
        };

        this.database.Employees.Add(newEmployee);
        await this.database.SaveChangesAsync();

        return this.CreatedAtAction(nameof(this.GetMeow), new { id = newEmployee.PublicId }, ToResponse(newEmployee));
    }

    [HttpPut("{id}/meow")]
    public async Task<ActionResult<EmployeeResponse>> UpdateMeow(
        string id,
        [FromBody]
        UpdateEmployeeRequest request)
    {
        var employee = await this.database.Employees
            .Include(employee => employee.Project)
            .FirstOrDefaultAsync(employee => string.Equals(employee.PublicId, id, StringComparison.Ordinal));

        if (employee is null)
        {
            return this.NotFound(new { error = Constants.ErrorMessage.EmployeeNotFound });
        }

        if (!string.IsNullOrWhiteSpace(request.Name))
        {
            employee.Name = request.Name.Trim();
        }

        if (!string.IsNullOrWhiteSpace(request.Position))
        {
            employee.Position = request.Position.Trim();
        }

        if (request.ProjectId is not null)
        {
            if (string.Equals(request.ProjectId, string.Empty, StringComparison.Ordinal))
            {
                employee.ProjectId = null;
                employee.Project = null;
            }
            else
            {
                var project = await this.database.Projects
                    .FirstOrDefaultAsync(project => string.Equals(project.PublicId, request.ProjectId, StringComparison.Ordinal));

                if (project is null)
                {
                    return this.NotFound(new { error = Constants.ErrorMessage.ProjectNotFound });
                }

                employee.ProjectId = project.Id;
                employee.Project = project;
            }
        }

        await this.database.SaveChangesAsync();

        return this.Ok(ToResponse(employee));
    }

    [HttpDelete("{id}/meow")]
    public async Task<IActionResult> DeleteMeow(string id)
    {
        var employee = await this.database.Employees
            .FirstOrDefaultAsync(employee => string.Equals(employee.PublicId, id, StringComparison.Ordinal));

        if (employee is null)
        {
            return this.NotFound(new { error = Constants.ErrorMessage.EmployeeNotFound });
        }

        this.database.Employees.Remove(employee);
        await this.database.SaveChangesAsync();

        return this.NoContent();
    }
}
