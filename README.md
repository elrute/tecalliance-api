# Todo Portal API

This solution was created with **Visual Studio 2026** and targets **.NET 10**. It provides a lightweight clone of the JSONPlaceholder `/todos` and `/users` API surface, exposing the same routes, query parameters, and payloads while persisting data in-memory.

## Solution layout

| Project | Type | Purpose |
|---------|------|---------|
| `TodoPortal.Domain` | Class library | Core entities (`Todo`, `User`, etc.), value objects (`Email`), and ports (`ITodoRepository`, `IUserRepository`). |
| `TodoPortal.Application` | Class library | Use cases/handlers for todos and users, DTOs, domain-to-DTO mapping, custom exceptions. |
| `TodoPortal.Infrastructure` | Class library | In-memory data store seeded from canonical JSONPlaceholder JSON files. Implements the domain ports. |
| `TodoPortal.Api` | ASP.NET Core Web API | Thin controllers that expose JSONPlaceholder-compatible endpoints, exception middleware, CORS, and DI wiring. |
| `TodoPortal.Domain.Tests` | xUnit | Tests for domain/value-object behavior (e.g., `Email`). |
| `TodoPortal.Application.Tests` | xUnit | Handler-level tests with mocked repositories (e.g., `CreateTodoHandler`). |
| `TodoPortal.Infrastructure.Tests` | xUnit | Verifies the JSON data store (seed loading, CRUD semantics). |
| `TodoPortal.Api.Tests` | xUnit + WebApplicationFactory | End-to-end tests that call the API’s `/todos` endpoints. |

## Storage model

The infrastructure project loads the official JSONPlaceholder `users` and `todos` payloads into `TodoPortal.Infrastructure/Data/*.json` and deserializes them into an in-memory `JsonPlaceholderDataStore`. All repository operations read/write against this store:

- Users: read-only mirrors of JSONPlaceholder data.
- Todos: querying plus full CRUD support. New todo IDs are issued sequentially, and updates/deletes mutate the in-memory list only.

Because the store lives in memory, restarting the API resets it back to the seed dataset.

## Running the API

1. Ensure the **.NET 10 SDK** (10.0.201 or later) and **Visual Studio 2026** (or any IDE that supports .NET 10) are installed.
2. Open `todo-portal-api.slnx` in Visual Studio.
3. Select `TodoPortal.Api` as the startup project (already configured in the solution).
4. Press **F5**. The API listens on `https://localhost:5001` (configurable via `appsettings.json`) and exposes the same routes as JSONPlaceholder:
   - `GET /todos`, `GET /todos/{id}`, `POST /todos`, `PUT /todos/{id}`, `PATCH /todos/{id}`, `DELETE /todos/{id}`
   - `GET /users`, `GET /users/{id}`, `GET /users/{id}/todos`

HTTPS dev certificates and CORS origins can be adjusted in `TodoPortal.Api/appsettings.json`.

## Testing

Test projects are already part of the solution. To execute them:

```bash
dotnet test
```

or run them individually from Visual Studio’s Test Explorer. The suite includes:

- Domain tests for value objects (validation, equality, invariants).
- Application tests for handler success/error paths with mocked ports.
- Infrastructure tests ensuring the JSON-backed store loads the real payloads and supports CRUD.
- API integration tests that spin up the ASP.NET Core host via `WebApplicationFactory` and exercise endpoints end-to-end.

## Notes

- The project deliberately mirrors JSONPlaceholder responses for easy front-end substitution.
- Storage is local-only; swap `InMemoryTodoRepository`/`InMemoryUserRepository` for other adapters if persistent storage is needed.
- Configuration (ports, CORS origins) lives in `appsettings.json` so different environments can override without code changes.
