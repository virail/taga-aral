# TagaAral

A Tagalog learning app built on comprehensible input, with an AI subtitle generator
(Tagalog <-> English) and interactive lessons.

## Architecture

Clean Architecture with a layered .NET 8 backend, self-hosted ML sidecars, and a React
Native frontend. The dependency rule: **Infrastructure and Api depend on Core, never the
reverse**.

```
backend/
  TagaAral.Core/           # Domain: entities, value objects, contracts. No framework packages
  TagaAral.Infrastructure/ # EF Core, Redis, MinIO, BCrypt implementations of Core contracts
  TagaAral.Api/            # Web host: DI composition, controllers, JWT, Swagger
frontend/                  # React Native + Expo SDK 57 + React Native Paper
ml-services/               # Python FastAPI sidecars (faster-whisper, NLLB-200)
infra/
  docker/                  # docker-compose: Postgres 16, Redis 7, MinIO
```

## Tech Stack

- **Backend:** .NET 8, EF Core 8 + PostgreSQL 16, JWT + BCrypt, Redis 7 (Redis Streams job queue), MinIO, SignalR, Swagger
- **ML sidecars:** Python FastAPI - faster-whisper (large-v3, INT8) and NLLB-200 (600M), self-hosted
- **Frontend:** React Native + Expo SDK 57 + React Native Paper

## Getting Started

### 1. Start the infrastructure

```powershell
docker compose -f infra/docker/docker-compose.yml up -d
```

Brings up Postgres 16, Redis 7, and MinIO with healthchecks. Configuration is read from
`infra/docker/.env` (gitignored). Copy `infra/docker/.env.example` to `infra/docker/.env`
and set real values.

### 2. Run the backend

```powershell
dotnet build TagaAral.sln
dotnet run --project backend/TagaAral.Api
```

Swagger is available at `https://localhost:<port>/swagger` in Development.

### 3. Apply migrations

```powershell
dotnet ef database update --project backend/TagaAral.Infrastructure --startup-project backend/TagaAral.Api
```

## Configuration & Secrets

- Docker secrets live in `infra/docker/.env` (gitignored); `infra/docker/.env.example` documents the shape.
- The backend reads its connection string from `appsettings.Development.json` (gitignored)
  in Development; `appsettings.json` is committed with non-sensitive defaults.
- Never commit `.env` files or real credentials.
