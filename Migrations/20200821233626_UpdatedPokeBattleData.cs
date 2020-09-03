using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Pokebot.Migrations
{
    public partial class UpdatedPokeBattleData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ExpirationDate",
                table: "PokeBattleData_Tbl",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "UserOnePokemonId",
                table: "PokeBattleData_Tbl",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UserTwoPokemonId",
                table: "PokeBattleData_Tbl",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }
    }
}
