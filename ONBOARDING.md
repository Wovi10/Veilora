# Onboarding

Step-by-step setup for running Veilora locally. See [README.md](README.md) for a feature overview and architecture notes, and [CLAUDE.md](CLAUDE.md) for coding conventions.

## 1. Prerequisites

- **.NET 10 SDK**
- **Node.js 20+** and npm (for the React frontend)
- A **PostgreSQL** database — either a free [Neon](https://neon.tech) project (what this repo is set up for) or a local Postgres instance
- The **EF Core CLI tool**, if you don't already have it:
  ```bash
  dotnet tool install -g dotnet-ef
  ```

## 2. Clone and restore

```bash
git clone <repo-url>
cd Veilora
dotnet restore
cd src/Veilora.Api/ClientApp && npm install && cd -
```

## 3. Configure secrets

Two values are required and are **never committed** — they're stored via [.NET user secrets](https://learn.microsoft.com/aspnet/core/security/app-secrets):

```bash
dotnet user-secrets set "Jwt:Key" "a-random-string-at-least-32-characters-long" --project src/Veilora.Api
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=<host>;Database=<db>;Username=<user>;Password=<password>;SSL Mode=VerifyFull;Channel Binding=Require" --project src/Veilora.Api
```

> **Watch the connection string format.** Neon's dashboard gives you a copy-paste connection string in `postgresql://user:pass@host/db?sslmode=require` URI form. Npgsql (the EF Core provider this project uses) does **not** accept that format — it needs the ADO.NET `Key=Value;Key=Value` form shown above. Pasting the URI form in verbatim will fail with a confusing `KeyNotFoundException` when you try to run migrations or start the API.

## 4. Apply database migrations

```bash
dotnet ef database update --project src/Veilora.Infrastructure --startup-project src/Veilora.Api
```

## 5. Run the app

```bash
# Terminal 1 — API (Swagger UI at the printed https://localhost:7xxx/swagger)
dotnet run --project src/Veilora.Api

# Terminal 2 — frontend dev server
cd src/Veilora.Api/ClientApp
npm run dev
```

The frontend proxies API calls, so open the Vite dev server URL (typically `http://localhost:5173`).

## 6. Local email (forgot-password, etc.) without a real SMTP provider

The forgot-password flow sends email via SMTP (`src/Veilora.Infrastructure/Email/EmailSender.cs`). For local development you don't need a real mailbox — [`smtp4dev`](https://github.com/rnwood/smtp4dev) is a lightweight SMTP server with a web UI that just captures whatever is sent to it.

```bash
dotnet tool install -g Rnwood.Smtp4dev
smtp4dev --smtpport=2525 --urls=http://localhost:5050
```

Leave that running, then open `http://localhost:5050` to see captured emails as they arrive — no real inbox, no external service.

Point the API at it by creating `src/Veilora.Api/appsettings.Development.json` (this file is gitignored by design, like `appsettings.Neon.json` — it's your own local override and is never committed):

```json
{
  "Email": {
    "Host": "localhost",
    "Port": "2525",
    "FromAddress": "noreply@veilora.local",
    "FromName": "Veilora (dev)",
    "Username": "",
    "Password": "",
    "UseSsl": "false"
  }
}
```

This is picked up automatically when `ASPNETCORE_ENVIRONMENT=Development` (the default for `dotnet run`, per [launchSettings.json](src/Veilora.Api/Properties/launchSettings.json)) — no other config changes needed.

If `smtp4dev` fails to start because it also tries to bind the IMAP (143) or POP3 (110) ports and your OS reserves those for root, disable them — you don't need them for this workflow:

```bash
smtp4dev --smtpport=2525 --urls=http://localhost:5050 --imapport= --pop3port=
```

For a real deployment, set actual SMTP credentials via user secrets / environment variables for the `Email:*` keys in [appsettings.json](src/Veilora.Api/appsettings.json) (`Username`, `Password`, etc.) — never commit real credentials there.

## 7. Run the tests

```bash
dotnet test

# or individually
dotnet test src/tests/Veilora.UnitTests
dotnet test src/tests/Veilora.IntegrationTests
```
