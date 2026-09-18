# AIAgentWebAPI

A controller-based ASP.NET Core Web API targeting .NET 10.

## Run

Install the .NET 10 SDK, then run from this directory:

```shell
dotnet build
dotnet dev-certs https --trust
dotnet run --launch-profile https
```

Use the address printed in the terminal. The supplied HTTPS profile uses `https://localhost:7008`.

- Sample request: `GET /weatherforecast` (also available in the `.http` file).
- OpenAPI document in Development: `/openapi/v1.json`.
- Scalar UI in Development: `/scalar`, if Scalar was enabled when generating this project.
- Serilog console and rolling JSON file logging in `Logs/`, if Serilog was enabled.

## Source layout

- `Program.cs`: service registration and the HTTP request pipeline.
- `Controllers/WeatherForecastController.cs`: sample controller.
- `WeatherForecast.cs`: sample response model.
- `appsettings.json`: application and logging configuration.
- `Properties/launchSettings.json`: local development profiles and ports.

Replace the sample endpoint with your application code. Keep credentials out of committed configuration. Review error responses and add any required authentication before deployment.
