namespace Api.DTOs;

public record CreateEmployeeRequest(string Name, string Position, string? ProjectId);

public record UpdateEmployeeRequest(string? Name, string? Position, string? ProjectId);

public record EmployeeResponse(string Id, string Name, string Position, string? ProjectId, DateTime CreatedAt);
