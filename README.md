# Enterprise Work Management Portal

Capstone project for the .NET + Angular roadmap (Phase 2, Days 16-20).

An ASP.NET Core 8 Web API backed by SQL Server, with an Angular 18 + PrimeNG front end laid
out in the Apollo admin style. It manages users, roles, projects, tasks, comments, attachments,
dashboards and audit information.

## Layout

```
Project/
├── WorkPortal.sln
├── src/WorkPortal.Api/          ASP.NET Core 8 Web API
│   ├── Entities/                AppUser, Project, WorkTask, Comment, Attachment, RefreshToken
│   ├── Data/                    AppDbContext, DbSeeder, Migrations
│   ├── Dtos/                    request and response contracts
│   ├── Services/                auth, users, projects, tasks, attachments, dashboard
│   ├── Controllers/             thin HTTP endpoints
│   ├── Middleware/              global errors, request logging with a correlation id
│   └── Common/                  PagedResult, ApiError, typed exceptions
├── tests/WorkPortal.Tests/      42 unit, controller and integration tests
├── client/                      Angular 18 + PrimeNG
├── scripts/                     smoke tests (API, files, UI)
├── docs/                        API notes, deployment guide, user guide
└── WorkPortal.postman_collection.json
```

## Run it

**1. Backend** — creates and seeds the database on first run.

```bash
dotnet run --project src/WorkPortal.Api
```

Swagger: <http://localhost:5080/swagger> · Health: <http://localhost:5080/health>

**2. Frontend**

```bash
cd client && npm install && npm start
```

App: <http://localhost:4200>

The connection string in `appsettings.json` points at
`(localdb)\MSSQLLocalDB;Database=WorkPortal`. The API applies migrations and seeds data at startup.

## Seeded accounts

| Email | Password | Role |
|---|---|---|
| `admin@workportal.com` | `Admin123!` | Admin |
| `alex@workportal.com` | `User1234!` | User |
| `sam@workportal.com` | `User1234!` | User |

## What each role can do

| Action | Admin | User |
|---|---|---|
| See the dashboard, projects and tasks | yes | yes |
| Create and edit tasks, comment, attach files | yes | yes |
| Create, edit and delete projects | yes | no (403) |
| Delete tasks | yes | no (403) |
| Manage users | yes | no (guard redirects to /forbidden) |
| Delete a comment | any | own only |

## Requirements coverage

| Requirement | Where |
|---|---|
| Login, logout, JWT access + refresh flow | `AuthService`, `TokenService`, `authInterceptor` |
| Admin and User roles, protected endpoints, role-based menus | `[Authorize(Roles)]`, `adminGuard`, sidebar filter |
| Users, Projects, Tasks CRUD with relationships and audit fields | entities carry `CreatedBy/At`, `UpdatedBy/At` |
| Dashboard cards and charts | `DashboardController`, `p-chart` doughnut and bar |
| Search, filtering, sorting, pagination | `PagedQuery` + server-side `p-table` lazy load |
| Reactive forms with front-end and back-end validation | dialogs use `FormBuilder`; API re-validates |
| Task comments and file attachments | `TasksController`, `AttachmentService` |
| Consistent errors, structured logging, health check | `ExceptionMiddleware`, Serilog, `/health` |
| Backend tests | 42 tests — `dotnet test` |
| Swagger, Postman collection, README, deployment guide | this repo |
| IIS deployment | `docs/deployment-guide.md`, `client/src/web.config` |

## Tests

```bash
dotnet test
```

42 passing: service unit tests (paging, filtering, sorting, audit stamping, comment ownership),
controller tests with Moq (status codes, `CreatedAtAction` location), and integration tests through
`WebApplicationFactory` (real routing, model binding, auth and the JSON error shape).

Three smoke scripts under `scripts/` exercise a running instance:

```powershell
powershell -File scripts\api-smoke-test.ps1     # 45 API checks
powershell -File scripts\api-file-test.ps1      # upload, download, rejected types and sizes
node scripts\ui-smoke-test.mjs                  # 31 browser checks (needs puppeteer)
```

## Docs

- [`docs/api-notes.md`](docs/api-notes.md) — endpoints, conventions, security notes
- [`docs/deployment-guide.md`](docs/deployment-guide.md) — IIS publish and troubleshooting
- [`docs/user-guide.md`](docs/user-guide.md) — how to use each screen
