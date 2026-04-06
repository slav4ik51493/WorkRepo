using Api.Models;

namespace Api.Repositories.Abstractions;

public interface ISalaryPaymentRepository : IRepository<SalaryPayment>
{
    Task<SalaryPayment?> FindWithEmployeeAsync(string publicId);

    Task<PagedResult<SalaryPayment>> GetPageByEmployeeAsync(int page, int pageSize, int employeeId);
}
