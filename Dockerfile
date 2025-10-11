# ==================================
# Stage 1: Build
# ==================================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution file
COPY ["WashBooking.sln", "./"]

# Copy all project files for better layer caching
COPY ["WashBooking/WashBooking.csproj", "WashBooking/"]
COPY ["WashBooking.Application/WashBooking.Application.csproj", "WashBooking.Application/"]
COPY ["WashBooking.Domain/WashBooking.Domain.csproj", "WashBooking.Domain/"]
COPY ["WashBooking.Infrastructure/WashBooking.Infrastructure.csproj", "WashBooking.Infrastructure/"]

# Restore all projects (solution-level restore for better dependency resolution)
RUN dotnet restore "WashBooking.sln"

# Copy all source code
COPY . .

# Build and publish in one step (more efficient)
WORKDIR "/src/WashBooking"
RUN dotnet publish "WashBooking.csproj" \
    -c Release \
    -o /app/publish \
    --no-restore \
    /p:UseAppHost=false \
    /p:DebugType=None \
    /p:DebugSymbols=false

# ==================================
# Stage 2: Runtime
# ==================================
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS final

# Install necessary dependencies for PostgreSQL and timezone data
RUN apk add --no-cache \
    icu-libs \
    tzdata

# Create non-root user for security
RUN addgroup -g 1000 appuser && \
    adduser -D -u 1000 -G appuser appuser

WORKDIR /app

# Copy published files
COPY --from=build /app/publish .

# Change ownership to non-root user
RUN chown -R appuser:appuser /app

# Switch to non-root user
USER appuser

# Set environment variables for production
ENV ASPNETCORE_ENVIRONMENT=Production \
    ASPNETCORE_URLS=http://+:8080 \
    DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false \
    DOTNET_EnableDiagnostics=0 \
    DOTNET_CLI_TELEMETRY_OPTOUT=1

# Expose port (Render will use this)
EXPOSE 8080

# Health check for Render to monitor app status
HEALTHCHECK --interval=30s --timeout=3s --start-period=10s --retries=3 \
    CMD wget --no-verbose --tries=1 --spider http://localhost:8080/swagger/index.html || exit 1

# Set entry point
ENTRYPOINT ["dotnet", "WashBooking.dll"]
