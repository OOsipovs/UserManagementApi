# User Management API

A **.NET 9 Web API** for managing users and their profile information, built with Clean Architecture, Entity Framework Core, and PostgreSQL.

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

This project follows **Clean Architecture** (also known as Onion Architecture), which organises code into concentric layers where dependencies always point **inward** — outer layers depend on inner layers, never the reverse.

### Why Clean Architecture?

| Benefit | How it applies here |
|---|---|
| **Separation of concerns** | Business logic in `Application` is completely isolated from HTTP and database details |
| **Testability** | `UserService` and controllers are unit-tested by mocking interfaces — no real database needed |
| **Replaceability** | The `IMessagePublisher` stub can be swapped for a real RabbitMQ implementation without touching any business logic |
| **Maintainability** | Each layer has a single responsibility, making future changes predictable and low-risk |

---

## Technologies

| Technology | Purpose |
|---|---|
| **.NET 9** | Target framework |
| **ASP.NET Core Web API** | HTTP API host |
| **Entity Framework Core 9** | ORM and database access |
| **Npgsql** | PostgreSQL EF Core provider |
| **PostgreSQL 17** | Relational database (via Docker) |
| **Swashbuckle / Swagger UI** | Interactive API documentation |
| **xUnit** | Unit test framework |
| **Moq** | Mocking library for unit tests |
| **FluentAssertions** | Readable assertion syntax in tests |
| **Docker Compose** | Local PostgreSQL setup |

---

## Project Structure

```plaintext
UserManagementApi.sln                  # Solution file
/.github/
  └── workflows/                       # GitHub Actions workflows
/src
  ├── UserManagementApi/               # Main web API project
  |    ├── Controllers/                # API controllers
  |    ├── DTOs/                       # Data transfer objects
  |    ├── Middleware/                 # Custom middleware
  |    ├── Properties/                 # Project properties
  |    └── Program.cs                  # App entry point
  |___	
  ├── UserManagementApi.Tests/         # Unit test project
  |    ├── Controllers/                # Tests for API controllers
  |    ├── Services/                   # Tests for application services
  |    └── UserManagementApi.Tests.csproj # Test project file
  └── README.md                        # Project documentation

```

---

## Getting Started

To run this project locally:

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)

### 1. Clone the repository

```bash
git clone https://github.com/your-username/UserManagementApi.git
cd UserManagementApi
```

### 2. Start the PostgreSQL database

```bash
docker-compose up -d
```

This starts a PostgreSQL 17 instance with:
- **Host:** `localhost:5432`
- **Database:** `homeworkdb`
- **Username:** `taskuser`
- **Password:** `mypassword`

### 3. Run the API

```bash
dotnet run --project src/UserManagementApi/UserManagementApi.csproj
```

The API will be available at `https://localhost:5001` (or `http://localhost:5000` for HTTP).

The application will automatically apply EF Core migrations on startup — no manual migration step needed.

> Alternatively, open the solution in **Visual Studio 2022** and press **F5**.

### 4. Access Swagger UI

Swagger UI opens automatically in the browser at:

- **HTTP:** http://localhost:5257
- **HTTPS:** https://localhost:7255

Use it to explore and test all endpoints interactively without Postman.

---

## API Endpoints

| Method | Endpoint | Description |
|---|---|---|
| `GET` | `/api/users/{id}` | Get a user with their profile by GUID |
| `POST` | `/api/users` | Create a new user and profile |

---

## Example Requests

### Create a user — `POST /api/users`

**Request:**

```http
POST /api/users
Content-Type: application/json

{
  "username": "jdoe",
  "password": "Password123",
  "email": "jdoe@example.com"
}
```

### Get all users

```http
GET /api/users
Authorization: Bearer your_jwt_token
```

# Response
```http
HTTP/1.1 201 Created
Content-Type: application/json

{
  "id": "e7b1aef0-e123-4dca-9c6e-f061ffd9e929",
  "username": "jdoe",
  "email": "jdoe@example.com",
  "profile": {
    "firstName": "John",
    "lastName": "Doe",  
    "bio": "Software developer",
    "photoUrl": null
  }
}
```

### Get a user by ID

```http
GET /api/users/e7b1aef0-e123-4dca-9c6e-f061ffd9e929
Authorization: Bearer your_jwt_token
```

# Response
```http
HTTP/1.1 200 OK
Content-Type: application/json

{
  "id": "e7b1aef0-e123-4dca-9c6e-f061ffd9e929",
  "username": "jdoe",   
  "email": "jdoe@example.com",
  "profile": {
    "firstName": "John",
    "lastName": "Doe",
    "bio": "Software developer",
    "photoUrl": null
  }
}
```

**Response — 404 Not Found** (ID does not exist)

---

## Running Tests

Unit tests use mocks — **no database or Docker required** to run them.

| Test class | What is tested |
|---|---|
| `UserServiceTests` | Business logic — get by ID, create user, publish order and call counts |
| `UserControllerTests` | HTTP responses — 200, 201, 404 status codes and response bodies |
| `CreateUserRequestValidationTests` | DTO validation — required fields, max lengths, email format |

To run tests in Visual Studio, open **Test Explorer** (__Test > Test Explorer__) and click **Run All**.
Alternatively, run tests from the command line:
```bash