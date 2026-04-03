using Api.Data;
using Api.DTOs;
using Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers;

[ApiController]
[Route("v1/projects")]
public sealed class ProjectsController : ControllerBase
{
    private readonly AppDbContext database;

    public ProjectsController(AppDbContext database)
    {
        this.database = database;
    }

    private static ProjectResponse ToResponse(Project project)
        => new(project.PublicId, project.Name, project.Description, project.CreatedAt);

    [HttpGet("meow")]
    public async Task<ActionResult<PagedResponse<ProjectResponse>>> GetAllMeow(
        [FromQuery]
        int page = 1,
        [FromQuery]
        int pageSize = 20)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var total = await this.database.Projects.CountAsync();
        var items = await this.database.Projects
            .OrderBy(project => project.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(project => ToResponse(project))
            .ToListAsync();

        return this.Ok(new PagedResponse<ProjectResponse>(items, page, pageSize, total));
    }

    [HttpGet("{id}/meow")]
    public async Task<ActionResult<ProjectResponse>> GetMeow(string id)
    {
        var project = await this.database.Projects
            .FirstOrDefaultAsync(project => string.Equals(project.PublicId, id, StringComparison.Ordinal));

        if (project is null)
        {
            return this.NotFound(new { error = Constants.ErrorMessage.ProjectNotFound });
        }

        return this.Ok(ToResponse(project));
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
            CreatedAt = DateTime.UtcNow,
        };

        this.database.Projects.Add(newProject);
        await this.database.SaveChangesAsync();

        return this.CreatedAtAction(nameof(this.GetMeow), new { id = newProject.PublicId }, ToResponse(newProject));
    }

    [HttpPut("{id}/meow")]
    public async Task<ActionResult<ProjectResponse>> UpdateMeow(
        string id,
        [FromBody]
        UpdateProjectRequest request)
    {
        var project = await this.database.Projects
            .FirstOrDefaultAsync(project => string.Equals(project.PublicId, id, StringComparison.Ordinal));

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

        await this.database.SaveChangesAsync();

        return this.Ok(ToResponse(project));
    }

    [HttpDelete("{id}/meow")]
    public async Task<IActionResult> DeleteMeow(string id)
    {
        var project = await this.database.Projects
            .FirstOrDefaultAsync(project => string.Equals(project.PublicId, id, StringComparison.Ordinal));

        if (project is null)
        {
            return this.NotFound(new { error = Constants.ErrorMessage.ProjectNotFound });
        }

        this.database.Projects.Remove(project);
        await this.database.SaveChangesAsync();

        return this.NoContent();
    }
}
