# Seminar 4 & 5: Yestino – Event-Driven E-Commerce Platform

Yestino is a **modular monolith** e-commerce application built with DDD and event-driven architecture.

**Existing modules:** ProductCatalog, Warehouse
**Your task:** Implement the Ordering module with async communication via domain events.

---

## Task 1: Setup Database and Run Migrations

### 1.1 Start PostgreSQL Database

Run the following command to start a PostgreSQL container:

```bash
docker run -d \
--name YestinoDb \
-e POSTGRES_USER=postgres \
-e POSTGRES_PASSWORD=postgres \
-e POSTGRES_DB=YestinoDb \
-p 1234:5432 \
postgres:latest
```

### 1.2 Apply Migrations for All Modules

Each module has its own database schema. Apply migrations for each:

**ProductCatalog Module:**
```bash
dotnet ef database update \
  --project Yestino.ProductCatalog/Yestino.ProductCatalog.csproj \
  --startup-project Yestino/Yestino.csproj \
  --context ProductCatalogDbContext
```

**Warehouse Module:**
```bash
dotnet ef database update \
  --project Yestino.Warehouse/Yestino.Warehouse.csproj \
  --startup-project Yestino/Yestino.csproj \
  --context WarehouseDbContext
```

**Verify Setup:**
```bash
dotnet run --project Yestino/Yestino.csproj
```

---

## Task 2: Create and Register the Ordering Module

- Create a new project for the Ordering module
- Add necessary package references (EF Core, Npgsql, etc.) similar to other modules.
- Create `DependencyInjection.cs` in the Ordering module.
- Register in Program.cs
- Configure Wolverine Assembly Discovery in [Yestino/Wolverine/WolverineSetup.cs](Yestino/Wolverine/WolverineSetup.cs)
- Create Init migration and apply it.


---

## Task 3: Implement Ordering Commands from Event Storming

Implement commands from the event storming session. Create aggregates, read models, commands, handler, endpoints. 

---

## Task 4: Implement Async Module Integration via Domain Events

**Create domain events** in `Yestino.OrderingContracts` project (inheriting from `DomainEvent`).

**Raise events from aggregates** using `RaiseDomainEvent()` method (events are auto-published via `DbContextBase.SaveChangesAsync()`) or as Cascading Messages (Wolverine way).

**Create event handlers in other modules** (e.g., Warehouse module reacts to `OrderPlaced`):
- Wolverine automatically discovers and routes handlers
- Modules communicate **only via domain events**, no direct references

---

## Resources

- **Wolverine Documentation**: https://wolverinefx.net/

