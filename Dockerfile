# ==================================
# Stage 1: Build
# ==================================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution file
COPY ["WashBooking.sln", "./"]

# Copy all project files
COPY ["WashBooking/WashBooking.csproj", "WashBooking/"]
COPY ["WashBooking.Application/WashBooking.Application.csproj", "WashBooking.Application/"]
COPY ["WashBooking.Domain/WashBooking.Domain.csproj", "WashBooking.Domain/"]
COPY ["WashBooking.Infrastructure/WashBooking.Infrastructure.csproj", "WashBooking.Infrastructure/"]

# Restore dependencies
RUN dotnet restore "WashBooking/WashBooking.csproj"

# Copy all source code
COPY . .

# Build the project
WORKDIR "/src/WashBooking"
RUN dotnet build "WashBooking.csproj" -c Release -o /app/build

# ==================================
# Stage 2: Publish
# ==================================
FROM build AS publish
RUN dotnet publish "WashBooking.csproj" -c Release -o /app/publish /p:UseAppHost=false

# ==================================
# Stage 3: Runtime
# ==================================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Set environment variables
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:8080

# Expose port
EXPOSE 8080

# Copy published files
COPY --from=publish /app/publish .

# Set entry point
ENTRYPOINT ["dotnet", "WashBooking.dll"]
