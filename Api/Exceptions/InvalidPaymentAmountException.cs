namespace Api.Exceptions;

public sealed class InvalidPaymentAmountException : BusinessException
{
    public decimal Amount { get; }

    public InvalidPaymentAmountException(decimal amount)
        : base($"Payment amount must be greater than zero, but got {amount:F2}.")
    {
        this.Amount = amount;
    }
}
