# Conference (split repo)

This repo is split into two separate apps:

- **backend/**: ASP.NET Core API (EF Core + JWT auth)
- **frontend/**: Next.js (TypeScript + Tailwind) web client

## Backend

```powershell
cd backend
dotnet restore
dotnet run
```

The API listens on `http://localhost:8080`.

## Frontend

```powershell
cd frontend
npm install
npm run dev
```

The app runs on `http://localhost:3000` and calls the API at `http://127.0.0.1:8080` by default.
