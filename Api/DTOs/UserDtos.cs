namespace Api.DTOs;

public record CreateUserRequest(string Name, string Email);

public record UpdateUserRequest(string? Name, string? Email);

public record UserResponse(string Id, string Name, string Email, DateTime CreatedAt);
