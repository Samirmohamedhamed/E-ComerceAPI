# E-Commerce RESTful API (ASP.NET Core 8)

A RESTful back-end for an e-commerce store, built with **ASP.NET Core Web API**, **Entity Framework Core** and **SQL Server**.
It covers the full flow from browsing products to cart, favorites and **Stripe** checkout, with **JWT** authentication and an admin area.

![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)
![C#](https://img.shields.io/badge/C%23-239120?logo=csharp&logoColor=white)
![SQL Server](https://img.shields.io/badge/SQL%20Server-CC2927?logo=microsoftsqlserver&logoColor=white)
![JWT](https://img.shields.io/badge/Auth-JWT-000000?logo=jsonwebtokens)
![Stripe](https://img.shields.io/badge/Payments-Stripe-635BFF?logo=stripe&logoColor=white)

## Features

**Identity & security**
- Register, login and logout with **ASP.NET Identity** + **JWT bearer tokens**
- Email confirmation, resend confirmation, forget / reset password (OTP-based)
- Role-based authorization (Admin / Customer)

**Admin area** (`/api/Admin/...`)
- CRUD for **Products**, **Categories** and **Brands** (including product images)
- User management: list users, create users, change role, lock / unlock accounts
- List roles

**Customer area** (`/api/Customer/...`)
- Browse products and product details
- Shopping **cart**: add, increase / decrease quantity, remove items, total price
- **Favorites**: add, list, remove
- **Checkout** with **Stripe** (pay / success callbacks) and order creation

## Tech stack

| Layer | Technology |
|---|---|
| Framework | ASP.NET Core 8 Web API |
| ORM | Entity Framework Core (Code First, migrations) |
| Database | SQL Server |
| Auth | ASP.NET Identity, JWT Bearer |
| Mapping | Mapster (DTOs ↔ entities) |
| Payments | Stripe.net |
| Docs / testing | Swagger (Swashbuckle), Postman |

## Architecture

```
E-CommerceAPI/
├── Areas/
│   ├── Admin/Controllers      # Products, Categories, Brands, Users, Roles
│   ├── Customer/Controllers   # Home, Carts, Favorites, CheckOuts
│   └── Identity/              # Accounts (register, login, password reset…)
├── Data/                      # ApplicationDbContext
├── Models/                    # Entities (Product, Category, Brand, Cart, Order…)
├── DtosConfiguration/         # Request / Response DTOs
├── MappingConfiguration/      # Mapster configuration
├── Repositories/              # Generic Repository + specific repositories
│   └── IRepositories/         # Repository interfaces
└── Utility/                   # Email sender, Stripe settings
```

- **Repository pattern** with a generic `Repository<T>` and per-entity repositories
- **Dependency Injection** for all repositories and services
- **DTOs** to keep API contracts separate from database entities

## Getting started

1. Clone the repo and open `E-CommerceAPI.sln` in Visual Studio 2022.
2. Add your settings with **User Secrets** (right-click the project → *Manage User Secrets*) — never commit real keys:
   ```json
   {
     "ConnectionStrings": { "DefaultConnection": "Server=.;Database=ECommerceApi;Trusted_Connection=True;TrustServerCertificate=True" },
     "JWT": { "ValidIssuer": "https://localhost:7000", "ValidAudiance": "http://localhost:4200", "Secret": "<a-long-random-secret>" },
     "Stripe": { "SecretKey": "<your-stripe-test-key>" }
   }
   ```
3. Set the Gmail **App Password** used for sending emails as an environment variable named `EMAIL_APP_PASSWORD`.
4. Create the database:
   ```bash
   dotnet ef database update
   ```
5. Run the project and open **Swagger** at `/swagger`.

## Author

**Samir Mohamed Hamed** — Backend .NET Developer
[LinkedIn](https://www.linkedin.com/in/samir-mohamed-dotnet) · [GitHub](https://github.com/Samirmohamedhamed)
