using Fiap.VehicleSales.Application.Interfaces;
using Fiap.VehicleSales.Infrastructure.Persistence;
using Fiap.VehicleSales.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Fiap.VehicleSales.Infrastructure.DependencyInjection;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("VehicleSalesDb");

        services.AddDbContext<VehicleSalesDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<IVehicleRepository, VehicleRepository>();
        services.AddScoped<ISaleRepository, SaleRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}