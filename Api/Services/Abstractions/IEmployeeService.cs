namespace Api.Services.Abstractions;

public interface IEmployeeService
{
    Task AssignToProjectAsync(string employeePublicId, string projectPublicId);

    Task TransferToProjectAsync(string employeePublicId, string targetProjectPublicId);
}
