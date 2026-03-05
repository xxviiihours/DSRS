using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DSRS.Infrastructure.Persistence.Migrations.Sqlite
{
    /// <inheritdoc />
    public partial class AddedMaxLimitPlayerTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MaxLimit",
                table: "Players",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaxLimit",
                table: "Players");
        }
    }
}
