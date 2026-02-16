using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DSRS.Infrastructure.Persistence.Migrations.Sqlite
{
    /// <inheritdoc />
    public partial class RemovedDistributionTypeAndPriceTotal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DistributionType",
                table: "Inventories");

            migrationBuilder.DropColumn(
                name: "PriceTotal",
                table: "Inventories");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DistributionType",
                table: "Inventories",
                type: "TEXT",
                maxLength: 5,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "PriceTotal",
                table: "Inventories",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
