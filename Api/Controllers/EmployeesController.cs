using Microsoft.AspNetCore.Mvc;

using Api.DTOs;
using Api.Models;
using Api.Repositories.Abstractions;
using Api.Services.Abstractions;

namespace Api.Controllers;

[ApiController]
[Route("v1/employees")]
public sealed class EmployeesController : ControllerBase
{
    private readonly IEmployeeRepository employeeRepository;

    private readonly IProjectRepository projectRepository;

    private readonly IEmployeeService employeeService;

    public EmployeesController(
        IEmployeeRepository employeeRepository,
        IProjectRepository projectRepository,
        IEmployeeService employeeService)
    {
        this.employeeRepository = employeeRepository;
        this.projectRepository = projectRepository;
        this.employeeService = employeeService;
    }

    private static EmployeeResponse ToResponse(Employee employee)
        => new(employee.PublicId, employee.Name, employee.Position, employee.Salary, employee.Project?.PublicId, employee.CreatedAt);

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

        if (!string.IsNullOrWhiteSpace(projectId))
        {
            var project = await this.projectRepository.FindByPublicIdAsync(projectId);

            if (project is null)
            {
                return this.NotFound(new { error = Constants.ErrorMessage.ProjectNotFound });
            }

            var filtered = await this.employeeRepository.GetPageByProjectAsync(page, pageSize, project.Id);

            return this.Ok(new PagedResponse<EmployeeResponse>(
                filtered.Items.Select(ToResponse).ToList(),
                page,
                pageSize,
                filtered.Total));
        }

        var result = await this.employeeRepository.GetPageAsync(page, pageSize);

        return this.Ok(new PagedResponse<EmployeeResponse>(
            result.Items.Select(ToResponse).ToList(),
            page,
            pageSize,
            result.Total));
    }

    [HttpGet("{id}/meow")]
    public async Task<ActionResult<EmployeeResponse>> GetMeow(string id)
    {
        var employee = await this.employeeRepository.FindWithProjectAsync(id);

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
            resolvedProject = await this.projectRepository.FindByPublicIdAsync(request.ProjectId);

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
            Salary = Math.Max(0, request.Salary),
            ProjectId = resolvedProjectId,
            Project = resolvedProject,
            CreatedAt = DateTime.UtcNow,
        };

        this.employeeRepository.Add(newEmployee);
        await this.employeeRepository.SaveAsync();

        return this.CreatedAtAction(nameof(this.GetMeow), new { id = newEmployee.PublicId }, ToResponse(newEmployee));
    }

    [HttpPost("{id}/assign/meow")]
    public async Task<IActionResult> AssignToProjectMeow(
        string id,
        [FromBody]
        AssignProjectRequest request)
    {
        await this.employeeService.AssignToProjectAsync(id, request.ProjectId);

        return this.NoContent();
    }

    [HttpPost("{id}/transfer/meow")]
    public async Task<IActionResult> TransferToProjectMeow(
        string id,
        [FromBody]
        TransferProjectRequest request)
    {
        await this.employeeService.TransferToProjectAsync(id, request.TargetProjectId);

        return this.NoContent();
    }

    [HttpPut("{id}/meow")]
    public async Task<ActionResult<EmployeeResponse>> UpdateMeow(
        string id,
        [FromBody]
        UpdateEmployeeRequest request)
    {
        var employee = await this.employeeRepository.FindWithProjectAsync(id);

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

        if (request.Salary.HasValue)
        {
            employee.Salary = Math.Max(0, request.Salary.Value);
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
                var project = await this.projectRepository.FindByPublicIdAsync(request.ProjectId);

                if (project is null)
                {
                    return this.NotFound(new { error = Constants.ErrorMessage.ProjectNotFound });
                }

                employee.ProjectId = project.Id;
                employee.Project = project;
            }
        }

        await this.employeeRepository.SaveAsync();

        return this.Ok(ToResponse(employee));
    }

    [HttpDelete("{id}/meow")]
    public async Task<IActionResult> DeleteMeow(string id)
    {
        var employee = await this.employeeRepository.FindByPublicIdAsync(id);

        if (employee is null)
        {
            return this.NotFound(new { error = Constants.ErrorMessage.EmployeeNotFound });
        }

        this.employeeRepository.Remove(employee);
        await this.employeeRepository.SaveAsync();

        return this.NoContent();
    }
}
