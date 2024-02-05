#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
# USER app
WORKDIR /app
EXPOSE 5000
# EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["server/Services/Restore/Restore.API/Restore.API.csproj", "server/Services/Restore/Restore.API/"]
COPY ["server/Services/Restore/Restore.Application/Restore.Application.csproj", "server/Services/Restore/Restore.Application/"]
COPY ["server/Services/Restore/Restore.Core/Restore.Core.csproj", "server/Services/Restore/Restore.Core/"]
COPY ["server/Services/Restore/Restore.Infrastructure/Restore.Infrastructure.csproj", "server/Services/Restore/Restore.Infrastructure/"]
RUN dotnet restore "./server/Services/Restore/Restore.API/./Restore.API.csproj"
COPY . .
WORKDIR "/src/server/Services/Restore/Restore.API"
RUN dotnet build "./Restore.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Restore.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Restore.API.dll"]