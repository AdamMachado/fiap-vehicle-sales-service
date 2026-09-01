using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fiap.VehicleSales.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPendingPaymentFlow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Sales_VehicleId",
                table: "Sales");

            migrationBuilder.AddColumn<string>(
                name: "BuyerCpf",
                table: "Sales",
                type: "character varying(11)",
                maxLength: 11,
                nullable: false,
                defaultValue: "00000000000");

            migrationBuilder.AddColumn<string>(
                name: "PaymentCode",
                table: "Sales",
                type: "character varying(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PaymentProcessedAt",
                table: "Sales",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.Sql(
                "UPDATE \"Sales\" SET \"PaymentCode\" = 'legacy-' || \"Id\"::text;");

            migrationBuilder.AlterColumn<string>(
                name: "PaymentCode",
                table: "Sales",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(64)",
                oldMaxLength: 64,
                oldNullable: true);

            migrationBuilder.Sql(
                "UPDATE \"Sales\" SET \"Status\" = CASE \"Status\" WHEN 1 THEN 2 WHEN 2 THEN 3 ELSE \"Status\" END;");

            migrationBuilder.Sql(
                "UPDATE \"Vehicles\" SET \"Status\" = 3 WHERE \"Status\" = 2;");

            migrationBuilder.CreateIndex(
                name: "IX_Sales_BuyerCpf",
                table: "Sales",
                column: "BuyerCpf");

            migrationBuilder.CreateIndex(
                name: "IX_Sales_PaymentCode",
                table: "Sales",
                column: "PaymentCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sales_VehicleId",
                table: "Sales",
                column: "VehicleId",
                unique: true,
                filter: "\"Status\" = 1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Sales_BuyerCpf",
                table: "Sales");

            migrationBuilder.DropIndex(
                name: "IX_Sales_PaymentCode",
                table: "Sales");

            migrationBuilder.DropIndex(
                name: "IX_Sales_VehicleId",
                table: "Sales");

            migrationBuilder.Sql(
                "UPDATE \"Sales\" SET \"Status\" = CASE \"Status\" WHEN 2 THEN 1 WHEN 3 THEN 2 ELSE 1 END;");

            migrationBuilder.Sql(
                "UPDATE \"Vehicles\" SET \"Status\" = CASE \"Status\" WHEN 3 THEN 2 WHEN 2 THEN 1 ELSE \"Status\" END;");

            migrationBuilder.DropColumn(
                name: "BuyerCpf",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "PaymentCode",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "PaymentProcessedAt",
                table: "Sales");

            migrationBuilder.CreateIndex(
                name: "IX_Sales_VehicleId",
                table: "Sales",
                column: "VehicleId",
                unique: true);
        }
    }
}
