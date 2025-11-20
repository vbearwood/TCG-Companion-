using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TCG_COMPANION.Migrations.Collections
{
    /// <inheritdoc />
    public partial class AddPricesColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Prices",
                table: "Cards",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Prices",
                table: "Cards");
        }
    }
}
