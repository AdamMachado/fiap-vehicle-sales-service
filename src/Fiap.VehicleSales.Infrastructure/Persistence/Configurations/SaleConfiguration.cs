using Fiap.VehicleSales.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fiap.VehicleSales.Infrastructure.Persistence.Configurations;

public sealed class SaleConfiguration : IEntityTypeConfiguration<Sale>
{
    public void Configure(EntityTypeBuilder<Sale> builder)
    {
        builder.ToTable("Sales");

        builder.HasKey(sale => sale.Id);

        builder.Property(sale => sale.VehicleId)
            .IsRequired();

        builder.Property(sale => sale.BuyerId)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(sale => sale.Price)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(sale => sale.SaleDate)
            .IsRequired();

        builder.Property(sale => sale.Status)
            .IsRequired()
            .HasConversion<int>();

        builder.HasIndex(sale => sale.VehicleId)
            .IsUnique();

        builder.HasIndex(sale => sale.BuyerId);
    }
}