using Api.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("AppDb"));

builder.Services.AddControllers();

var app = builder.Build();

app.UseExceptionHandler(exceptionApp =>
    exceptionApp.Run(async httpContext =>
    {
        var feature = httpContext.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>();

        if (feature?.Error is KeyNotFoundException ex)
        {
            httpContext.Response.StatusCode = StatusCodes.Status404NotFound;
            httpContext.Response.ContentType = "application/json";

            await httpContext.Response.WriteAsJsonAsync(new { error = ex.Message });

            return;
        }

        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
    }));

app.MapControllers();

app.Run();
