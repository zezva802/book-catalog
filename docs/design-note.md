# Book Catalog Platform — Design Note

*Living document. Week 2.*

## What it is
- REST API for books: CRUD + paged, filtered list.
- Validation, logging, one error format (ProblemDetails), Swagger.
- Storage in memory (lost on restart) → database in week 3.

## Structure
| Project | Job |
|---|---|
| Api | HTTP ↔ use cases: routes, DTOs, request validation, status codes, error handler |
| Application | Use cases (`BookService`), business rules, defines `IBookRepository` |
| Domain | `Book` entity. References nothing |
| Infrastructure | Implements `IBookRepository` (in memory) |

- Dependencies point inward: Api → Application → Domain; Infrastructure → Application.
- Projects, not folders → compiler enforces direction, cycles are build errors.
- `Program.cs` = only place that knows concrete types.
- Domain separate although tiny: week 3 adds Author, User, Loan.

## Data access abstraction
- `IBookRepository`: `GetByIdAsync`, `ListAsync(BookListQuery)`, `CreateAsync`, `UpdateAsync`, `DeleteAsync`.
- Lives in Application (owner = user of it), implemented in Infrastructure → dependency inversion.
- Hides: storage tech, query mechanism, concurrency (lock), lifetime/connections, id generation.
- Contract beyond signatures: missing book → `null` / `false`, never throws; `TotalCount` = all matches; pages start at 1.
- No `IQueryable`, no generic `IRepository<T>`, one list method with a query object.
- Against it: EF's `DbSet`/`DbContext` already are repository/unit of work → duplication, hidden EF features and cost. Kept for swappable storage + DB-free unit tests.

## Validation rules
- **Title** — required, 1–200. Needed to find/refer to a book; 200 covers real titles.
- **Author** — optional (unknown authors exist). If present: 2–100, not only whitespace. 100 leaves room for several names; no one-letter names.
- **Year** — optional (may be unknown). Negative = BCE. Min −3500 (just before earliest writing, ~3400–3200 BCE). Max next year (announced / late-year publications).
- Fixed limits = attributes. Logic rules (next year, whitespace) = `IValidatableObject.Validate`; next year uses `TimeProvider`.
- List request: page ≥ 1, pageSize 1–100, yearFrom ≤ yearTo.
- Numbers sent as strings (`"1990"`) accepted — framework default, harmless.

## HTTP behaviour
| Endpoint | Responses |
|---|---|
| GET list | 200, 400 |
| GET by id | 200, 404 |
| POST | 201 + `Location`, 400 |
| PUT | 200, 400, 404 |
| DELETE | 204 |

- `{id:int}` → non-numeric id = 404 from routing (same for GET, PUT, DELETE).
- DELETE idempotent → 204 even if the book didn't exist; safe to retry.
- PUT = full replacement; fields not sent are cleared; id from route only.
- Client can't set `Id` (not in `BookRequest`).
- All errors = ProblemDetails; unexpected → generic 500 + `traceId`.

## Decisions
- **Async interface now** — Application and tests compile against it; changing it in week 3 would touch everything. Cost: `Task.FromResult` in memory. `CancellationToken` last.
- **Service layer** — use cases, rules, business logging, no HTTP. Thin now; home for lending rules + transactions.
- **`IBookService`** — controller depends on abstraction. Cost: one file, one implementation.
- **Not found = `null`/`bool`, not exceptions** — expected outcome. Revisit when there are several expected failures (not found vs already borrowed).
- **DTOs in Api, manual mapping** — stops over-posting (client-set Id), decouples JSON from entity, avoids leaks/cycles later. Extension methods: compile-checked, explainable.
- **One `BookRequest` for POST + PUT** — PUT is full replacement, same fields; splitting later is cheap.
- **Pagination** — offset (page + size). Sort Title then Id. Filter → count → sort → skip/take, in the repository (→ SQL in week 3). Page size max 100. Invalid input → 400; page past end → 200, empty, real total. Weakness: drifts if data changes between pages.
- **Filters** — title/author contains, case-insensitive; years inclusive; no-year books excluded; AND.
- **Total count** — second query in a DB, can't stop early. Fallback: "has next page".
- **Errors** — expected → 4xx where it happens. Unexpected → `GlobalExceptionHandler` (first in pipeline): Error log with method, path, traceId; client gets generic 500 + traceId. No stack traces to clients (paths, versions = reconnaissance). Client disconnect ≠ 500.
- **`TimeProvider`** — "year ≤ next year" testable with a frozen clock. Missing registration throws, no silent fallback.
- **Lifetimes** — repo Singleton (state in instance), service Scoped. Week 3: EF repo Scoped.
- **Logging** — message templates; business events in service at Information (incl. not found — normal, not Warning); unexpected errors logged once.
- **Other** — Swagger Development only (exposes whole API surface); no HTTPS redirection (proxy handles HTTPS).

## Testing
- Application.Tests: `BookService` (NSubstitute), `PagedResult`.
- Api.Tests: `BookRequest`, `BookListRequest`, mappings.
- Infrastructure.Tests: `ListAsync`.
- Logic tested, wiring (controller, handler, `Program.cs`) not → week 4 integration.
- Service tests solitary: 7 stubs (results), 1 mock (`DeleteAsync` must call repo).
- Both outcomes per method; boundaries for validation; data where right ≠ wrong (unsorted seeds, more matches than a page).
- No external deps, fresh instance per test, fake clock.
- Not tested on purpose: in-memory Get/Update/Delete — replaced in week 3.
- Verified by breaking code: count-of-page bug → exactly 3 paging tests failed.
- Coverage 56% (no generated code): logic/validation/mapping 85–100%, HTTP 0% by design. Coverage = lines ran, not checked (`Book` 100% with no tests).
- Week 3: service/validation/mapping tests unaffected; in-memory repo tests go with the class; EF repo untested until week 4 (reuse repo tests as contract tests).

## Painful to change from week 1
- Hardest: dependency inversion. One project, no business layer, `IBookStorage` next to the controller → ownership never decided. Got project references backwards first.
- Entity = API contract → POST had to reset `Id`; DTOs touched controller, validation, entity.
- Sync storage interface → every signature changed.
- Clock read in validation → rule had to change to be testable.
- Lesson: week 1 had one thing doing several jobs (`Book`: entity + contract + validation; controller: HTTP + logging + storage). Since splitting, each change touches one layer.

## Known limitations
- Data lost on restart.
- Repo shares `Book` references with callers (lock covers dictionary, not books).
- `ListAsync` filters twice (count + page).
- Offset drift.
- No integration tests yet.
- Template comments left in `Program.cs`.
