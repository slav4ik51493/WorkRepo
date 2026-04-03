namespace Api.Exceptions;

public sealed class BudgetExceededException : BusinessException
{
    public decimal Budget { get; }

    public decimal Required { get; }

    public BudgetExceededException(
        string projectName,
        decimal budget,
        decimal required)
        : base($"Assigning this employee would exceed the budget of project '{projectName}'. Budget: {budget:F2}, required: {required:F2}.")
    {
        this.Budget = budget;
        this.Required = required;
    }
}
