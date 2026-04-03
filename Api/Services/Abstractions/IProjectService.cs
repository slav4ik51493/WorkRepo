using Api.DTOs;

namespace Api.Services.Abstractions;

public interface IProjectService
{
    Task ArchiveAsync(string projectPublicId);

    Task AssignManagerAsync(string projectPublicId, string userPublicId);

    Task<BudgetReportResponse> GetBudgetReportAsync(string projectPublicId);
}
