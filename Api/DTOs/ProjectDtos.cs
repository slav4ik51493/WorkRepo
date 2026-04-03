namespace Api.DTOs;

public record CreateProjectRequest(string Name, string? Description, decimal Budget = 0);

public record UpdateProjectRequest(string? Name, string? Description, decimal? Budget = null);

public record ProjectResponse(
    string Id,
    string Name,
    string? Description,
    string Status,
    decimal Budget,
    string? ManagerId,
    DateTime CreatedAt);
