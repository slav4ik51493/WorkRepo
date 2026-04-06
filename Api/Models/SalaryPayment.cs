using Api.Models.Base;

namespace Api.Models;

public sealed class SalaryPayment : BaseEntity
{
    public int EmployeeId { get; set; }

    public Employee Employee { get; set; } = default!;

    public decimal Amount { get; set; }

    public string? Note { get; set; }
}
