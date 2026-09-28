# Book Catalog Platform — Design Note

*Living document. Week 1 version.*

## What I built

A REST API for a book catalog. It supports creating, reading, updating and deleting books. Input is validated, every operation that changes data is logged, and the API is documented with Swagger. For now, books are stored in memory, so they are lost when the application restarts.

## How it is structured

The solution has one project, `src/BookCatalog.Api`, split into folders:

- **Controllers** — `BooksController`, which exposes the HTTP endpoints.
- **Entities** — `Book`, the only model for now.
- **Interfaces** — `IBookStorage`, the contract for storing books.
- **Storage** — `BookStorage`, the in-memory implementation.

`Program.cs` is the entry point. All services are registered there, and the controller receives its dependencies through constructor injection.

A request goes through routing, then model binding and validation. If validation fails, `[ApiController]` returns a 400 before my code runs. Otherwise the controller calls `IBookStorage` and turns the result into an HTTP response. The controller depends on the interface, not on the concrete class.

## Decisions and why

### Storage: a Dictionary in memory, registered as a singleton

Books live in a `Dictionary<int, Book>` inside `BookStorage`, which gives fast lookup by id. `BookStorage` is registered as a **singleton**, so every request uses the same instance and the same data. Scoped would create a new instance for every HTTP request, and transient a new one every time it is injected. In both cases the books would be gone by the next request.

### Thread safety: one lock around every operation

Requests run in parallel, and a singleton is shared between them. Without protection, two requests could get the same id from the counter, or one request could read the dictionary while another is changing it. Every method therefore runs inside the same lock, so only one thread at a time can use the storage.

`GetAll` copies the values with `ToList()` while it still holds the lock, so the caller gets a snapshot and never reads the dictionary after the lock is released.

**Known condition:** the lock protects the dictionary, not the `Book` objects inside it. `GetById` returns the stored object itself, and `Update` stores the object it received from the controller. This is safe as long as no code modifies a stored `Book` directly. Today `Update` always replaces the whole object instead of editing it. Week 2 DTOs will remove this condition, because the storage will keep its own copies.

### Validation rules

- **Title** — required, 1–200 characters. Books must have a title so people can find and refer to them. Maximum 200 is reasonable length for books, used standard for validation, most of them does not have that long title.
- **Author** — optional, because books with unknown authors exist. If present: 2–100 characters, and not only whitespace. Some works have multiple authors, having upper bound 100 takes it into account. Authors do not have one letter name, so minimum length is 2.
- **Year** — optional, because it may be unknown. Negative numbers are BCE. The minimum is -3500, just before the earliest known writing (about 3400–3200 BCE), so no real work can be older. The maximum is next year, because books can be announced before publication or published late in the year.

Fixed limits are validation attributes. Rules that need logic (the next-year limit and the whitespace check) are in `Validate` (`IValidatableObject`). Numbers sent as strings (`"1990"`) are accepted; I left the framework default because it causes no harm.

### HTTP behaviour

| Endpoint | Responses |
|---|---|
| GET all | 200 |
| GET by id | 200, 404 |
| POST | 201 with `Location` header, 400 |
| PUT | 200, 400, 404 |
| DELETE | 204 |

- **Route constraint `{id:int}`** — a non-numeric id does not match the route, so the client gets 404 from routing instead of 400 from model binding. The behaviour is the same for GET, PUT and DELETE.
- **DELETE is idempotent** — it returns 204 even if the book did not exist. Deleting twice gives the same result, so a client can safely retry.
- **PUT is a full replacement** — fields not sent are cleared.
- **Id is assigned by the server** — an `id` in the POST or PUT body is ignored. PUT always uses the id from the route.
- All errors use the ProblemDetails format, which `[ApiController]` provides automatically.

### Logging

I use the built-in `ILogger<T>` with message templates (`"Created book {BookId}"`), not string interpolation. That way `BookId` is stored as a separate, searchable property, and no string is built when the level is disabled. I log IDs, not whole objects.

Create, update, delete and not-found cases are logged at **Information**. I first logged not-found at Warning, assuming a correct frontend never produces a 404. I changed it, because 404s also happen normally: an old bookmark, a book deleted in another tab, clients other than my frontend. Warnings that fire in normal situations teach you to ignore Warnings.

Unhandled exceptions are logged at Error by the framework itself. I tested this with a temporary exception, so I did not add try/catch blocks that would only log the error a second time.

### HTTPS

I removed HttpsRedirection, that gave console warnings and is not actually recomended by microsoft to use as a protection. In real deployments HTTPS is usually handled by a proxy in front of the app, and redirection is weak protection for an API.

### Swagger

The OpenAPI document comes from Microsoft's built-in generator, and Swagger UI shows it. Both are only enabled in Development, because the document reveals the whole API surface. Each endpoint declares its possible responses with `[ProducesResponseType]`, and XML comments describe the behaviour the generator cannot see, such as the rules inside `Validate`. Disabled CS1592 warnings indicating descriptions on each method, even the ones not needed (constructors, classes), in .csproj

## What I would improve with more time

- **The entity is also the API contract.** One `Book` class is both my internal model and what clients send and receive. Because of that, `id` appears in the POST schema, its description ("ignored") also shows on responses where it makes no sense, the storage keeps the controller's object, and any internal field I add would be exposed automatically. Week 2 DTOs will separate them.
- **Production 500s have an empty body** instead of ProblemDetails. Clients should get the same error format everywhere, with a `traceId` and no internal details. Week 2 adds centralized error handling.
- **The thread-safety condition** described above.
- **`Validate` reads the system clock directly**, which will make the next-year rule hard to test.
- **GET all returns everything**, with no pagination.
- **Data is lost on restart.** Week 3 moves storage to a database.

