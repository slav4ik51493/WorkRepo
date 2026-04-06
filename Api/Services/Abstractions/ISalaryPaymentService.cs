using Api.Models;

namespace Api.Services.Abstractions;

public interface ISalaryPaymentService
{
    Task<SalaryPayment> LogPaymentAsync(string employeePublicId, decimal amount, string? note);
}
