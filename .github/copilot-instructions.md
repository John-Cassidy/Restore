# Restore E-Commerce Application

Full-stack e-commerce store with .NET 8 API (Clean Architecture + Minimal APIs + MediatR) and React 18 + TypeScript + Redux Toolkit client.

## Architecture Overview

### Backend: Clean Architecture with Vertical Slices

- **Restore.API**: Minimal API endpoints (no controllers), organized by feature modules in `/Endpoints`
- **Restore.Application**: MediatR handlers, commands, queries, DTOs, validators (FluentValidation)
- **Restore.Core**: Domain entities, repository interfaces, custom results/exceptions
- **Restore.Infrastructure**: EF Core DbContext, repository implementations, authentication services

**Key Pattern**: Use MediatR CQRS pattern - Commands/Queries in `Application`, Handlers process requests, minimal API endpoints in `API/Endpoints` modules (e.g., `ProductsModule.cs`, `BasketModule.cs`).

### Frontend: React + Redux Toolkit

- **State Management**: Redux Toolkit with feature-based slices (`catalogSlice`, `basketSlice`, `accountSlice`)
- **API Layer**: Centralized `agent.ts` with axios interceptors for auth, error handling, pagination headers
- **Features**: Organized by domain (`catalog`, `basket`, `account`, `orders`, `checkout`) with co-located components/slices
- **UI**: Material-UI (MUI) components, dark/light theme support, react-hook-form + yup validation

## Development Workflow

### Running the Application

**Full Stack (Docker Compose):**

```powershell
docker-compose up
# API: http://localhost:5000
# Client: Vite dev server needs separate start
```

**Client Development:**

```powershell
cd client
npm install
npm start  # Vite dev server on port 3000
```

**API Development:**

```powershell
cd server/Services/Restore/Restore.API
dotnet run  # Runs on port 5000
```

**Environment Variables:**

- Client: Uses Vite env vars (`VITE_API_URL` in `.env` files)
- API: Configured in `docker-compose.override.yml` or `appsettings.Development.json`
- CORS: Client address configured via `Cors:ClientAddress` setting

### Database Migrations (PostgreSQL)

```powershell
# Create migration from solution root
dotnet ef migrations add MigrationName -o Data/Migrations -p server/Services/Restore/Restore.Infrastructure -s server/Services/Restore/Restore.API

# Remove last migration
dotnet ef migrations remove -p server/Services/Restore/Restore.Infrastructure -s server/Services/Restore/Restore.API
```

**Connection String** in `docker-compose.override.yml`: `Server=restoredb;Port=5432;Database=store;User Id=appuser;Password=secret;`

## Code Conventions

### Backend (C#/.NET)

1. **Minimal APIs over Controllers**: Define endpoints in static modules (`*Module.cs`) using `MapGet`, `MapPost`, etc.

   ```csharp
   endpoints.MapGet("/api/products", async (IMediator mediator, [AsParameters] ProductParams params) => {...})
   ```

2. **MediatR Pattern**:

   - Commands/Queries implement `IRequest<TResponse>`
   - Handlers implement `IRequestHandler<TRequest, TResponse>`
   - Example: `GetProductsQuery` → `GetProductsHandler`

3. **Result Pattern**: Use `Result<T>` from `Restore.Core.Results` for operations that can fail (replaces exceptions for flow control)

4. **Extension Methods**: Service registration and pipeline config in `/Extensions` (e.g., `HostingExtensions.cs`)

5. **Validation**: FluentValidation validators in `Restore.Application/Validators`, manually invoked in endpoints

### Frontend (React/TypeScript)

1. **Redux Slices**: Use `createSlice` with `createAsyncThunk` for API calls

   ```typescript
   export const fetchProductsAsync = createAsyncThunk<IProduct[], void, { state: RootState }>(...);
   ```

2. **Typed Hooks**: Import `useAppDispatch` and `useAppSelector` from `configureStore.ts` (never plain `useDispatch`/`useSelector`)

3. **API Agent Pattern**: All API calls go through `agent.ts` - organized by resource (`Catalog`, `Basket`, `Account`, etc.)

   ```typescript
   agent.Catalog.list(params); // Not direct axios calls
   ```

4. **Form Validation**: Use `react-hook-form` + `yup` schemas (see `/checkout/checkoutValidation.ts`, `/admin/productValidation.ts`)

5. **Pagination**: Server returns metadata in `pagination` header; client stores in slice `metaData`

## Integration Points

- **Authentication**: JWT tokens in cookies, interceptor adds `Authorization: Bearer {token}` header
- **Stripe Payments**: `@stripe/stripe-js` + `@stripe/react-stripe-js` for checkout, webhook endpoint at `/api/payments/webhook`
- **Image Uploads**: `react-dropzone` on client, multipart/form-data handling in API
- **XSRF Protection**: Antiforgery tokens via `X-XSRF-TOKEN` header

## Testing & CI/CD

- GitHub Actions workflow: `.github/workflows/build-and-test.yaml`
- Docker image build: `.github/workflows/docker-push.yaml`

## Key Files to Reference

- Backend entry point: [server/Services/Restore/Restore.API/Program.cs](server/Services/Restore/Restore.API/Program.cs)
- Service configuration: [server/Services/Restore/Restore.API/Extensions/HostingExtensions.cs](server/Services/Restore/Restore.API/Extensions/HostingExtensions.cs)
- Client entry: [client/src/app/layout/App.tsx](client/src/app/layout/App.tsx)
- Redux store: [client/src/app/store/configureStore.ts](client/src/app/store/configureStore.ts)
- API client: [client/src/app/api/agent.ts](client/src/app/api/agent.ts)
