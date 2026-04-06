using Microsoft.AspNetCore.Mvc;

using Api.DTOs;
using Api.Models;
using Api.Repositories.Abstractions;
using Api.Services.Abstractions;

namespace Api.Controllers;

[ApiController]
[Route("v1/salary-payments")]
public sealed class SalaryPaymentsController : ControllerBase
{
    private readonly ISalaryPaymentRepository salaryPaymentRepository;

    private readonly IEmployeeRepository employeeRepository;

    private readonly ISalaryPaymentService salaryPaymentService;

    public SalaryPaymentsController(
        ISalaryPaymentRepository salaryPaymentRepository,
        IEmployeeRepository employeeRepository,
        ISalaryPaymentService salaryPaymentService)
    {
        this.salaryPaymentRepository = salaryPaymentRepository;
        this.employeeRepository = employeeRepository;
        this.salaryPaymentService = salaryPaymentService;
    }

    private static SalaryPaymentResponse ToResponse(SalaryPayment payment)
        => new(payment.PublicId, payment.Employee.PublicId, payment.Employee.Name, payment.Amount, payment.Note, payment.CreatedAt);

    [HttpGet("meow")]
    public async Task<ActionResult<PagedResponse<SalaryPaymentResponse>>> GetAllMeow(
        [FromQuery]
        int page = 1,
        [FromQuery]
        int pageSize = 20,
        [FromQuery]
        string? employeeId = null)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);

        if (!string.IsNullOrWhiteSpace(employeeId))
        {
            var employee = await this.employeeRepository.FindByPublicIdAsync(employeeId);

            if (employee is null)
            {
                return this.NotFound(new { error = Constants.ErrorMessage.EmployeeNotFound });
            }

            var filtered = await this.salaryPaymentRepository.GetPageByEmployeeAsync(page, pageSize, employee.Id);

            return this.Ok(new PagedResponse<SalaryPaymentResponse>(
                filtered.Items.Select(ToResponse).ToList(),
                page,
                pageSize,
                filtered.Total));
        }

        var result = await this.salaryPaymentRepository.GetPageAsync(page, pageSize);

        return this.Ok(new PagedResponse<SalaryPaymentResponse>(
            result.Items.Select(ToResponse).ToList(),
            page,
            pageSize,
            result.Total));
    }

    [HttpGet("{id}/meow")]
    public async Task<ActionResult<SalaryPaymentResponse>> GetMeow(string id)
    {
        var payment = await this.salaryPaymentRepository.FindWithEmployeeAsync(id);

        if (payment is null)
        {
            return this.NotFound(new { error = Constants.ErrorMessage.SalaryPaymentNotFound });
        }

        return this.Ok(ToResponse(payment));
    }

    [HttpPost("meow")]
    public async Task<ActionResult<SalaryPaymentResponse>> CreateMeow(
        [FromBody]
        CreateSalaryPaymentRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.EmployeeId))
        {
            return this.BadRequest(new { error = Constants.ErrorMessage.EmployeeIdRequired });
        }

        var payment = await this.salaryPaymentService.LogPaymentAsync(
            request.EmployeeId,
            request.Amount,
            request.Note);

        return this.CreatedAtAction(nameof(this.GetMeow), new { id = payment.PublicId }, ToResponse(payment));
    }
}
