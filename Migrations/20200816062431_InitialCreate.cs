using Microsoft.EntityFrameworkCore.Migrations;

namespace Pokebot.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users_Tbl",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DiscordId = table.Column<ulong>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users_Tbl", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Pokemon_Tbl",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PokeId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Url = table.Column<string>(nullable: true),
                    UserId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pokemon_Tbl", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Pokemon_Tbl_Users_Tbl_UserId",
                        column: x => x.UserId,
                        principalTable: "Users_Tbl",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Pokemon_Tbl_UserId",
                table: "Pokemon_Tbl",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Pokemon_Tbl");

            migrationBuilder.DropTable(
                name: "Users_Tbl");
        }
    }
}
