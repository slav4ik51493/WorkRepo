using Api.Exceptions;
using Api.Models;
using Api.Repositories.Abstractions;
using Api.Services.Abstractions;

namespace Api.Services;

public sealed class SalaryPaymentService : ISalaryPaymentService
{
    private readonly IEmployeeRepository employeeRepository;

    private readonly ISalaryPaymentRepository salaryPaymentRepository;

    public SalaryPaymentService(
        IEmployeeRepository employeeRepository,
        ISalaryPaymentRepository salaryPaymentRepository)
    {
        this.employeeRepository = employeeRepository;
        this.salaryPaymentRepository = salaryPaymentRepository;
    }

    public async Task<SalaryPayment> LogPaymentAsync(string employeePublicId, decimal amount, string? note)
    {
        if (amount <= 0)
        {
            throw
              new InvalidPaymentAmountException(amount);
        }

        var employee = await this.employeeRepository.FindByPublicIdAsync(employeePublicId);

        if (employee is null)
        {
            throw
              new KeyNotFoundException(Constants.ErrorMessage.EmployeeNotFound);
        }

        if (employee.Salary > 0 && amount > employee.Salary)
        {
            throw
              new PaymentAmountExceedsSalaryException(employee.Name, amount, employee.Salary);
        }

        var shortSuffix = Guid.NewGuid().ToString("N")[..5];
        var payment = new SalaryPayment
        {
            PublicId = $"{Constants.PublicIdPrefix.SalaryPayment}{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}_{shortSuffix}",
            EmployeeId = employee.Id,
            Employee = employee,
            Amount = amount,
            Note = note,
            CreatedAt = DateTime.UtcNow,
        };

        this.salaryPaymentRepository.Add(payment);
        await this.salaryPaymentRepository.SaveAsync();

        return payment;
    }
}
