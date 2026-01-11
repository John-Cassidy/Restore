# Multi-stage Dockerfile for React + Vite application
# Production build - serves static files with Node.js

FROM node:20-alpine AS base
WORKDIR /app

# Install dependencies stage
FROM base AS deps
COPY package.json package-lock.json* ./
RUN npm ci

# Build stage
FROM base AS build
COPY --from=deps /app/node_modules ./node_modules
COPY . .

# Set environment variable for build
ENV VITE_API_URL=http://localhost:5000/api/

# Build the application for production (override outDir for Docker)
RUN npm run build -- --outDir dist

# Production stage - serve with Vite preview server
FROM node:20-alpine AS production
WORKDIR /app

# Copy package files
COPY package.json package-lock.json* ./

# Install vite globally for preview command
RUN npm install -g vite

# Copy built assets from build stage
COPY --from=build /app/dist ./dist

# Expose Vite preview port
EXPOSE 3000

# Start Vite preview server
CMD ["vite", "preview", "--host", "0.0.0.0", "--port", "3000"]
