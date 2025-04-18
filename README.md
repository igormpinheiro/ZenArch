# ZenArch - Clean Architecture Template

ZenArch is a .NET 9 template implementing Clean Architecture principles for building scalable, maintainable, and testable applications.

## Architecture Overview

The solution follows the Clean Architecture principles and is organized into the following projects:

- **Domain**: Contains enterprise logic and entities
- **Application**: Contains business logic and use cases using CQRS pattern
- **Infrastructure**: Contains implementations of external services
- **Persistence**: Contains database access and ORM configurations
- **Presentation.API**: Contains API controllers and presentation logic
- **SharedKernel**: Contains shared components used across the solution

## Technologies

- .NET 9
- CQRS pattern with MediatR
- ErrorOr for domain-centric error handling
- FluentValidation for request validation
- Entity Framework Core for data access
- xUnit for testing
- Clean Architecture principles
- SOLID principles
- Docker support

## Getting Started

### Prerequisites

- .NET 9 SDK
- Docker (optional, for containerized database)
- IDE of your choice (Visual Studio, VS Code, Rider, etc.)

### Building and Running

1. Clone the repository
2. Navigate to the root directory
3. Run `dotnet restore` to restore dependencies
4. Run `dotnet build` to build the solution
5. To run the API: `dotnet run --project src/Presentation.API`

With Docker:

```shell
docker-compose up -d
```

### Running Tests

```shell
dotnet test
```

## Project Structure

- **src**: Contains the source code for the application
    - **Domain**: Core domain entities and business logic
    - **Application**: Application use cases and CQRS implementations
    - **Infrastructure**: External service implementations
    - **Persistence**: Database-related implementations
    - **Presentation.API**: API endpoints and controllers
    - **SharedKernel**: Shared components

- **tests**: Contains the test projects
    - **CommonTestUtilities**: Shared testing utilities
    - **UnitTests**: Tests for individual components
    - **IntegrationTests**: Tests for component interactions
    - **FunctionalTests**: End-to-end API tests
    - **ArchitectureTests**: Tests for architecture compliance

## License

This project is licensed under the MIT License.
