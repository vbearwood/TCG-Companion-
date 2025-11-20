using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TCG_COMPANION.Migrations.Collections
{
    /// <inheritdoc />
    public partial class AddCollectionsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CardData_Collections_CollectionsId",
                table: "CardData");

            migrationBuilder.DropForeignKey(
                name: "FK_Collections_User_UserId",
                table: "Collections");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropIndex(
                name: "IX_Collections_UserId",
                table: "Collections");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CardData",
                table: "CardData");

            migrationBuilder.DropIndex(
                name: "IX_CardData_CollectionsId",
                table: "CardData");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Collections");

            migrationBuilder.DropColumn(
                name: "CollectionsId",
                table: "CardData");

            migrationBuilder.RenameTable(
                name: "CardData",
                newName: "Cards");

            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "Collections",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Cards",
                table: "Cards",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "CollectionCards",
                columns: table => new
                {
                    CardsId = table.Column<int>(type: "INTEGER", nullable: false),
                    CollectionsId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CollectionCards", x => new { x.CardsId, x.CollectionsId });
                    table.ForeignKey(
                        name: "FK_CollectionCards_Cards_CardsId",
                        column: x => x.CardsId,
                        principalTable: "Cards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CollectionCards_Collections_CollectionsId",
                        column: x => x.CollectionsId,
                        principalTable: "Collections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CollectionCards_CollectionsId",
                table: "CollectionCards",
                column: "CollectionsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CollectionCards");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Cards",
                table: "Cards");

            migrationBuilder.DropColumn(
                name: "UserName",
                table: "Collections");

            migrationBuilder.RenameTable(
                name: "Cards",
                newName: "CardData");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Collections",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CollectionsId",
                table: "CardData",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_CardData",
                table: "CardData",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Bio = table.Column<string>(type: "TEXT", nullable: true),
                    DisplayName = table.Column<string>(type: "TEXT", nullable: true),
                    Email = table.Column<string>(type: "TEXT", nullable: true),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: false),
                    ProfileImage = table.Column<string>(type: "TEXT", nullable: true),
                    Username = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Collections_UserId",
                table: "Collections",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_CardData_CollectionsId",
                table: "CardData",
                column: "CollectionsId");

            migrationBuilder.AddForeignKey(
                name: "FK_CardData_Collections_CollectionsId",
                table: "CardData",
                column: "CollectionsId",
                principalTable: "Collections",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Collections_User_UserId",
                table: "Collections",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
