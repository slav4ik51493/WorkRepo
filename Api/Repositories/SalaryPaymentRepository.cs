using Microsoft.EntityFrameworkCore;

using Api.Data;
using Api.Models;
using Api.Repositories.Abstractions;
using Api.Repositories.Base;

namespace Api.Repositories;

public sealed class SalaryPaymentRepository : BaseRepository<SalaryPayment>, ISalaryPaymentRepository
{
    public SalaryPaymentRepository(AppDbContext database)
        : base(database)
    {
    }

    public override async Task<PagedResult<SalaryPayment>> GetPageAsync(int page, int pageSize)
    {
        var total = await this.database.SalaryPayments.CountAsync();
        var items = await this.database.SalaryPayments
            .Include(payment => payment.Employee)
            .OrderBy(payment => payment.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<SalaryPayment>(items, total);
    }

    public async Task<SalaryPayment?> FindWithEmployeeAsync(string publicId)
    {
        return await this.database.SalaryPayments
            .Include(payment => payment.Employee)
            .FirstOrDefaultAsync(payment => string.Equals(payment.PublicId, publicId, StringComparison.Ordinal));
    }

    public async Task<PagedResult<SalaryPayment>> GetPageByEmployeeAsync(int page, int pageSize, int employeeId)
    {
        var query = this.database.SalaryPayments
            .Include(payment => payment.Employee)
            .Where(payment => payment.EmployeeId == employeeId);

        var total = await query.CountAsync();
        var items = await query
            .OrderBy(payment => payment.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<SalaryPayment>(items, total);
    }
}
