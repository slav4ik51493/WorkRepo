namespace Api.DTOs;

public record AssignManagerRequest(string UserId);

public record AssignProjectRequest(string ProjectId);

public record TransferProjectRequest(string TargetProjectId);
