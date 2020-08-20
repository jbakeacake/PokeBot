using Microsoft.EntityFrameworkCore.Migrations;

namespace Pokebot.Migrations
{
    public partial class AddedPokeTypeTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PokeType_Tbl",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PokeTypeId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Delimited_Double_Damage_From = table.Column<string>(nullable: true),
                    Delimited_Double_Damage_To = table.Column<string>(nullable: true),
                    Delimited_Half_Damage_From = table.Column<string>(nullable: true),
                    Delimited_Half_Damage_To = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PokeType_Tbl", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PokeType_Tbl");
        }
    }
}
