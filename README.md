# Track Distribution API

A Track Management API for a music distribution company — manages artists, tracks, and their
distribution status across DSPs (Spotify, Apple Music, YouTube Music).

Built for the Takwene Full-Stack Developer take-home task.

## Description

Backend and front-end for a music distribution company's internal tool: create artists and
tracks, submit tracks to one or more DSPs, and track their per-DSP distribution status
(pending / live / rejected) alongside the track's own lifecycle status (draft / submitted /
distributed). Built with Clean Architecture on the backend (.NET 9 Web API + EF Core + JWT
auth) and a React SPA on the front-end for browsing and managing tracks.

## Stack

- **Backend:** .NET 9 Web API, EF Core 9, SQL Server (LocalDB), JWT auth, FluentValidation — Clean Architecture (Domain / Application / Infrastructure / Api)
- **Frontend:** React 18 + Vite, React Router

## Project structure

```
TrackDistribution/
├── src/
│   ├── TrackDistribution.Domain/          # Entities, enums — no dependencies
│   ├── TrackDistribution.Application/     # DTOs, service interfaces, validators, business logic
│   ├── TrackDistribution.Infrastructure/  # EF Core DbContext, migrations, JWT service, DB seeder
│   └── TrackDistribution.Api/             # Controllers, Program.cs, appsettings
├── frontend/                              # React SPA
├── README.md
└── DECISIONS.md
```

## Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Node.js 18+](https://nodejs.org/)
- SQL Server LocalDB (ships with Visual Studio, or install via [SQL Server Express/LocalDB](https://learn.microsoft.com/en-us/sql/database-engine/configure-windows/sql-server-express-localdb))
- EF Core CLI tools: `dotnet tool install --global dotnet-ef`

## 1. Backend setup

### 1.1 Database

This project uses SQL Server LocalDB by default — no separate database server to install or run.
The connection string in `appsettings.json` already points at it:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=TrackDistributionDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
}
```

If you're on macOS/Linux without LocalDB available, point this at any reachable SQL Server
instance instead (connection string format stays the same, just swap the `Server=` value).

### 1.2 Create the solution file and restore

The `.sln` isn't committed (keeps the repo diff-free across IDEs) — create it once locally:

```bash
cd src
dotnet new sln -n TrackDistribution
dotnet sln add TrackDistribution.Domain/TrackDistribution.Domain.csproj
dotnet sln add TrackDistribution.Application/TrackDistribution.Application.csproj
dotnet sln add TrackDistribution.Infrastructure/TrackDistribution.Infrastructure.csproj
dotnet sln add TrackDistribution.Api/TrackDistribution.Api.csproj
dotnet restore
```

### 1.3 Run migrations

From the `src/` folder:

```bash
dotnet ef migrations add InitialCreate \
  --project TrackDistribution.Infrastructure \
  --startup-project TrackDistribution.Api

dotnet ef database update \
  --project TrackDistribution.Infrastructure \
  --startup-project TrackDistribution.Api
```

> The app also auto-applies pending migrations and seeds sample data on startup (see
> `Program.cs`), so once you run the API this step effectively self-heals — but running it
> manually first lets you inspect the generated schema and catch connection issues early.

### 1.4 Run the API

```bash
cd TrackDistribution.Api
dotnet run
```

- Swagger UI: **https://localhost:5081/swagger/index.html** (or `http://localhost:5080/swagger` on HTTP)
- On first run, the database is seeded automatically with 3 artists, 9 tracks (mixed genres/statuses), 3 DSPs, and sample distribution records.

## 2. How to obtain a JWT token

Demo credentials are in `appsettings.Development.json` (`admin` / `Admin123!`) — this is a minimal demo login, not a real user system (see DECISIONS.md).

```bash
curl -k -X POST https://localhost:5081/api/auth/token \
  -H "Content-Type: application/json" \
  -d '{"username":"admin","password":"Admin123!"}'
```

Response:
```json
{ "token": "eyJhbGciOi...", "expiresAtUtc": "2026-07-01T15:00:00Z" }
```

Use it on protected endpoints (`POST /api/artists`, `POST /api/tracks`, `POST /api/tracks/{id}/distribute`, `PATCH /api/tracks/{id}/status`):

```bash
curl -k https://localhost:5081/api/tracks/{id}/distribute \
  -X POST \
  -H "Authorization: Bearer <token>" \
  -H "Content-Type: application/json" \
  -d '{"dspIds":["<dsp-guid>"]}'
```

In Swagger, click **Authorize** (top right) and paste `Bearer <token>`.

The front-end's Login page does this for you and stores the token in `localStorage`.

## 3. Frontend setup

```bash
cd frontend
cp .env.example .env   # points at the API base URL — update to match your backend port
npm install
npm run dev
```

Runs on **http://localhost:5173**. Make sure the backend is running first, and that
`Cors:AllowedOrigin` in `appsettings.json` matches this URL (it does by default).

## API reference

| Method | Route | Auth | Description |
|---|---|---|---|
| POST | `/api/artists` | required | Create an artist |
| GET | `/api/artists` | — | List all artists |
| POST | `/api/tracks` | required | Create a track for an artist |
| GET | `/api/tracks?artistId=&genre=&status=` | — | List tracks with filters |
| GET | `/api/tracks/{id}` | — | Track details incl. DSP distribution statuses |
| POST | `/api/tracks/{id}/distribute` | required | Submit to one or more DSPs |
| PATCH | `/api/tracks/{id}/status` | required | Update track status |
| GET | `/api/dsps` | — | List DSPs (helper endpoint for the front-end) |
| POST | `/api/auth/token` | — | Demo login, returns a JWT |

All write endpoints require `Authorization: Bearer <token>`.

## Design notes

See `DECISIONS.md` for the AI-usage writeup required by the task. A few architecture notes:

- **Clean Architecture**: Domain has zero dependencies; Application defines `IApplicationDbContext` and depends only on Domain + abstractions; Infrastructure implements persistence/JWT; Api wires everything via DI and owns HTTP concerns only.
- **Validation split**: FluentValidation validators check request *shape* (required fields, ISRC format, valid enum values). Services check *business rules* that need a DB round-trip (artist exists, ISRC uniqueness, DSP exists). A global exception middleware turns both into consistent JSON error responses with proper status codes (400/404/409).
- **ISRC** is normalized (hyphens stripped, uppercased) and has a unique index at the DB level as a second line of defense behind the service-level check.
