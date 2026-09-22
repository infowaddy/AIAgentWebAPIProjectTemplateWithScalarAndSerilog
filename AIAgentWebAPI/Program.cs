//#if (SetupOpenAIAgents)
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Hosting;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Extensions.AI;
using OpenAI.Chat;
using OpenAI.Responses;
using System.ClientModel;
using System.ComponentModel;
//#endif
//#if (SetupDevUI)
using Microsoft.Agents.AI.DevUI;
//#endif
//#if (EnableScalar)
using Scalar.AspNetCore;
//#endif
//#if (EnableSerilog)
using Serilog;
//#endif
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
//#if (EnableSerilog)
// Capture failures that occur before the configured application logger is ready.
Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateBootstrapLogger();
try
{
//#endif
    //#if (EnableSerilog)
    Log.Information("Starting application");
    //#endif
    var builder = WebApplication.CreateBuilder(args);
    //#if (EnableSerilog)
    Log.Information("Web Application builder created");
    builder.Host.UseSerilog((hostingContext, services, loggerConfiguration) =>
        loggerConfiguration
            .ReadFrom.Configuration(hostingContext.Configuration)
            .ReadFrom.Services(services));
    //#endif

    //#if(SetupOpenAIAgents)
    // You will need to set the API key to your own value
    // You can do this using Visual Studio's "Manage User Secrets" UI, or on the command line:
    //   cd this-project-directory
    //   dotnet user-secrets init
    //   dotnet user-secrets set "OPENAI_KEY" "your-openai-api-key-here"
    //   dotnet user-secrets set "OPENAI_ENDPOINT" "your-openai-endpoint-here"
    //   dotnet user-secrets set "OPENAI_MODEL" "your-openai-model-here"
    var inferenceEndpoint = builder.Configuration["OPENAI_ENDPOINT"] ?? throw new InvalidOperationException("Missing configuration: OPENAI_ENDPOINT");
    var inferenceModel = builder.Configuration["OPENAI_MODEL"] ?? throw new InvalidOperationException("Missing configuration: OPENAI_MODEL");
    var inferenceCredential = new ApiKeyCredential(builder.Configuration["OPENAI_KEY"] ?? throw new InvalidOperationException("Missing configuration: OPENAI_KEY"));
    // Create a ResponsesClient for the OpenAI model
    #pragma warning disable OPENAI001
    var responsesClient = new ResponsesClient(
        inferenceCredential, 
        new ResponsesClientOptions
        {
            Endpoint = new Uri(inferenceEndpoint)
        });
    //#if (EnableSerilog)
    Log.Information("ResponsesClient created for endpoint {Endpoint}", inferenceEndpoint);
    //#endif
    builder.Services.AddKeyedSingleton<ResponsesClient>("responsesClient", responsesClient);
    //#if (EnableSerilog)
    Log.Information("ResponsesClient registered with dependency injection container.");
    //#endif

    builder.AddAIAgent(
    "writer",
    (sp, key) => sp.GetRequiredKeyedService<ResponsesClient>("responsesClient").AsAIAgent(
        model: inferenceModel,
        name: key,
        instructions:
            "You write short stories (300 words or less) about the specified topic."));
    //#if (EnableSerilog)
    Log.Information("Writer AIAgent registered with dependency injection container.");
    //#endif

    builder.AddAIAgent(
        "editor",
        (sp, key) => sp.GetRequiredKeyedService<ResponsesClient>("responsesClient").AsAIAgent(
            model: inferenceModel,
            name: key,
            instructions:
                """
            You edit short stories to improve grammar and style,
            ensuring the stories are less than 300 words.

            Once finished editing, select a title and format
            the story for publishing.
            """,
            tools:
            [
                AIFunctionFactory.Create(FormatStory)
            ]));
    //#if (EnableSerilog)
    Log.Information("Editor AIAgent registered with dependency injection container.");
    //#endif
    #pragma warning restore OPENAI001
    builder.AddWorkflow("publisher", (sp, key) => AgentWorkflowBuilder.BuildSequential(
        workflowName: key,
        agents:
        [
            sp.GetRequiredKeyedService<AIAgent>("writer"),
            sp.GetRequiredKeyedService<AIAgent>("editor")
        ]
    )).AddAsAIAgent("publisher-agent");
    //#if (EnableSerilog)
    Log.Information("Publisher workflow registered with dependency injection container.");
    //#endif
    // Register services for OpenAI responses and conversations (also required for DevUI)
    builder.Services.AddOpenAIResponses();
    builder.Services.AddOpenAIConversations();
    //#endif
    //#if (SetupDevUI)
    builder.Services.AddDevUI();
    //#endif
    builder.Services.AddControllers();
    builder.Services.AddOpenApi();

    var app = builder.Build();
    //#if(SetupOpenAIAgents)
    // Map endpoints for OpenAI responses and conversations (also required for DevUI)
    app.MapOpenAIResponses();
    app.MapOpenAIConversations();
    //#endif

    //#if (EnableSerilog)
    Log.Information("Serilog system is configured.");

    // Wrap error handling so request logs include the final response status.
    app.UseSerilogRequestLogging();
    //#endif
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
        //#if (EnableScalar)
        app.MapScalarApiReference(options =>
        {
            options.WithTitle("API Reference")
                .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
        });
        //#endif
        //#if (SetupDevUI)
        app.MapDevUI();
        //#endif
    }

    app.UseHttpsRedirection();
    app.UseAuthorization();
    app.MapControllers();

    app.Run();

//#if (EnableSerilog)
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
//#endif

//#if (SetupOpenAIAgents)
[Description("Formats the story for publication, revealing its title.")]
string FormatStory(string title, string story) => $"""
    **Title**: {title}

    {story}
    """;
//#endif