using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DSRS.Infrastructure.Persistence.Migrations.Sqlite
{
    /// <inheritdoc />
    public partial class ApplyPendingChangesToSQLiteDbContext : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DailyPrices_PlayerId",
                table: "DailyPrices");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Items",
                type: "TEXT",
                maxLength: 150,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateOnly>(
                name: "Date",
                table: "DailyPrices",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.CreateIndex(
                name: "IX_DailyPrices_PlayerId_ItemId_Date",
                table: "DailyPrices",
                columns: new[] { "PlayerId", "ItemId", "Date" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DailyPrices_PlayerId_ItemId_Date",
                table: "DailyPrices");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "Date",
                table: "DailyPrices");

            migrationBuilder.CreateIndex(
                name: "IX_DailyPrices_PlayerId",
                table: "DailyPrices",
                column: "PlayerId");
        }
    }
}
