# Seminar 3: Clean Architecture with Value Objects and Domain Events

## Task 1: Explore the Codebase

What Changed from Seminar 2? Familiarize yourself with the architectural changes and new features:

### 1. Clean Architecture Implementation

The project has been refactored from **3-layer architecture** to **Clean Architecture** with separate projects:

```
   Library.API    Library.Infrastructure
        ↓                ↓
         Library.Application
                ↓
           Library.Domain
```

**Key Changes:**

- **Library.Domain** - Pure domain layer with zero dependencies

  - Contains domain entities, aggregates, value objects
  - No references to infrastructure or frameworks

- **Library.Application** - Application layer

  - Commands, queries, and handlers (CQRS pattern)
  - Repository interfaces defined here
  - Depends only on Domain

- **Library.Infrastructure** - Infrastructure layer

  - EF Core implementation
  - Repository implementations
  - Depends on Domain & Application (dependency inversion)

- **Library.API** - Presentation layer
  - Controllers, middleware, configuration
  - Depends on all layers

**Benefits:**

- Clean dependency flow (dependencies point inward toward Domain)
- Domain logic is isolated and testable
- Infrastructure can be easily swapped without affecting business logic

### 2. Base Classes for Entities and Aggregates

- **Entity** ([Domain/Common/Entity.cs](Domain/Common/Entity.cs)) - Base class providing `Id` property
- **AggregateRoot** ([Domain/Common/AggregateRoot.cs](Domain/Common/AggregateRoot.cs)) - Extends `Entity`, enables publishing domain events via `RaiseDomainEvent()`

### 3. Domain Events Publishing

- Domain events are **raised in aggregates** using `RaiseDomainEvent()` method (e.g., `BookCreated` event in [Book.cs](Library.Domain/Aggregates/Book.cs))
- Domain events are defined in [Library.Domain/DomainEvents/](Library.Domain/DomainEvents/)
- Events are **automatically published** in overridden `SaveChangesAsync()` in [ApplicationDbContext](Library.Infrastructure/Data/ApplicationDbContext.cs)
- Event handlers are in Application layer (e.g., [BookCreatedHandler.cs](Library.Application/Authors/DomainEventHandlers/BookCreatedHandler.cs))
- Flow: Aggregate raises event → SaveChanges collects and publishes via MediatR → Event handlers respond

### 4. Refactored EF Core Configurations

- Separated EF Core configurations from domain entities into dedicated classes ([Infrastructure/Data/Configurations/](Infrastructure/Data/Configurations/))
- Each entity has its own `IEntityTypeConfiguration<T>` implementation
- Applied automatically via `ApplyConfigurationsFromAssembly()` in `ApplicationDbContext`

### Test Users:

```
admin@library.com / Admin@123
librarian@library.com / Librarian@123
member@library.com / Member@123
```

## Task 2: Value Objects - Reducing Primitive Obsession

### What are Value Objects?

Value Objects are immutable objects defined by their attributes rather than their identity. Unlike entities which have a unique ID, two value objects with the same attributes are considered equal.

### Example: Money Value Object

The codebase already has a good example: [Money](Library.Domain/ValueObjects/Money.cs)

```csharp
// Instead of primitive types:
decimal amount;
string currency;

// We use a Value Object:
Money amount = Money.Create(10.50m, "EUR");
```

**Benefits:**
- Encapsulates validation logic (no negative amounts, valid currency codes)
- Type safety (can't accidentally swap amount and currency)
- Immutable - operations return new instances
- Rich behavior (Add, Subtract operations with currency matching)

### Your Task

1. **Analyze the Loan aggregate** - Look at [Loan.cs](Library.Domain/Aggregates/Loan/Loan.cs) and identify properties that could be extracted into Value Objects. Think about:
   - Which primitive types (string, DateTime, etc.) represent domain concepts?
   - What validation rules could be encapsulated?
   - What behavior could be moved from the aggregate to a Value Object?

2. **Consider these examples** (or find your own):
   - **PhoneNumber** - For borrower contact information (validation, formatting)
   - **LoanPeriod** - For managing loan dates (StartDate, DueDate, ReturnDate with overdue calculations)
   - Or identify other primitives in Loan or other aggregates that would benefit from Value Object extraction

3. **Implement Value Objects**:
   - Create at least 2 Value Objects in `Library.Domain` project
   - Make them immutable
   - Add validation in factory methods (Create method)
   - Implement equality based on values (maybe you don't have to 😉)

4. **Refactor the aggregate** to use your new Value Objects instead of primitives

5. **Configure EF Core mapping** in the entity configuration files (`Library.Infrastructure/Data/EntityConfigurations/`) to map Value Objects to database columns or tables (owned entity types, value conversions, etc.)

## Task 3: Cross-Aggregate Communication

The application has a bug - the Book aggregate contains an `IsAvailable` property, but it always remains `true` even when books are loaned out. You should update this property based on the state of the Loan related to this particular Book.

Your task is to fix this bug. Keep in mind that Book and Loan are separate aggregates, so you should not modify both in the same command handler. Use the appropriate DDD tactical pattern to handle cross-aggregate communication 😉
