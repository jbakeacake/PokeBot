using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Pokebot.Migrations
{
    public partial class AddedExpirationDateToPokeBattles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ExpirationDate",
                table: "PokeBattleData_Tbl",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExpirationDate",
                table: "PokeBattleData_Tbl");
        }
    }
}
