using Microsoft.AspNetCore.Mvc;
using Scalar.AspNetCore;
using Serilog;
using System.Diagnostics;

// Capture failures that occur before the configured application logger is ready.
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting application");

    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((hostingContext, services, loggerConfiguration) =>
        loggerConfiguration
            .ReadFrom.Configuration(hostingContext.Configuration)
            .ReadFrom.Services(services));

    builder.Services.AddControllers();
    builder.Services.AddOpenApi();

    var app = builder.Build();

    Log.Information("Serilog system is configured.");

    // Wrap error handling so request logs include the final response status.
    app.UseSerilogRequestLogging();

    // Keep error responses safe and consistent with either logging provider.
    // Exception-handler middleware logs the full exception on the server.
    app.UseExceptionHandler(errorApp =>
    {
        errorApp.Run(async context =>
        {
            var problem = new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "Unexpected Error",
                Detail = "An unexpected error occurred. Please try again later.",
                Instance = context.Request.Path
            };
            problem.Extensions["traceId"] = Activity.Current?.Id ?? context.TraceIdentifier;

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsJsonAsync(
                problem,
                options: (System.Text.Json.JsonSerializerOptions?)null,
                contentType: "application/problem+json",
                cancellationToken: context.RequestAborted);
        });
    });

    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
        app.MapScalarApiReference(options =>
        {
            options.WithTitle("API Reference")
                .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
        });
    }

    app.UseHttpsRedirection();
    app.UseAuthorization();
    app.MapControllers();

    app.Run();

}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
    throw;
}
finally
{
    Log.CloseAndFlush();
}

