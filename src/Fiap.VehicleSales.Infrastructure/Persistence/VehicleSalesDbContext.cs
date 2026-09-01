using Fiap.VehicleSales.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace Fiap.VehicleSales.Infrastructure.Persistence;

public sealed class VehicleSalesDbContext : DbContext
{
    public VehicleSalesDbContext(DbContextOptions<VehicleSalesDbContext> options)
        : base(options)
    {
    }

    public DbSet<Vehicle> Vehicles => Set<Vehicle>();
    public DbSet<Sale> Sales => Set<Sale>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(VehicleSalesDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}