# Bakery Order App

A full-stack ASP.NET Core MVC application simulating a bakery ordering system. Built as an architectural showcase applying multiple enterprise design patterns in a real-world scenario.

> **Live Repository:** [github.com/emreokanbaskaya1/10-CQRS_My_Academy](https://github.com/emreokanbaskaya1/10-CQRS_My_Academy)

---




https://github.com/user-attachments/assets/ab497fde-e5fa-4c7c-9b32-e962d6b796b8




---

## Purpose

This project was built to demonstrate the practical application of enterprise-level design patterns in a .NET 9 web application — not just as isolated examples, but wired together in a cohesive, working system.

---

## Tech Stack

| Layer | Technology |
|-------|------------|
| Runtime | .NET 9.0 |
| Framework | ASP.NET Core MVC |
| ORM | Entity Framework Core |
| Database | SQL Server (LocalDB) |
| Auth | ASP.NET Core Identity |
| Logging | Serilog (Console + SQL Server sink) |
| Cloud | Google Cloud Storage |
| Frontend | Bootstrap 5, Chart.js, SweetAlert2, Owl Carousel |

---

## Architecture & Design Patterns

### CQRS (Command Query Responsibility Segregation)
All data operations are separated into **Commands** (write) and **Queries** (read). Each has its own handler, keeping responsibilities isolated and the codebase easy to extend.

```
CQRSPattern/
|-- Commands/        # CreateProductCommand, UpdateOrderStatusCommand, ...
|-- Queries/         # GetProductsQuery, GetOrderByIdQuery, ...
|-- Handlers/        # One handler per command/query
`-- Results/         # Typed result objects for queries
```

### Chain of Responsibility
Used for multi-step validation pipelines. Order processing passes through a chain of handlers — each responsible for one validation concern — before being committed.

```
OrderValidation chain:
  ProductAvailabilityHandler
    -> PromoCodeValidationHandler
      -> PriceCalculationHandler
        -> OrderValidationHandler

ContactValidation chain:
  EmailValidationHandler
    -> SpamCheckHandler
      -> ContactValidationHandler
```

### Mediator Pattern
Decouples the controllers from business logic for testimonial and contact workflows. Commands such as `CreateTestimonialCommand`, `ApproveTestimonialCommand`, and `SendContactMessageCommand` are dispatched through a mediator, with `LoggingBehavior` applied as a cross-cutting pipeline behavior.

### Observer Pattern
Order and promotion events notify registered observers without tight coupling. Multiple observers react to the same event independently.

```
OrderSubject -> OrderLogObserver
             -> OrderNotificationObserver

PromotionSubject -> PromotionLogObserver
```

### Repository + Unit of Work
A generic `IRepository<T>` abstracts data access, and `IUnitOfWork` wraps multiple repository operations in a single transaction boundary.

---

## Application Overview

### Customer-Facing
- Product catalog with cart and checkout
- Promotion code support with discount calculation
- Contact form with spam and email validation
- Testimonial submission
- Photo gallery

### Admin Panel
- **Dashboard** — 7 Chart.js visualizations: order status distribution, products by category, 7-day order trend, monthly revenue, top 5 products, active/inactive product ratio, testimonial approval status
- Full CRUD for: Products, Categories, Sliders, Gallery, Services, Feature Steps, Promotions, Our History, Page Headers
- Order management with pagination and inline status updates
- Testimonial moderation (approve / reject)
- Contact message inbox
- System log viewer (Serilog ? SQL Server)

---

## Project Structure

```
MyAcademyCQRS/
|-- Areas/Admin/            # Isolated admin area (controllers + views)
|-- Controllers/            # Public-facing controllers
|-- CQRSPattern/            # Commands, queries, handlers, results
|-- DesignPatterns/
|   |-- ChainOfResponsibility/
|   |-- Mediator/
|   |-- Observer/
|   `-- UnitOfWork/
|-- Entities/               # Domain models + enums
|-- Context/                # EF Core DbContext
|-- Data/                   # Seed data
|-- Extensions/             # Service registration extensions
|-- Mappings/               # AutoMapper profiles
|-- Services/               # Cloud storage abstraction
`-- wwwroot/                # Static assets
```

---

## Database Schema

| Table | Purpose |
|-------|---------|
| `Products` | Product catalog |
| `Categories` | Product categories |
| `Orders` | Customer orders |
| `OrderItems` | Order line items |
| `Promotions` | Discount campaigns |
| `Testimonials` | Customer reviews |
| `Contacts` | Contact form submissions |
| `Sliders` | Homepage slider content |
| `PhotoGallery` | Gallery images |
| `Services` | Service offerings |
| `ServiceSteps` | Feature step entries |
| `OurHistory` | Brand history timeline |
| `PageHeaders` | Dynamic page header content |
| `AppLogs` | Serilog structured logs |

---

## Security

- ASP.NET Core Identity for authentication
- Role-based authorization on the Admin area
- Anti-forgery token (CSRF) on all forms
- EF Core parameterized queries (SQL Injection protection)
- File upload type and size validation

---

## License

[MIT](LICENSE)
