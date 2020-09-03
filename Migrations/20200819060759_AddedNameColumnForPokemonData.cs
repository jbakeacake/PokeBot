using Microsoft.EntityFrameworkCore.Migrations;

namespace Pokebot.Migrations
{
    public partial class AddedNameColumnForPokemonData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "PokeData_Tbl",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "PokeData_Tbl");
        }
    }
}
