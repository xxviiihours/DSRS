using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DSRS.Infrastructure.Persistence.Migrations.Sqlite
{
    /// <inheritdoc />
    public partial class UpdateDistributionHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "InventoryId",
                table: "DistributionHistory",
                newName: "PlayerId");

            migrationBuilder.AddColumn<Guid>(
                name: "DailyPriceId",
                table: "DistributionHistory",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddForeignKey(
                name: "FK_DistributionHistory_Players_Id",
                table: "DistributionHistory",
                column: "Id",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DistributionHistory_Players_Id",
                table: "DistributionHistory");

            migrationBuilder.DropColumn(
                name: "DailyPriceId",
                table: "DistributionHistory");

            migrationBuilder.RenameColumn(
                name: "PlayerId",
                table: "DistributionHistory",
                newName: "InventoryId");
        }
    }
}
