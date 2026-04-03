using Api.Exceptions;
using Api.Models;
using Api.Repositories.Abstractions;
using Api.Services.Abstractions;

namespace Api.Services;

public sealed class EmployeeService : IEmployeeService
{
    private readonly IEmployeeRepository employeeRepository;

    private readonly IProjectRepository projectRepository;

    public EmployeeService(
        IEmployeeRepository employeeRepository,
        IProjectRepository projectRepository)
    {
        this.employeeRepository = employeeRepository;
        this.projectRepository = projectRepository;
    }

    public async Task AssignToProjectAsync(string employeePublicId, string projectPublicId)
    {
        var employee = await this.employeeRepository.FindWithProjectAsync(employeePublicId);

        if (employee is null)
        {
            throw
              new KeyNotFoundException(Constants.ErrorMessage.EmployeeNotFound);
        }

        var project = await this.projectRepository.FindByPublicIdAsync(projectPublicId);

        if (project is null)
        {
            throw
              new KeyNotFoundException(Constants.ErrorMessage.ProjectNotFound);
        }

        if (project.Status == ProjectStatus.Archived)
        {
            throw
              new ProjectArchivedException(project.Name);
        }

        await this.ValidateBudgetAsync(employee, project);

        employee.ProjectId = project.Id;
        employee.Project = project;

        await this.employeeRepository.SaveAsync();
    }

    public async Task TransferToProjectAsync(string employeePublicId, string targetProjectPublicId)
    {
        var employee = await this.employeeRepository.FindWithProjectAsync(employeePublicId);

        if (employee is null)
        {
            throw
              new KeyNotFoundException(Constants.ErrorMessage.EmployeeNotFound);
        }

        if (employee.ProjectId is null)
        {
            throw
              new BusinessException(Constants.ErrorMessage.EmployeeNotAssigned);
        }

        var targetProject = await this.projectRepository.FindByPublicIdAsync(targetProjectPublicId);

        if (targetProject is null)
        {
            throw
              new KeyNotFoundException(Constants.ErrorMessage.ProjectNotFound);
        }

        if (employee.ProjectId == targetProject.Id)
        {
            throw
              new BusinessException(Constants.ErrorMessage.EmployeeAlreadyOnProject);
        }

        if (targetProject.Status == ProjectStatus.Archived)
        {
            throw
              new ProjectArchivedException(targetProject.Name);
        }

        await this.ValidateBudgetAsync(employee, targetProject);

        employee.ProjectId = targetProject.Id;
        employee.Project = targetProject;

        await this.employeeRepository.SaveAsync();
    }

    private async Task ValidateBudgetAsync(Employee employee, Project project)
    {
        if (project.Budget <= 0)
        {
            return;
        }

        var currentTotal = await this.employeeRepository.GetTotalSalaryByProjectAsync(project.Id);
        var required = currentTotal + employee.Salary;

        if (required > project.Budget)
        {
            throw
              new BudgetExceededException(project.Name, project.Budget, required);
        }
    }
}
