using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TCG_COMPANION.Migrations.Deck
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CardData_Decks_DeckId",
                table: "CardData");

            migrationBuilder.DropForeignKey(
                name: "FK_Decks_User_UserId",
                table: "Decks");

            migrationBuilder.DropIndex(
                name: "IX_Decks_UserId",
                table: "Decks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_User",
                table: "User");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CardData",
                table: "CardData");

            migrationBuilder.DropIndex(
                name: "IX_CardData_DeckId",
                table: "CardData");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Decks");

            migrationBuilder.DropColumn(
                name: "DeckId",
                table: "CardData");

            migrationBuilder.RenameTable(
                name: "User",
                newName: "Users");

            migrationBuilder.RenameTable(
                name: "CardData",
                newName: "Cards");

            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "Decks",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Users",
                table: "Users",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Cards",
                table: "Cards",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "DeckCards",
                columns: table => new
                {
                    CardsId = table.Column<int>(type: "INTEGER", nullable: false),
                    DeckId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeckCards", x => new { x.CardsId, x.DeckId });
                    table.ForeignKey(
                        name: "FK_DeckCards_Cards_CardsId",
                        column: x => x.CardsId,
                        principalTable: "Cards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DeckCards_Decks_DeckId",
                        column: x => x.DeckId,
                        principalTable: "Decks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DeckCards_DeckId",
                table: "DeckCards",
                column: "DeckId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeckCards");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Users",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Cards",
                table: "Cards");

            migrationBuilder.DropColumn(
                name: "UserName",
                table: "Decks");

            migrationBuilder.RenameTable(
                name: "Users",
                newName: "User");

            migrationBuilder.RenameTable(
                name: "Cards",
                newName: "CardData");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Decks",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DeckId",
                table: "CardData",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_User",
                table: "User",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CardData",
                table: "CardData",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Decks_UserId",
                table: "Decks",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_CardData_DeckId",
                table: "CardData",
                column: "DeckId");

            migrationBuilder.AddForeignKey(
                name: "FK_CardData_Decks_DeckId",
                table: "CardData",
                column: "DeckId",
                principalTable: "Decks",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Decks_User_UserId",
                table: "Decks",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
