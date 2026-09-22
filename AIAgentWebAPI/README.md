# AI Agent Web API

This application was generated from the `aiagent-webapi-scalarandserilog` community template. It targets .NET 10 and can include Microsoft Agent Framework agents and workflows, Scalar API documentation, Serilog logging, and Agent Framework DevUI.

Features depend on the options selected when the project was created. AI configuration is required only when AI support was included. Some Agent Framework dependencies are preview or alpha releases.

## Configure the AI service

Run these commands from the directory containing this application's `.csproj`, replacing the placeholders:

```shell
dotnet user-secrets init
dotnet user-secrets set "OPENAI_KEY" "your-api-key"
dotnet user-secrets set "OPENAI_ENDPOINT" "your-service-base-url"
dotnet user-secrets set "OPENAI_MODEL" "your-model-or-deployment-name"
```

All three settings are required when AI support is enabled. There is no default model. `OPENAI_ENDPOINT` sets the client base URL; use the URL expected by your provider's Responses API client. The service must support the Responses API and the features used by the agents.

User secrets are loaded in Development. As an alternative, set environment variables in the shell that starts the application:

**PowerShell**

```powershell
$env:OPENAI_KEY = "your-api-key"
$env:OPENAI_ENDPOINT = "your-service-base-url"
$env:OPENAI_MODEL = "your-model-or-deployment-name"
```

**Bash**

```bash
export OPENAI_KEY="your-api-key"
export OPENAI_ENDPOINT="your-service-base-url"
export OPENAI_MODEL="your-model-or-deployment-name"
```

Keep API credentials out of source control. The template is free; AI service usage may incur charges.

## Run the application

```shell
dotnet run --launch-profile https
```

The provided HTTPS profile runs in Development and listens on:

- HTTPS: `https://localhost:7008`
- HTTP: `http://localhost:5290`

Use the addresses printed at startup if you customize `Properties/launchSettings.json`.

## Scalar and DevUI

| Interface | URL | Purpose |
| --- | --- | --- |
| Scalar | `https://localhost:7008/scalar` | Browse the API reference and make API requests |
| Agent Framework DevUI | `https://localhost:7008/devui/` | Interact with registered agents and workflows |
| OpenAPI document | `https://localhost:7008/openapi/v1.json` | Retrieve the API description |
| Weather forecast | `https://localhost:7008/weatherforecast` | Try the sample controller |

**The IDE launch profile opens Scalar, not DevUI.** Open `https://localhost:7008/devui/` manually to use DevUI. When starting with `dotnet run`, open the desired URL manually if no browser opens.

Scalar and DevUI are mapped only in Development and only when their features were included. DevUI also requires AI support. If Scalar was disabled, the provided IDE profile does not automatically launch a browser, even when DevUI is enabled.

In DevUI, select an available agent, enter a story topic, and inspect its response:

- `writer`: drafts a short story.
- `editor`: edits the story and can use the `FormatStory` function tool.
- `publisher-agent`: runs the sequential `publisher` workflow through writer and editor.

The prompts request a maximum of 300 words; the application does not enforce a word count. The publisher workflow returns formatted text and does not publish to an external service.

When AI support is included, the application also exposes OpenAI-compatible Responses and Conversations endpoints for clients supporting those APIs.

## Project structure

- `Program.cs`: application startup, optional AI registrations, middleware, and endpoint mappings.
- `Controllers/WeatherForecastController.cs`: sample controller.
- `appsettings.json`: application settings and optional Serilog configuration.
- `Properties/launchSettings.json`: development launch profiles and URLs.
- `AIAgentWebAPI.http`: sample weather forecast request.

## Troubleshooting

- **Missing configuration:** Initialize user secrets and set all three `OPENAI_*` values in this project. Use the Development profile, or configure environment variables.
- **DevUI returns 404:** Check that Development is active and both AI support and DevUI were included when creating the project.
- **Scalar returns 404:** Check that Development is active and Scalar was included.
- **HTTPS certificate error:** Run `dotnet dev-certs https --trust` where supported.
- **AI request fails:** Check the service base URL, key, model access, and Responses API support.
- **Port already in use:** Change the application URLs in `Properties/launchSettings.json`.

## Template source and license

For installation instructions, generation options, and contributions, see the [template repository](https://github.com/infowaddy/AIAgentWebAPIWithScalarAndSerilog).

Released under the MIT License; see `LICENSE.txt`.
