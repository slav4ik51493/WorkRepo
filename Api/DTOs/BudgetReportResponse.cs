namespace Api.DTOs;

public record BudgetReportResponse(
    string ProjectId,
    string ProjectName,
    decimal Budget,
    decimal TotalSalaries,
    decimal Remaining,
    bool IsOverBudget,
    int EmployeeCount);
