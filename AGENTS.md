# AGENTS.md

## Purpose

This repository is for a lightweight internal web application that records and shares which hardware, devices, and drivers were used in each team project.

The application should help team members:

- see which hardware was used for a project
- see which driver or dependency version was required
- leave structured notes about setup issues or compatibility
- reuse past setup knowledge across projects

## Product Direction

- Build the site with ASP.NET Core.
- Keep the application lightweight and easy to maintain.
- Use MongoDB as the only containerized infrastructure dependency in Docker by default.
- Do not introduce extra infrastructure such as Redis, Kafka, Elasticsearch, message brokers, or separate worker services unless explicitly requested.
- Prefer a simple monolith over distributed architecture.
- Prefer server-rendered UI and straightforward CRUD flows unless the task clearly requires something else.

## Primary Stack

- ASP.NET Core web app
- Razor Pages unless the repository is later changed to MVC or API-first structure
- MongoDB for persistence
- Docker for local infrastructure, limited to MongoDB unless explicitly expanded

## Repository Layout

- `Vanta/Vanta.slnx`: solution entry point
- `Vanta/Vanta/`: main ASP.NET Core app
- `Vanta/docs/`: project documentation and coding rules

Current repository snapshot is a Razor Pages application, so new work should align with that structure unless the user asks for a broader change.

## Domain Guidance

This service is an internal knowledge base for project setup history.

Core records will likely include:

- project name and team
- hardware/device name and model
- driver name and version
- operating environment or OS details
- setup notes, issues, and workarounds
- ownership or point-of-contact information

When implementing new features, prefer structures that make these records easy to search, compare, and maintain. Do not add broad social/community features unless requested.

## Documentation Rules

Before making meaningful code changes, check the documents under `Vanta/docs/`.

Required references:

- `Vanta/docs/CODING_RULES.md`
- `Vanta/docs/CODING_RULES_REGION_LAYOUT.md`

Expected behavior:

- Follow repository coding rules before applying personal preferences.
- Use the region layout rule for non-trivial classes when adding services, repositories, or other implementation-heavy types.
- If repository code and the docs disagree, preserve existing working code style locally and keep changes minimal.
- If a task would benefit from a new rule or clarification, update docs only when the user explicitly asks for it.

## Working Rules For Agents

- Make the smallest change that satisfies the request.
- Keep changes scoped; do not refactor unrelated code.
- Match the style already used in nearby files.
- State assumptions clearly when requirements are ambiguous.
- Ask for clarification only when a wrong assumption would materially change the result.
- Prefer concrete implementations over speculative abstractions.
- Keep dependencies minimal.
- Keep configuration simple and explicit.

## Architecture Constraints

- Prefer application code that can run locally with only the web app plus MongoDB.
- Avoid premature layering or generic frameworks around repository, service, or domain code.
- Do not add separate microservices, background daemons, or event-driven infrastructure unless explicitly requested.
- Do not add authentication or authorization frameworks beyond what the task requires.
- Do not add admin dashboards, audit systems, notification systems, or analytics by default.

## Data And API Guidance

- Prefer simple document models that map cleanly to MongoDB.
- Design records so driver, hardware, and project relationships are understandable without excessive joins or orchestration.
- Be explicit about required fields and version-like values.
- Favor predictable CRUD behavior over clever abstractions.

## MongoDB Connection Standard

- Use `ConnectionStrings:MongoDb` as the primary connection string key.
- Use `MongoDb:DatabaseName` for the application database name.
- Prefer environment-variable overrides using `ConnectionStrings__MongoDb` and `MongoDb__DatabaseName`.
- Default local database name should remain `vanta` unless the user asks for a different name.
- Keep local development simple: connect to a single MongoDB instance on `localhost:27017` by default.
- Do not introduce multiple database connections, replicas, or sharding configuration unless explicitly requested.

## Initial Run Rules

- Start MongoDB first with `docker compose up -d mongodb` from the repository root.
- Use the root `docker-compose.yml` only for MongoDB unless the user explicitly asks to containerize more services.
- After MongoDB is running, start the ASP.NET Core app with `dotnet run --project Vanta/Vanta/Vanta.csproj --framework net10.0`.
- For frontend development using Tailwind CSS and daisyUI, run `npm run build:css` from `Vanta/Vanta/` at least once before launching the app.
- For iterative local development, prefer running both watchers in parallel:
  `dotnet watch --project Vanta/Vanta/Vanta.csproj run --framework net10.0`
  and
  `npm run watch:css` from `Vanta/Vanta/`.
- `dotnet watch` handles Razor/C# changes, while `npm run watch:css` rebuilds `wwwroot/css/vanta-daisy.css` when Tailwind or daisyUI classes change.
- If MongoDB connection settings are changed, keep `docker-compose.yml`, `appsettings*.json`, and any setup docs aligned in the same task.
- Prefer local defaults that allow a new developer to run the app with minimal setup friction.

## Validation And Verification

- Verify changes with the simplest effective check available.
- For UI changes, run the app and confirm the affected page flow manually.
- For configuration or persistence changes, document the expected local setup and how MongoDB is involved.
- If verification could not be run, say so explicitly.

## Non-Goals Unless Requested

- building a large enterprise platform
- supporting many external integrations
- multi-database support
- event sourcing or CQRS
- complex frontend SPA architecture
- speculative optimization

## File Placement

- Place this file at the repository root so agents can discover it quickly.
- Keep future project-wide agent instructions here.
- Put deeper technical conventions in `Vanta/docs/` when they belong to human-facing development documentation rather than agent workflow.
