# uml-ai-gen

UML-driven CRUD generator reference app (JHipster-style): ASP.NET Core API, Avalonia desktop client, and integration tests.

## Projects

| Project | Description |
|---------|-------------|
| `TodoAppApi` | REST API (SQLite, EF Core) |
| `TodoApp` | Avalonia desktop client (Prism, NSwag) |
| `TodoAppApiTest` | NUnit integration tests |

## UML model

Entity and action definitions live in [`todo-list.puml`](todo-list.puml).

## Requirements

- .NET 8 (desktop client)
- .NET 9 (API and tests)

## Build and test

```bash
dotnet restore todoapp.sln
dotnet build todoapp.sln
dotnet test TodoAppApiTest
```

## Run

```bash
dotnet run --project TodoAppApi
dotnet run --project TodoApp
```
