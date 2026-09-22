# AI Agent Web API with Scalar and Serilog

A free, open-source .NET 10 Web API project template built with Microsoft Agent Framework. Create a controller-based API with optional Scalar API documentation, Serilog logging, sample AI agents, a sequential workflow, and Agent Framework DevUI.

This is a community project maintained by Zin Min, not an official Microsoft template. Some Agent Framework dependencies are preview or alpha releases. The template is free; your chosen AI service may charge for model usage.

- **NuGet package ID:** `AIAgentWebAPIWithScalarAndSerilog.Templates`
- **Template command:** `aiagent-webapi-scalarandserilog`
- **Source and issues:** [GitHub repository](https://github.com/infowaddy/AIAgentWebAPIWithScalarAndSerilog)

## Features

- ASP.NET Core controllers and a sample weather forecast endpoint.
- OpenAPI document and optional Scalar API reference UI.
- Optional Serilog console and rolling JSON file logging.
- Writer and editor agents, including a formatting function tool.
- A sequential publisher workflow exposed as `publisher-agent`.
- Optional DevUI for interacting with agents and workflows.
- Generic Problem Details error responses with a trace ID.

## Prerequisites

- .NET 10 SDK.
- When AI support is enabled: a service supporting the Responses API, its API key, its base endpoint URL, and an appropriate model or deployment name.

There is no default model. Configure the model you intend to use. Provider compatibility depends on support for the API and features used by the sample.

## Install and create a project

Once the package is available on NuGet.org:

```shell
dotnet new install AIAgentWebAPIWithScalarAndSerilog.Templates
dotnet new aiagent-webapi-scalarandserilog -n MyAgentApi
cd MyAgentApi
```

This is a `dotnet new` template package. Install it with `dotnet new install`, rather than adding it as an application package reference.

## Configure and run

With AI support enabled, run these commands from the generated project directory, replacing the placeholder values:

```shell
dotnet user-secrets init
dotnet user-secrets set "OPENAI_KEY" "your-api-key"
dotnet user-secrets set "OPENAI_ENDPOINT" "your-service-base-url"
dotnet user-secrets set "OPENAI_MODEL" "your-model-or-deployment-name"
dotnet run --launch-profile https
```

`OPENAI_ENDPOINT` configures the client base URL; use the value expected by your provider's Responses API client. User secrets are loaded in Development. Alternatively, supply the same three keys as environment variables. Keep credentials out of tracked configuration files.

If AI support was disabled when creating the project, skip the three AI settings and run the application directly.

## Open Scalar and DevUI

The supplied HTTPS launch profile listens on `https://localhost:7008` and `http://localhost:5290`. If you change the ports, use the addresses printed at startup.

| Page or endpoint | HTTPS URL | Availability |
| --- | --- | --- |
| Scalar API reference | `https://localhost:7008/scalar` | Development, when Scalar is enabled |
| Agent Framework DevUI | `https://localhost:7008/devui/` | Development, when AI support and DevUI are enabled |
| OpenAPI document | `https://localhost:7008/openapi/v1.json` | Development |
| Weather forecast sample | `https://localhost:7008/weatherforecast` | All environments |

The IDE launch configuration opens **Scalar** when enabled. To use **DevUI**, open its URL manually after the application starts. Command-line `dotnet run` may require opening both pages manually.

Scalar is the API reference UI. DevUI is the interface for trying the agents and workflow: select an available agent, enter a story topic, and inspect the response. The sample also maps OpenAI-compatible Responses and Conversations endpoints for clients that support those APIs.

## Template options

All four feature switches default to `true`.

| Option | Short form | Purpose |
| --- | --- | --- |
| `--serilog` | `-s` | Include Serilog logging |
| `--scalar` | `-c` | Include Scalar API reference UI |
| `--openai` | `-oa` | Include the AI client, agents, workflow, and API endpoints |
| `--devui` | `-du` | Include Agent Framework DevUI; requires AI support |

```shell
# Create an AI API without DevUI.
dotnet new aiagent-webapi-scalarandserilog -n MyAgentApi --devui false

# Create a Web API without the AI sample; DevUI is unavailable.
dotnet new aiagent-webapi-scalarandserilog -n MyWebApi --openai false

# Use built-in logging and omit Scalar.
dotnet new aiagent-webapi-scalarandserilog -n MinimalApi --serilog false --scalar false

# Inspect all supported parameters.
dotnet new aiagent-webapi-scalarandserilog --help
```

Only `net10.0` is supported. Options control project generation; changing them later requires editing the generated application.

## Sample workflow

The `writer` agent drafts a short story. The `editor` agent improves the text and can call `FormatStory` to format the title and story. The `publisher` workflow runs these agents sequentially and is exposed as `publisher-agent`.

The prompts request stories of at most 300 words; this is not a programmatic length check. The workflow produces formatted text and does not publish to an external service.

## Troubleshooting

- **Missing configuration:** Set all three `OPENAI_*` values in the generated project. Run `dotnet user-secrets init` before setting user secrets, and use a Development launch profile.
- **Scalar or DevUI returns 404:** Check the environment and the corresponding template options. DevUI also requires AI support.
- **HTTPS certificate error:** Trust the ASP.NET Core development certificate using `dotnet dev-certs https --trust` where supported.
- **Model requests fail:** Check the configured base URL, credentials, model access, and provider support for the Responses API.
- **Port already in use:** Adjust `Properties/launchSettings.json` and use the new URL.

## Build and test the template locally

From the repository root:

```shell
dotnet pack AIAgentWebAPIWithScalarAndSerilog.csproj --configuration Release --output artifacts
dotnet new install ./artifacts/AIAgentWebAPIWithScalarAndSerilog.Templates.1.0.0.nupkg
dotnet new aiagent-webapi-scalarandserilog -n TemplateSmokeTest -o artifacts/TemplateSmokeTest
dotnet build artifacts/TemplateSmokeTest
```

The filename above corresponds to the current package version `1.0` (normalized to `1.0.0`). Use the filename printed by `dotnet pack` if the version changes. Configure AI settings before running the generated sample.

To uninstall:

```shell
dotnet new uninstall AIAgentWebAPIWithScalarAndSerilog.Templates
```

## Contributing and license

Report issues and propose improvements through [GitHub issues](https://github.com/infowaddy/AIAgentWebAPIWithScalarAndSerilog/issues). When changing template behavior, verify generated projects with the affected feature switches enabled and disabled.

Released under the [MIT License](https://github.com/infowaddy/AIAgentWebAPIWithScalarAndSerilog/blob/main/LICENSE.txt). Copyright (c) 2026 ZIN MIN.
