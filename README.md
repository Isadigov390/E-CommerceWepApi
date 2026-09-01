# E-Commerce Shopping API

[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![Build and deploy](https://github.com/Isadigov390/E-CommerceWepApi/actions/workflows/main_shoppingwepapi.yml/badge.svg)](https://github.com/Isadigov390/E-CommerceWepApi/actions/workflows/main_shoppingwepapi.yml)

A REST API for an e-commerce application, built with ASP.NET Core and a layered architecture. The project covers product catalog management, account flows, image uploads, filtering, and deployment to Azure.

## Main features

- Account registration, email verification, login, logout, and password reset
- JWT access tokens with refresh-token rotation through an HTTP-only cookie
- Product, category, product detail, and product image management
- Product search, pagination, price and rating filters, stock filters, and sorting
- Single and multiple product image uploads
- Soft deletion and audit timestamps for database entities
- Central error handling with Problem Details responses
- Request validation with FluentValidation
- Swagger UI with Bearer token support
- GitHub Actions build and Azure App Service deployment

## Tech stack

| Area | Technology |
| --- | --- |
| API | ASP.NET Core Web API, .NET 8 |
| Database | SQL Server, Entity Framework Core |
| Authentication | JWT Bearer tokens, refresh tokens, BCrypt |
| Validation | FluentValidation |
| Email | MailKit over SMTP |
| File storage | Local file storage; Azure Blob Storage package included |
| API docs | Swagger / OpenAPI |
| Deployment | GitHub Actions, Azure App Service |

## Project structure

```text
Shopping/
|-- Shopping/                 # API controllers, configuration, and startup
|-- Shopping.Application/     # Services, DTOs, validation, and error handling
|-- Shopping.Domain/          # Entities, enums, and repository contracts
|-- Shopping.Infrastructure/  # Repository, authentication, email, and storage code
|-- Shopping.Persistence/     # EF Core context, mappings, and migrations
`-- Shopping.sln
```

The dependencies point inward: the API connects the layers, the Application layer contains the use cases, and the Domain layer holds the core models and contracts.

## API overview

| Route group | Purpose |
| --- | --- |
| `/api/accounts` | Registration, verification, login, token refresh, logout, and password reset |
| `/api/categories` | Category CRUD operations |
| `/api/products` | Product CRUD, product images, brands, search, filters, sorting, and pagination |
| `/api/productimages` | Upload, list, view, and delete product images |
| `/api/productdetails` | Create and read product details |

Protected routes accept an access token in this header:

```http
Authorization: Bearer <access-token>
```

## Run locally

### Requirements

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server or SQL Server Express
- SMTP account for email verification and password-reset flows
- EF Core CLI tools for applying migrations

### 1. Clone and restore

```bash
git clone https://github.com/Isadigov390/E-CommerceWepApi.git
cd E-CommerceWepApi
dotnet restore Shopping.sln
```

### 2. Add local secrets

The API project supports .NET User Secrets. Replace the example values below with your local settings:

```bash
dotnet user-secrets set --project Shopping/Shopping.WebApi.csproj "ConnectionStrings:cString" "Server=localhost;Database=ShoppingDb;Trusted_Connection=True;TrustServerCertificate=True"
dotnet user-secrets set --project Shopping/Shopping.WebApi.csproj "JwtSettings:Issuer" "ShoppingApi"
dotnet user-secrets set --project Shopping/Shopping.WebApi.csproj "JwtSettings:Audience" "ShoppingClient"
dotnet user-secrets set --project Shopping/Shopping.WebApi.csproj "JwtSettings:Key" "replace-with-a-long-random-secret-key"
dotnet user-secrets set --project Shopping/Shopping.WebApi.csproj "JwtSettings:ExpiryMinutes" "15"
dotnet user-secrets set --project Shopping/Shopping.WebApi.csproj "JwtSettings:RefreshTokenExpiryDays" "7"
dotnet user-secrets set --project Shopping/Shopping.WebApi.csproj "EmailSettings:Host" "smtp.example.com"
dotnet user-secrets set --project Shopping/Shopping.WebApi.csproj "EmailSettings:Port" "587"
dotnet user-secrets set --project Shopping/Shopping.WebApi.csproj "EmailSettings:SenderName" "Shopping API"
dotnet user-secrets set --project Shopping/Shopping.WebApi.csproj "EmailSettings:SenderEmail" "no-reply@example.com"
dotnet user-secrets set --project Shopping/Shopping.WebApi.csproj "EmailSettings:Username" "your-smtp-username"
dotnet user-secrets set --project Shopping/Shopping.WebApi.csproj "EmailSettings:Password" "your-smtp-password"
dotnet user-secrets set --project Shopping/Shopping.WebApi.csproj "PasswordResetSettings:ResetPageUrl" "http://localhost:5173/reset-password"
```

Do not commit real connection strings, JWT keys, or email passwords.

### 3. Create the database

Install the EF Core CLI if needed, then apply the included migrations:

```bash
dotnet tool install --global dotnet-ef
dotnet ef database update --project Shopping.Persistence --startup-project Shopping
```

### 4. Start the API

```bash
dotnet run --project Shopping
```

Open Swagger at `https://localhost:7270/swagger` or use the URL printed in the terminal.

## Build

```bash
dotnet build Shopping.sln --configuration Release
```

## Project status

This is an active learning project. The current version focuses on the backend API and its main e-commerce flows. More features and tests can be added as the project grows.

## Author

GitHub: [Isadigov390](https://github.com/Isadigov390)
