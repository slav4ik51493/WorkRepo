using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;

using Api.Data;
using Api.Exceptions;
using Api.Repositories;
using Api.Repositories.Abstractions;
using Api.Services;
using Api.Services.Abstractions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("AppDb"));

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<ISalaryPaymentRepository, SalaryPaymentRepository>();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<ISalaryPaymentService, SalaryPaymentService>();

builder.Services.AddControllers();

var app = builder.Build();

app.UseExceptionHandler(exceptionApp =>
    exceptionApp.Run(async httpContext =>
    {
        var feature = httpContext.Features.Get<IExceptionHandlerFeature>();

        if (feature?.Error is BusinessException businessException)
        {
            httpContext.Response.StatusCode = StatusCodes.Status422UnprocessableEntity;
            httpContext.Response.ContentType = "application/json";

            await httpContext.Response.WriteAsJsonAsync(new { error = businessException.Message });

            return;
        }

        if (feature?.Error is KeyNotFoundException notFoundException)
        {
            httpContext.Response.StatusCode = StatusCodes.Status404NotFound;
            httpContext.Response.ContentType = "application/json";

            await httpContext.Response.WriteAsJsonAsync(new { error = notFoundException.Message });

            return;
        }

        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
    }));

app.MapControllers();

app.Run();
