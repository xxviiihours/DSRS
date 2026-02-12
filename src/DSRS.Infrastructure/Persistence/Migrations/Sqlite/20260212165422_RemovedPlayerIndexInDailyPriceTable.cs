using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DSRS.Infrastructure.Persistence.Migrations.Sqlite
{
    /// <inheritdoc />
    public partial class RemovedPlayerIndexInDailyPriceTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DailyPrices_Players_PlayerId",
                table: "DailyPrices");

            migrationBuilder.DropIndex(
                name: "IX_DailyPrices_ItemId",
                table: "DailyPrices");

            migrationBuilder.DropIndex(
                name: "IX_DailyPrices_PlayerId_ItemId_Date",
                table: "DailyPrices");

            migrationBuilder.AlterColumn<Guid>(
                name: "PlayerId",
                table: "DailyPrices",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "TEXT");

            migrationBuilder.CreateIndex(
                name: "IX_DailyPrices_ItemId_Date",
                table: "DailyPrices",
                columns: new[] { "ItemId", "Date" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DailyPrices_PlayerId",
                table: "DailyPrices",
                column: "PlayerId");

            migrationBuilder.AddForeignKey(
                name: "FK_DailyPrices_Players_PlayerId",
                table: "DailyPrices",
                column: "PlayerId",
                principalTable: "Players",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DailyPrices_Players_PlayerId",
                table: "DailyPrices");

            migrationBuilder.DropIndex(
                name: "IX_DailyPrices_ItemId_Date",
                table: "DailyPrices");

            migrationBuilder.DropIndex(
                name: "IX_DailyPrices_PlayerId",
                table: "DailyPrices");

            migrationBuilder.AlterColumn<Guid>(
                name: "PlayerId",
                table: "DailyPrices",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DailyPrices_ItemId",
                table: "DailyPrices",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_DailyPrices_PlayerId_ItemId_Date",
                table: "DailyPrices",
                columns: new[] { "PlayerId", "ItemId", "Date" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_DailyPrices_Players_PlayerId",
                table: "DailyPrices",
                column: "PlayerId",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
