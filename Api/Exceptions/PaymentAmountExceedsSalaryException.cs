namespace Api.Exceptions;

public sealed class PaymentAmountExceedsSalaryException : BusinessException
{
    public decimal Amount { get; }

    public decimal Salary { get; }

    public PaymentAmountExceedsSalaryException(
        string employeeName,
        decimal amount,
        decimal salary)
        : base($"Payment amount {amount:F2} exceeds the salary of employee '{employeeName}' ({salary:F2}).")
    {
        this.Amount = amount;
        this.Salary = salary;
    }
}
