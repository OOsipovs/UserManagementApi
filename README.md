# User Management API

A **.NET 9 Web API** for managing users and their profile information, built with Clean Architecture, Entity Framework Core, and PostgreSQL.

> **Repository:** https://github.com/OOsipovs/UserManagementApi

---

## Table of Contents

- [Architecture](#architecture)
- [Technologies](#technologies)
- [Project Structure](#project-structure)
- [Getting Started](#getting-started)
- [API Endpoints](#api-endpoints)
- [Example Requests](#example-requests)
- [Running Tests](#running-tests)

---

## Architecture

In this project I've decide to follow **Clean Architecture**, organising the codebase into four concentric layers where dependencies always point **inward** — outer layers depend on inner layers, never the reverse.
It might seem overcomplicated for this particular task , but I wanted to demonstrate how to structure a real-world application with clear separation of concerns and testability in mind.

### Layers

| Layer | Project | Responsibility |
|---|---|---|
| **Domain** | `UserManagementApi.Domain` | Core entities (`User`, `Profile`) and repository interfaces (`IUserRepository`). No dependencies on any other layer. |
| **Application** | `UserManagementApi.Application` | Business logic (`UserService`), service interfaces (`IUserService`), DTOs (`CreateUserRequest`, `UserResponse`). Depends only on Domain. |
| **Infrastructure** | `UserManagementApi.Infrastructure` | EF Core `AppDbContext`, `UserRepository`, `MessagePublisher`. Implements interfaces defined in Domain and Application. |
| **API** | `UserManagementApi.API` | ASP.NET Core controllers, middleware, DI registration, app entry point. Depends on all other layers. |

### Key Design Decisions

| Decision | Reason |
|---|---|
| `IUserRepository` defined in **Domain** | Keeps persistence details out of business logic; EF Core is swappable without changing the domain |
| `IUserService` defined in **Application** | Controllers depend on an abstraction, enabling full unit testing without a real database |
| `IMessagePublisher` stub in **Infrastructure** | Simulates event publishing (e.g. RabbitMQ) without requiring a broker to run locally |
| DTOs in **Application**, not **API** | Decouples the HTTP contract from the transport layer |
| Auto-migration on startup | `db.Database.Migrate()` in `Program.cs` ensures the schema is always up-to-date with no manual steps |

### Request Flow

1. **HTTP Request**: A client sends an HTTP request to the API.
2. **UsersController** (API layer): Receives the request and calls the appropriate service.
3. **UserService** (Application layer): Contains business logic and calls the repository.
4. **UserRepository** (Infrastructure layer): Interacts with the database via EF Core and Mesage bus via IMessagePublisher.

## Technologies

| Technology | Version | Purpose |
|---|---|---|
| **.NET** | 9 | Target framework |
| **ASP.NET Core Web API** | 9 | HTTP API host |
| **Entity Framework Core** | 9.0.14 | ORM and database access |
| **Npgsql EF Core Provider** | 9.0.4 | PostgreSQL driver for EF Core |
| **PostgreSQL** | 17 | Relational database (via Docker) |
| **Swashbuckle / Swagger UI** | 9.0.6 | Interactive API documentation |
| **xUnit** | 2.9.2 | Unit test framework |
| **Moq** | 4.20.72 | Mocking library for unit tests |
| **FluentAssertions** | 8.8.0 | Readable assertion syntax in tests |
| **Docker Compose** | — | Local PostgreSQL setup |


## Getting Started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)

### 1. Clone the repository

```bash
git clone https://github.com/OOsipovs/UserManagementApi.git
cd UserManagementApi
```

### 2. Start the PostgreSQL container

```bash
docker-compose up -d
```

### 3. Run the application

```bash
dotnet run --project UserManagementApi.API/UserManagementApi.API.csproj
```


 **Visual Studio:** Open the solution and press **F5**.
 
 On startup, EF Core migrations are applied automatically — the `Users` and `Profiles` tables are created if they don't exist.
 
 ### 5. Access Swagger UI
 Swagger UI is served at the root URL in Development mode:
 
> | Profile | URL |
> |---|---|
> | HTTP | http://localhost:5257 |
> | HTTPS | https://localhost:7255 |

---

## API Endpoints

| Method | Endpoint | Description | Success | Failure |
|---|---|---|---|---|
| `POST` | `/api/users` | Create a new user and profile | `201 Created` | `400 Bad Request` |
| `GET` | `/api/users/{id}` | Get a user with their profile by GUID | `200 OK` | `404 Not Found` |

### Request Validation

`CreateUserRequest` fields are validated via Data Annotations before reaching the service layer:

| Field | Rules |
|---|---|
| `username` | Required, max 100 characters |
| `email` | Required, valid email format, max 200 characters |
| `firstName` | Required, max 100 characters |
| `lastName` | Required, max 100 characters |
| `dateOfBirth` | Required |

---

## Example Requests

### Create a user — `POST /api/users`

**Request:**

```http
POST /api/users
Content-Type: application/json

{
    "username": "jdoe",
    "email": "jdoe@example.com",
    "firstName": "John",
    "lastName": "Doe",
    "dateOfBirth": "1990-05-15T00:00:00Z"
}
```

**Response**
```http
HTTP/1.1 201 Created
Content-Type: application/json

{
    "id": "e7b1aef0-e123-4dca-9c6e-f061ffd9e929",
    "username": "jdoe",
    "email": "jdoe@example.com",
    "firstName": "John",
    "lastName": "Doe",
    "dateOfBirth": "1990-05-15T00:00:00Z"
}
```


### Get a user by ID — `GET /api/users/:userId`
**Request:**

```http
GET /api/users/e7b1aef0-e123-4dca-9c6e-f061ffd9e929
```

**Response**
```http
HTTP/1.1 200 OK
Content-Type: application/json

{
  "id": "e7b1aef0-e123-4dca-9c6e-f061ffd9e929",
  "username": "jdoe",
  "email": "jdoe@example.com",
  "firstName": "John",
  "lastName": "Doe",
  "dateOfBirth": "1990-05-15T00:00:00Z"
}
```

**Response — 404 Not Found** (ID does not exist)

**Response — `404 Not Found`:** returned when the GUID does not exist in the database.

---

## Running Tests

Unit tests use mocks only — **no database or Docker required**.

> **Visual Studio:** Open __Test > Test Explorer__ and click **Run All**.

### Test Coverage

| Test class | What is tested |
|---|---|
| `UserServiceTests` | `GetUserByIdAsync` — returns correct response, returns `null` when not found, handles missing profile; `CreateUserAsync` — maps all fields, publishes message exactly once, publishes *after* save |
| `UserControllerTests` | `GetById` — returns `200` with body, returns `404`; `Create` — returns `201` with correct location header, calls service exactly once |
| `CreateUserRequestValidationTests` | Required fields, max-length rules on all string fields, email format validation |