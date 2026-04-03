using Api.DTOs;
using Api.Exceptions;
using Api.Models;
using Api.Repositories.Abstractions;
using Api.Services.Abstractions;

namespace Api.Services;

public sealed class ProjectService : IProjectService
{
    private readonly IProjectRepository projectRepository;

    private readonly IUserRepository userRepository;

    private readonly IEmployeeRepository employeeRepository;

    public ProjectService(
        IProjectRepository projectRepository,
        IUserRepository userRepository,
        IEmployeeRepository employeeRepository)
    {
        this.projectRepository = projectRepository;
        this.userRepository = userRepository;
        this.employeeRepository = employeeRepository;
    }

    public async Task ArchiveAsync(string projectPublicId)
    {
        var project = await this.projectRepository.FindByPublicIdAsync(projectPublicId);

        if (project is null)
        {
            throw
              new KeyNotFoundException(Constants.ErrorMessage.ProjectNotFound);
        }

        if (project.Status == ProjectStatus.Archived)
        {
            throw
              new BusinessException(Constants.ErrorMessage.ProjectAlreadyArchived);
        }

        project.Status = ProjectStatus.Archived;

        var employees = await this.projectRepository.GetEmployeesAsync(project.Id);

        foreach (var employee in employees)
        {
            employee.ProjectId = null;
            employee.Project = null;
        }

        await this.projectRepository.SaveAsync();
    }

    public async Task AssignManagerAsync(string projectPublicId, string userPublicId)
    {
        var project = await this.projectRepository.FindByPublicIdAsync(projectPublicId);

        if (project is null)
        {
            throw
              new KeyNotFoundException(Constants.ErrorMessage.ProjectNotFound);
        }

        var manager = await this.userRepository.FindByPublicIdAsync(userPublicId);

        if (manager is null)
        {
            throw
              new KeyNotFoundException(Constants.ErrorMessage.UserNotFound);
        }

        project.ManagerId = manager.Id;
        project.Manager = manager;

        await this.projectRepository.SaveAsync();
    }

    public async Task<BudgetReportResponse> GetBudgetReportAsync(string projectPublicId)
    {
        var project = await this.projectRepository.FindWithManagerAsync(projectPublicId);

        if (project is null)
        {
            throw
              new KeyNotFoundException(Constants.ErrorMessage.ProjectNotFound);
        }

        var totalSalaries = await this.employeeRepository.GetTotalSalaryByProjectAsync(project.Id);
        var employees = await this.projectRepository.GetEmployeesAsync(project.Id);
        var remaining = project.Budget - totalSalaries;
        var isOverBudget = project.Budget > 0 && totalSalaries > project.Budget;

        return new BudgetReportResponse(
            project.PublicId,
            project.Name,
            project.Budget,
            totalSalaries,
            remaining,
            isOverBudget,
            employees.Count);
    }
}
