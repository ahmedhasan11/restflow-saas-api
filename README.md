
# Restflow | SaaS Restaurant Management System

Restflow is a high-performance, multi-tenant SaaS platform designed to streamline restaurant operations. Built with a focus on data integrity, security, and scalability, it provides a robust foundation for managing inventory, menus, and customers across multiple restaurant tenants.

## 🚀 Key Modules

### 1. Authentication & Authorization
- **Multi-Tenant Identity**: Secure JWT-based authentication with automatic tenant detection.
- **RBAC**: Role-Based Access Control (Super Admin, Restaurant Owner, Employee).
- **Security First**: Integrated OTP verification (Email/Phone) and Refresh Token rotation.

### 2. Inventory Management
- **Stock Tracking**: Real-time tracking of inventory quantities with negative stock prevention.
- **Financial History**: Immutable stock movement logs with locked-in unit costs for accurate historical valuation.
- **Low Stock Alerts**: Configurable thresholds to ensure the kitchen never runs out of essentials.

### 3. Product & Menu Management
- **Recipe Mapping**: Link products to specific inventory items with precise ingredient quantities.
- **Dynamic Menu**: Manage menu categories and product availability with ease.
- **Aesthetic Integration**: Support for product imagery and localized descriptions.

### 4. Customer Management
- **Lightweight CRM**: Store and manage customer contact information isolated per tenant.
- **Operational Readiness**: Prepared for future integration with order and loyalty modules.

### 5. Settings & Localization
- **Multi-Localization**: Per-tenant configuration for currency, timezone, and language.
- **Security Management**: Self-service profile updates and secure password rotation.

---

## 🛠 Tech Stack

- **Framework**: .NET 8.0 (ASP.NET Core Web API)
- **Database**: SQL Server
- **ORM**: Entity Framework Core
- **Architecture**: Clean Architecture principles with Fluent API configurations.

---

## 🏗 Architectural Highlights

### Multi-Tenancy (Shared Database)
The system uses a **Row-Level Isolation** strategy. Every tenant-specific table is partitioned by a `TenantId`. This approach provides the perfect balance between isolation and operational simplicity for SaaS scaling.

### Soft Delete & Auditing
Instead of permanent deletion, Restflow uses a **Soft Delete** strategy via a `DeletedAt` timestamp.
- **Global Query Filters**: EF Core automatically filters out deleted records across all queries.
- **Audit Trails**: Every entity tracks `CreatedAt`, `UpdatedAt`, and the associated `UserIds`.

### Data Integrity & Concurrency
- **Optimistic Concurrency**: Uses `RowVersion` tokens in inventory to prevent race conditions during simultaneous stock updates.
- **Fluent API**: Strict database constraints (Max lengths, Precisions, Unique Indices) are enforced at the schema level.

---

## 🏁 Getting Started

### Prerequisites
- .NET 8.0 SDK
- SQL Server (LocalDB or Express)

### Setup
1. Clone the repository.
2. Update the `ConnectionStrings:DefaultConnection` in `appsettings.json`.
3. Apply migrations:
   ```bash
   dotnet ef database update
