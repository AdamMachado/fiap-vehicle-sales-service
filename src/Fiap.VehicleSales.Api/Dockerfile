FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["Fiap.VehicleSales.Service.slnx", "./"]

COPY ["src/Fiap.VehicleSales.Api/Fiap.VehicleSales.Api.csproj", "src/Fiap.VehicleSales.Api/"]
COPY ["src/Fiap.VehicleSales.Application/Fiap.VehicleSales.Application.csproj", "src/Fiap.VehicleSales.Application/"]
COPY ["src/Fiap.VehicleSales.Domain/Fiap.VehicleSales.Domain.csproj", "src/Fiap.VehicleSales.Domain/"]
COPY ["src/Fiap.VehicleSales.Infrastructure/Fiap.VehicleSales.Infrastructure.csproj", "src/Fiap.VehicleSales.Infrastructure/"]

RUN dotnet restore "src/Fiap.VehicleSales.Api/Fiap.VehicleSales.Api.csproj"

COPY . .

WORKDIR "/src/src/Fiap.VehicleSales.Api"

RUN dotnet publish "Fiap.VehicleSales.Api.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/publish \
    /p:UseAppHost=false

FROM base AS final
WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "Fiap.VehicleSales.Api.dll"]
