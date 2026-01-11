# Restore Frontend - Docker Setup

This directory contains Docker configurations for the frontend application.

## Docker Files

- `Dockerfile` - Production build with Vite preview server
- `Dockerfile.dev` - Development container with Vite dev server and hot reload
- `.dockerignore` - Excludes unnecessary files from Docker build context

## Running the Frontend

All docker-compose files are located in the **root directory** of the project, not in the client folder.

### Backend Only (Default)

```powershell
# From the root directory
docker-compose up --build
```

This runs only the backend services (API + Database).

### Backend + Frontend (Production)

```powershell
# From the root directory
docker-compose -f docker-compose.yml -f docker-compose.override.yml -f docker-compose.client.yml up --build

# Or with detached mode
docker-compose -f docker-compose.yml -f docker-compose.override.yml -f docker-compose.client.yml up -d --build
```

### Backend + Frontend (Development with Hot Reload)

```powershell
# From the root directory
docker-compose -f docker-compose.yml -f docker-compose.override.yml -f docker-compose.client.dev.yml up --build

# Or with detached mode
docker-compose -f docker-compose.yml -f docker-compose.override.yml -f docker-compose.client.dev.yml up -d --build
```

## Features by Mode

### Production Mode

- Uses Vite's built-in preview server
- Optimized production build (minified, bundled)
- Runs on port 3000
- Lightweight Node.js serving
- Proper MIME types and SPA routing

### Development Mode

- Hot module replacement (HMR)
- Source code mounted as volumes
- Auto-reloads on file changes
- Vite dev server
- Runs on port 3000

## Environment Variables

The application uses Vite environment variables:

- **Development:** `VITE_API_URL=http://localhost:5000/api/`
- **Production:** `VITE_API_URL=/api/` (relative path)

These are configured in:

- `.env.development`
- `.env.production`

## Network Configuration

All services (frontend, backend API, and database) run on Docker's default bridge network and can communicate with each other using service names:

- Frontend can access API at `http://restore.api:5000`
- API can access database at `restoredb:5432`

## Common Commands

### View Logs

```powershell
# All services
docker-compose -f docker-compose.yml -f docker-compose.override.yml -f docker-compose.client.yml logs -f

# Specific service
docker-compose -f docker-compose.yml -f docker-compose.override.yml -f docker-compose.client.yml logs -f restore.client
```

### Stop Services

```powershell
docker-compose -f docker-compose.yml -f docker-compose.override.yml -f docker-compose.client.yml down
```

### Build Only

```powershell
# Production
docker-compose -f docker-compose.yml -f docker-compose.override.yml -f docker-compose.client.yml build

# Development
docker-compose -f docker-compose.yml -f docker-compose.override.yml -f docker-compose.client.dev.yml build
```

### View Running Containers

```powershell
docker ps
```

### Access Container Shell

```powershell
# Development
docker exec -it restore-client-dev sh

# Production
docker exec -it restore-client sh
```

## Port Mappings

- **Frontend (Both modes):** `3000:3000`
- **Backend API:** `5000:5000`
- **PostgreSQL:** `5432:5432`

## Volume Mounts (Development Only)

The development configuration mounts these directories for hot reload:

- `./client/src` - Application source code
- `./client/public` - Public assets
- `./client/index.html` - Entry HTML file
- Configuration files (vite.config.ts, tsconfig.json)

## Troubleshooting

### Hot Reload Not Working

If hot reload isn't working in development mode, ensure:

1. File watching is enabled (`usePolling: true` in vite.config.ts)
2. Volumes are correctly mounted
3. Container has been rebuilt after config changes

### Port Conflicts

If port 3000 is already in use:

1. Stop the conflicting service
2. Or modify the port mapping in the root `docker-compose.client.yml` or `docker-compose.client.dev.yml` files

### Build Failures

```powershell
# Clear Docker cache and rebuild (from root directory)
docker-compose -f docker-compose.yml -f docker-compose.override.yml -f docker-compose.client.yml build --no-cache
```

## Performance Notes

### Development Mode

- Slower startup (node_modules installation)
- Uses more memory (Node.js + Vite)
- Best for active development

### Production Mode

- Fast startup (pre-built static files)
- Lightweight Node.js serving
- Optimized for deployment
