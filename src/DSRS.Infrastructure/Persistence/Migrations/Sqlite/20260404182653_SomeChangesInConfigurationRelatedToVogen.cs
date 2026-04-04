using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DSRS.Infrastructure.Persistence.Migrations.Sqlite
{
    /// <inheritdoc />
    public partial class SomeChangesInConfigurationRelatedToVogen : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Friendships_PlayerA_PlayerB",
                table: "Friendships");

            migrationBuilder.DropColumn(
                name: "PlayerA",
                table: "Friendships");

            migrationBuilder.RenameColumn(
                name: "PlayerB",
                table: "Friendships",
                newName: "PlayerPair");

            migrationBuilder.RenameIndex(
                name: "IX_Friendships_RequesterId",
                table: "Friendships",
                newName: "IX_Friendships_Requester");

            migrationBuilder.RenameIndex(
                name: "IX_Friendships_AddresseeId",
                table: "Friendships",
                newName: "IX_Friendships_Addressee");

            migrationBuilder.AlterColumn<decimal>(
                name: "PriceTotal",
                table: "DistributionHistory",
                type: "TEXT",
                precision: 18,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "DailyPrices",
                type: "TEXT",
                precision: 18,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.CreateIndex(
                name: "IX_Friendships_PairStatus",
                table: "Friendships",
                columns: new[] { "PlayerPair", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Friendships_UniquePair",
                table: "Friendships",
                column: "PlayerPair",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Friendships_PairStatus",
                table: "Friendships");

            migrationBuilder.DropIndex(
                name: "IX_Friendships_UniquePair",
                table: "Friendships");

            migrationBuilder.RenameColumn(
                name: "PlayerPair",
                table: "Friendships",
                newName: "PlayerB");

            migrationBuilder.RenameIndex(
                name: "IX_Friendships_Requester",
                table: "Friendships",
                newName: "IX_Friendships_RequesterId");

            migrationBuilder.RenameIndex(
                name: "IX_Friendships_Addressee",
                table: "Friendships",
                newName: "IX_Friendships_AddresseeId");

            migrationBuilder.AddColumn<Guid>(
                name: "PlayerA",
                table: "Friendships",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<decimal>(
                name: "PriceTotal",
                table: "DistributionHistory",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "TEXT",
                oldPrecision: 18,
                oldScale: 2);

            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "DailyPrices",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "TEXT",
                oldPrecision: 18,
                oldScale: 2);

            migrationBuilder.CreateIndex(
                name: "IX_Friendships_PlayerA_PlayerB",
                table: "Friendships",
                columns: new[] { "PlayerA", "PlayerB" },
                unique: true);
        }
    }
}
