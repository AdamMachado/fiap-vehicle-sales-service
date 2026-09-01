using Fiap.VehicleSales.Domain.Entities;
using Fiap.VehicleSales.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fiap.VehicleSales.Infrastructure.Persistence.Configurations;

public sealed class VehicleConfiguration : IEntityTypeConfiguration<Vehicle>
{
    public void Configure(EntityTypeBuilder<Vehicle> builder)
    {
        builder.ToTable("Vehicles");

        builder.HasKey(vehicle => vehicle.Id);

        builder.Property(vehicle => vehicle.Brand)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(vehicle => vehicle.Model)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(vehicle => vehicle.Year)
            .IsRequired();

        builder.Property(vehicle => vehicle.Color)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(vehicle => vehicle.Price)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(vehicle => vehicle.Status)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(vehicle => vehicle.CreatedAt)
            .IsRequired();

        builder.Property(vehicle => vehicle.UpdatedAt);

        builder.HasIndex(vehicle => vehicle.Status);

        builder.HasIndex(vehicle => vehicle.Price);
    }
}