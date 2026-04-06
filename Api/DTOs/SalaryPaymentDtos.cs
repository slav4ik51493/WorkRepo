namespace Api.DTOs;

public record CreateSalaryPaymentRequest(string EmployeeId, decimal Amount, string? Note = null);

public record SalaryPaymentResponse(string Id, string EmployeeId, string EmployeeName, decimal Amount, string? Note, DateTime CreatedAt);
