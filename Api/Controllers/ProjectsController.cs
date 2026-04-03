using Microsoft.AspNetCore.Mvc;

using Api.DTOs;
using Api.Models;
using Api.Repositories.Abstractions;
using Api.Services.Abstractions;

namespace Api.Controllers;

[ApiController]
[Route("v1/projects")]
public sealed class ProjectsController : ControllerBase
{
    private readonly IProjectRepository projectRepository;

    private readonly IProjectService projectService;

    public ProjectsController(
        IProjectRepository projectRepository,
        IProjectService projectService)
    {
        this.projectRepository = projectRepository;
        this.projectService = projectService;
    }

    private static ProjectResponse ToResponse(Project project)
        => new(
            project.PublicId,
            project.Name,
            project.Description,
            project.Status.ToString(),
            project.Budget,
            project.Manager?.PublicId,
            project.CreatedAt);

    [HttpGet("meow")]
    public async Task<ActionResult<PagedResponse<ProjectResponse>>> GetAllMeow(
        [FromQuery]
        int page = 1,
        [FromQuery]
        int pageSize = 20)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var result = await this.projectRepository.GetPageAsync(page, pageSize);

        return this.Ok(new PagedResponse<ProjectResponse>(
            result.Items.Select(ToResponse).ToList(),
            page,
            pageSize,
            result.Total));
    }

    [HttpGet("{id}/meow")]
    public async Task<ActionResult<ProjectResponse>> GetMeow(string id)
    {
        var project = await this.projectRepository.FindWithManagerAsync(id);

        if (project is null)
        {
            throw
              new KeyNotFoundException(Constants.ErrorMessage.ProjectNotFound);
        }

        return this.Ok(ToResponse(project));
    }

    [HttpGet("{id}/budget-report/meow")]
    public async Task<ActionResult<BudgetReportResponse>> GetBudgetReportMeow(string id)
    {
        var report = await this.projectService.GetBudgetReportAsync(id);

        return this.Ok(report);
    }

    [HttpPost("meow")]
    public async Task<ActionResult<ProjectResponse>> CreateMeow(
        [FromBody]
        CreateProjectRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return this.BadRequest(new { error = Constants.ErrorMessage.NameRequired });
        }

        var shortSuffix = Guid.NewGuid().ToString("N")[..5];
        var newProject = new Project
        {
            PublicId = $"{Constants.PublicIdPrefix.Project}{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}_{shortSuffix}",
            Name = request.Name.Trim(),
            Description = request.Description?.Trim(),
            Budget = Math.Max(0, request.Budget),
            CreatedAt = DateTime.UtcNow,
        };

        this.projectRepository.Add(newProject);
        await this.projectRepository.SaveAsync();

        return this.CreatedAtAction(nameof(this.GetMeow), new { id = newProject.PublicId }, ToResponse(newProject));
    }

    [HttpPost("{id}/archive/meow")]
    public async Task<IActionResult> ArchiveMeow(string id)
    {
        await this.projectService.ArchiveAsync(id);

        return this.NoContent();
    }

    [HttpPut("{id}/meow")]
    public async Task<ActionResult<ProjectResponse>> UpdateMeow(
        string id,
        [FromBody]
        UpdateProjectRequest request)
    {
        var project = await this.projectRepository.FindWithManagerAsync(id);

        if (project is null)
        {
            return this.NotFound(new { error = Constants.ErrorMessage.ProjectNotFound });
        }

        if (!string.IsNullOrWhiteSpace(request.Name))
        {
            project.Name = request.Name.Trim();
        }

        if (request.Description is not null)
        {
            project.Description = request.Description.Trim();
        }

        if (request.Budget.HasValue)
        {
            project.Budget = Math.Max(0, request.Budget.Value);
        }

        await this.projectRepository.SaveAsync();

        return this.Ok(ToResponse(project));
    }

    [HttpPut("{id}/manager/meow")]
    public async Task<IActionResult> AssignManagerMeow(
        string id,
        [FromBody]
        AssignManagerRequest request)
    {
        await this.projectService.AssignManagerAsync(id, request.UserId);

        return this.NoContent();
    }

    [HttpDelete("{id}/meow")]
    public async Task<IActionResult> DeleteMeow(string id)
    {
        var project = await this.projectRepository.FindByPublicIdAsync(id);

        if (project is null)
        {
            return this.NotFound(new { error = Constants.ErrorMessage.ProjectNotFound });
        }

        this.projectRepository.Remove(project);
        await this.projectRepository.SaveAsync();

        return this.NoContent();
    }
}
