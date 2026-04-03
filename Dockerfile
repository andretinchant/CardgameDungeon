# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:10.0-preview AS build
WORKDIR /src

# Copy solution and project files for layer caching
COPY CardgameDungeon.slnx .
COPY src/CardgameDungeon.Domain/CardgameDungeon.Domain.csproj src/CardgameDungeon.Domain/
COPY src/CardgameDungeon.Features/CardgameDungeon.Features.csproj src/CardgameDungeon.Features/
COPY src/CardgameDungeon.Infrastructure/CardgameDungeon.Infrastructure.csproj src/CardgameDungeon.Infrastructure/
COPY src/CardgameDungeon.API/CardgameDungeon.API.csproj src/CardgameDungeon.API/

RUN dotnet restore CardgameDungeon.slnx

# Copy source and publish
COPY src/ src/
RUN dotnet publish src/CardgameDungeon.API/CardgameDungeon.API.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0-preview AS runtime
WORKDIR /app

RUN adduser --disabled-password --gecos "" appuser
USER appuser

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

HEALTHCHECK --interval=15s --timeout=5s --start-period=10s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "CardgameDungeon.API.dll"]
