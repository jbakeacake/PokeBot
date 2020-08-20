using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Pokebot.Migrations
{
    public partial class ReformattingPokemonTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.AddColumn<Guid>(
                name: "BattleTokenId",
                table: "Users_Tbl",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<float>(
                name: "Attack",
                table: "Pokemon_Tbl",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "Base_Experience",
                table: "Pokemon_Tbl",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "Defense",
                table: "Pokemon_Tbl",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "Experience",
                table: "Pokemon_Tbl",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "Level",
                table: "Pokemon_Tbl",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "MaxHP",
                table: "Pokemon_Tbl",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<int>(
                name: "MoveId_Four",
                table: "Pokemon_Tbl",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MoveId_One",
                table: "Pokemon_Tbl",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MoveId_Three",
                table: "Pokemon_Tbl",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MoveId_Two",
                table: "Pokemon_Tbl",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<float>(
                name: "SpecialAttack",
                table: "Pokemon_Tbl",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "SpecialDefense",
                table: "Pokemon_Tbl",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "Speed",
                table: "Pokemon_Tbl",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Pokemon_Tbl",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BastionUrl",
                table: "PokeData_Tbl",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BattleTokenId",
                table: "Users_Tbl");

            migrationBuilder.DropColumn(
                name: "Attack",
                table: "Pokemon_Tbl");

            migrationBuilder.DropColumn(
                name: "Base_Experience",
                table: "Pokemon_Tbl");

            migrationBuilder.DropColumn(
                name: "Defense",
                table: "Pokemon_Tbl");

            migrationBuilder.DropColumn(
                name: "Experience",
                table: "Pokemon_Tbl");

            migrationBuilder.DropColumn(
                name: "Level",
                table: "Pokemon_Tbl");

            migrationBuilder.DropColumn(
                name: "MaxHP",
                table: "Pokemon_Tbl");

            migrationBuilder.DropColumn(
                name: "MoveId_Four",
                table: "Pokemon_Tbl");

            migrationBuilder.DropColumn(
                name: "MoveId_One",
                table: "Pokemon_Tbl");

            migrationBuilder.DropColumn(
                name: "MoveId_Three",
                table: "Pokemon_Tbl");

            migrationBuilder.DropColumn(
                name: "MoveId_Two",
                table: "Pokemon_Tbl");

            migrationBuilder.DropColumn(
                name: "SpecialAttack",
                table: "Pokemon_Tbl");

            migrationBuilder.DropColumn(
                name: "SpecialDefense",
                table: "Pokemon_Tbl");

            migrationBuilder.DropColumn(
                name: "Speed",
                table: "Pokemon_Tbl");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Pokemon_Tbl");

            migrationBuilder.DropColumn(
                name: "BastionUrl",
                table: "PokeData_Tbl");

            migrationBuilder.AddColumn<string>(
                name: "Url",
                table: "Pokemon_Tbl",
                type: "TEXT",
                nullable: true);
        }
    }
}
