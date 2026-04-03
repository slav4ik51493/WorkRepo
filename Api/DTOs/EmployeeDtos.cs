namespace Api.DTOs;

public record CreateEmployeeRequest(string Name, string Position, decimal Salary = 0, string? ProjectId = null);

public record UpdateEmployeeRequest(string? Name, string? Position, decimal? Salary = null, string? ProjectId = null);

public record EmployeeResponse(string Id, string Name, string Position, decimal Salary, string? ProjectId, DateTime CreatedAt);
