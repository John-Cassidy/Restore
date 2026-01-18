# Restore - Docker Compose Guide

This guide explains how to run the Restore application using Docker Compose.

## Available Configurations

### 1. Backend Only (Default)

Runs PostgreSQL database and .NET API only. Use this when developing the frontend locally with `npm start`.

**Command:**

```powershell
docker-compose up --build
```

**Services Started:**

- `restoredb` - PostgreSQL database (port 5432)
- `restore.api` - .NET API (port 5000)

### 2. Full Stack - Production

Runs backend + frontend with production build (optimized static files).

**Command:**

```powershell
docker-compose -f docker-compose.yml -f docker-compose.override.yml -f docker-compose.client.yml up --build

# Detached mode (background)
docker-compose -f docker-compose.yml -f docker-compose.override.yml -f docker-compose.client.yml up -d --build
```

**Services Started:**

- `restoredb` - PostgreSQL database (port 5432)
- `restore.api` - .NET API (port 5000)
- `restore.client` - React frontend with Vite preview server (port 3000)

### 3. Full Stack - Development

Runs backend + frontend with hot reload for active frontend development.

**Command:**

```powershell
docker-compose -f docker-compose.yml -f docker-compose.override.yml -f docker-compose.client.dev.yml up --build

# Detached mode (background)
docker-compose -f docker-compose.yml -f docker-compose.override.yml -f docker-compose.client.dev.yml up -d --build
```

**Services Started:**

- `restoredb` - PostgreSQL database (port 5432)
- `restore.api` - .NET API (port 5000)
- `restore.client.dev` - React frontend with Vite dev server + HMR (port 3000)

## Docker Compose Files

- **docker-compose.yml** - Base configuration for backend services
- **docker-compose.override.yml** - Development overrides for backend
- **docker-compose.client.yml** - Frontend production configuration
- **docker-compose.client.dev.yml** - Frontend development configuration with hot reload

## Common Commands

### Stop All Services

```powershell
# For backend only
docker-compose down

# For full stack (production)
docker-compose -f docker-compose.yml -f docker-compose.override.yml -f docker-compose.client.yml down

# For full stack (development)
docker-compose -f docker-compose.yml -f docker-compose.override.yml -f docker-compose.client.dev.yml down
```

### View Logs

```powershell
# All services
docker-compose logs -f

# Specific service
docker-compose logs -f restore.api
docker-compose logs -f restoredb
docker-compose -f docker-compose.yml -f docker-compose.override.yml -f docker-compose.client.yml logs -f restore.client
```

### Rebuild Specific Service

```powershell
# Backend API
docker-compose build restore.api

# Frontend (production)
docker-compose -f docker-compose.client.yml build restore.client

# Frontend (development)
docker-compose -f docker-compose.client.dev.yml build restore.client.dev
```

### Clean Up (Remove All Containers, Networks, Volumes)

```powershell
docker-compose down -v
docker-compose -f docker-compose.yml -f docker-compose.override.yml -f docker-compose.client.yml down -v
```

## Access Points

After starting the services:

- **Frontend:** http://localhost:3000
- **Backend API:** http://localhost:5000
- **Database:** localhost:5432
  - Database: `store`
  - User: `appuser`
  - Password: `secret`

## Recommended Workflows

### Backend Development

When working only on the backend:

```powershell
# Start backend in Docker
docker-compose up -d

# Run frontend locally
cd client
npm start
```

### Frontend Development

When working only on the frontend:

```powershell
# Start full stack with frontend hot reload
docker-compose -f docker-compose.yml -f docker-compose.override.yml -f docker-compose.client.dev.yml up -d

# Make changes to frontend code - changes auto-reload
# Files in client/src are mounted as volumes
```

### Full Stack Development

When working on both:

```powershell
# Option 1: Backend in Docker, Frontend local
docker-compose up -d
cd client && npm start

# Option 2: Everything in Docker with frontend hot reload
docker-compose -f docker-compose.yml -f docker-compose.override.yml -f docker-compose.client.dev.yml up -d
```

### Production Testing

Test the production build locally:

```powershell
docker-compose -f docker-compose.yml -f docker-compose.override.yml -f docker-compose.client.yml up --build
```

## Environment Variables

### Backend (API)

Configured in `docker-compose.override.yml`:

- `ASPNETCORE_ENVIRONMENT` - Development
- `ConnectionStrings__DefaultConnection` - PostgreSQL connection
- `Cors__ClientAddress` - Frontend URL for CORS
- JWT settings

### Frontend

Configured in `.env.development` and `.env.production`:

- `VITE_API_URL` - Backend API URL
  - Development: `http://localhost:5000/api/`
  - Production: `/api/` (relative)

- `VITE_STRIPE_PUBLISHABLE_KEY` - Stripe publishable key (client-side)
  - Format: `pk_test_...` for sandbox/test mode
  - Format: `pk_live_...` for production
  - **Required for payment checkout to work**

⚠️ **Docker Build Limitation**: The current Docker configuration does not pass `VITE_STRIPE_PUBLISHABLE_KEY` as a build argument. This means:

- **Local development** (without Docker): Works with `.env.development` file
- **Docker builds**: Client image will be built without Stripe key, payment checkout will fail
- **Future enhancement needed**: Update `client/Dockerfile` and `docker-compose.client.yml` to accept `VITE_STRIPE_PUBLISHABLE_KEY` as a build argument

For Stripe configuration details, see [STRIPE.md](STRIPE.md).

## Troubleshooting

### Port Already in Use

If ports 3000, 5000, or 5432 are already in use:

1. Stop the conflicting service
2. Or modify the port mappings in the respective docker-compose files

### Database Connection Issues

Ensure the database is fully started before the API:

```powershell
# Start database first
docker-compose up -d restoredb

# Wait a few seconds, then start API
docker-compose up -d restore.api
```

### Frontend Hot Reload Not Working

Make sure you're using `docker-compose.client.dev.yml` and the volumes are properly mounted.

### Clear Everything and Start Fresh

```powershell
# Stop and remove all containers, networks, and volumes
docker-compose down -v
docker-compose -f docker-compose.yml -f docker-compose.override.yml -f docker-compose.client.yml down -v

# Remove images
docker rmi restoreapi restore-client

# Rebuild from scratch
docker-compose -f docker-compose.yml -f docker-compose.override.yml -f docker-compose.client.yml up --build
```

## Development Tips

1. **Backend Only Development:** Use `docker-compose up` (default) and run frontend with `npm start`
2. **Frontend Only Development:** Use full stack dev mode for automatic API connection
3. **Database Changes:** Recreate the database volume with `docker-compose down -v` then `up` again
4. **Production Build Testing:** Always test with production compose before deploying

## Next Steps

- See [client/README_DOCKER.md](client/README_DOCKER.md) for frontend Docker details
- See [README_SERVER.md](README_SERVER.md) for backend API documentation
- See [README_CLIENT.md](README_CLIENT.md) for frontend development guide
