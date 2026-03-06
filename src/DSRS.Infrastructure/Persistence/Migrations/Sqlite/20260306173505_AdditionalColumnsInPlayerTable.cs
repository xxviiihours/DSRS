using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DSRS.Infrastructure.Persistence.Migrations.Sqlite
{
    /// <inheritdoc />
    public partial class AdditionalColumnsInPlayerTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateOnly>(
                name: "LastLimitGeneration",
                table: "Players",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<int>(
                name: "PurchaseLimit",
                table: "Players",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastLimitGeneration",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "PurchaseLimit",
                table: "Players");
        }
    }
}
