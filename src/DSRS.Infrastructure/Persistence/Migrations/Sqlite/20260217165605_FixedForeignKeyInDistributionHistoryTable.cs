using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DSRS.Infrastructure.Persistence.Migrations.Sqlite
{
    /// <inheritdoc />
    public partial class FixedForeignKeyInDistributionHistoryTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DistributionHistory_Players_Id",
                table: "DistributionHistory");

            migrationBuilder.CreateIndex(
                name: "IX_DistributionHistory_PlayerId",
                table: "DistributionHistory",
                column: "PlayerId");

            migrationBuilder.AddForeignKey(
                name: "FK_DistributionHistory_Players_PlayerId",
                table: "DistributionHistory",
                column: "PlayerId",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DistributionHistory_Players_PlayerId",
                table: "DistributionHistory");

            migrationBuilder.DropIndex(
                name: "IX_DistributionHistory_PlayerId",
                table: "DistributionHistory");

            migrationBuilder.AddForeignKey(
                name: "FK_DistributionHistory_Players_Id",
                table: "DistributionHistory",
                column: "Id",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
