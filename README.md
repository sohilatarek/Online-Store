# 🛍️ Online Store Management System

A modern e-commerce management platform built with **Angular 20**, **.NET 8**, and **ABP Framework**. Enterprise-grade solution for managing product catalogs with advanced inventory control, distributed caching, and multi-tenant architecture.

## Overview

Enterprise-grade e-commerce management system with 26 RESTful API endpoints for managing categories, products, and inventory at scale. Built on ABP Framework's layered architecture following Domain-Driven Design (DDD) principles.

---

## Core Features

- **Category Management** - CRUD operations with display ordering and activation controls
- **Product Management** - Full lifecycle with publishing workflow and advanced search
- **Inventory Control** - Stock tracking, bulk updates (1000+ items), adjustments, and alerts
- **Distributed Caching** - Redis-powered caching for sub-100ms response times
- **Multi-Tenancy** - Complete data isolation for SaaS deployments
- **Security** - OAuth 2.0 authentication with role-based access control
- **Multi-Language** - Arabic and English localization
- **Audit Trail** - Complete change tracking with soft delete

---

## Technology Stack

**Backend:**
- .NET 8.0 + ABP Framework 10.0
- Entity Framework Core + SQL Server
- Redis (Distributed Caching)
- OpenIddict (OAuth 2.0)

**Frontend:**
- Angular 20 (Standalone Components)
- TypeScript + RxJS
- ABP Theme Lepton X

**Architecture:**
- Domain-Driven Design (DDD)
- Repository Pattern + CQRS
- Multi-layered architecture

---

## API Endpoints

**Categories API (11 endpoints):**
- Paginated lists with filtering
- CRUD operations
- Display order management
- Activate/Deactivate controls

**Products API (15 endpoints):**
- Advanced search and filtering
- CRUD operations
- Publish/Unpublish workflow
- Stock management (individual, bulk, adjustments)
- Stock availability checks
- Low stock and out-of-stock reports

---

## Performance

- Category List: <100ms (cached)
- Product Search: <150ms
- Bulk Stock Updates: <500ms (1000 items)
- 30-minute cache for categories
- 15-minute cache for products

---

## Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet) or later
- [Node.js v18 or v20](https://nodejs.org/en)
- [SQL Server](https://www.microsoft.com/sql-server) 2019 or later
- [Redis](https://redis.io/download) (optional, for caching)

---

## Quick Start

### 1. Install Dependencies
```bash
abp install-libs
```

### 2. Configure Database
Update connection string in `appsettings.json` under:
- `OnlineStore.HttpApi.Host`
- `OnlineStore.DbMigrator`

### 3. Run Database Migrations
```bash
cd src/OnlineStore.DbMigrator
dotnet run
```

### 4. Generate Signing Certificate (First Time)
```bash
dotnet dev-certs https -v -ep openiddict.pfx -p 407836b8-42eb-4130-80a0-f257e0640cbf
```

### 5. Run the Application

**Backend:**
```bash
cd src/OnlineStore.HttpApi.Host
dotnet run
```

**Frontend:**
```bash
cd angular
npm start
```

**Access:**
- API: `https://localhost:44327`
- Swagger: `https://localhost:44327/swagger`
- Frontend: `http://localhost:4200`

**Default Login:** `admin` / `1q2w3E*`

---

## Solution Structure

```
OnlineStore/
├── src/
│   ├── OnlineStore.Domain/              # Entities & domain logic
│   ├── OnlineStore.Application/         # Use cases & DTOs
│   ├── OnlineStore.EntityFrameworkCore/ # Data access
│   ├── OnlineStore.HttpApi/             # API controllers
│   ├── OnlineStore.HttpApi.Host/        # API host
│   └── OnlineStore.DbMigrator/          # Database migrations
├── angular/                             # Angular frontend
└── test/                                # Test projects
```

---

## Security

- OAuth 2.0 authentication (JWT tokens)
- Permission-based authorization
- Multi-tenant data isolation
- Input validation (FluentValidation)
- Audit logging with soft delete



---

**Built with ABP Framework, Angular, and .NET 8**
