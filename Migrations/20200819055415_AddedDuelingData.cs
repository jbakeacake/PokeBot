using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Pokebot.Migrations
{
    public partial class AddedDuelingData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MoveData_Tbl",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: true),
                    MoveId = table.Column<int>(nullable: false),
                    Accuracy = table.Column<float>(nullable: false),
                    Effect_Chance = table.Column<float>(nullable: false),
                    AilmentName = table.Column<string>(nullable: true),
                    Power = table.Column<float>(nullable: false),
                    PP = table.Column<float>(nullable: false),
                    Type = table.Column<string>(nullable: true),
                    StatChangeName = table.Column<string>(nullable: true),
                    StatChangeValue = table.Column<int>(nullable: false),
                    Target = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MoveData_Tbl", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PokeBattleData_Tbl",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BattleTokenId = table.Column<Guid>(nullable: false),
                    UserOneId = table.Column<int>(nullable: false),
                    UserTwoId = table.Column<int>(nullable: false),
                    UserOnePokemonId = table.Column<int>(nullable: false),
                    UserTwoPokemonId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PokeBattleData_Tbl", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PokeData_Tbl",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PokeId = table.Column<int>(nullable: false),
                    Base_Experience = table.Column<float>(nullable: false),
                    BackSpriteUrl = table.Column<string>(nullable: true),
                    FrontSpriteUrl = table.Column<string>(nullable: true),
                    MaxHP = table.Column<float>(nullable: false),
                    Level = table.Column<float>(nullable: false),
                    Experience = table.Column<float>(nullable: false),
                    Attack = table.Column<float>(nullable: false),
                    Defense = table.Column<float>(nullable: false),
                    SpecialAttack = table.Column<float>(nullable: false),
                    SpecialDefense = table.Column<float>(nullable: false),
                    Speed = table.Column<float>(nullable: false),
                    Type = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PokeData_Tbl", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MoveLink_Tbl",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MoveId = table.Column<int>(nullable: false),
                    PokemonDataId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MoveLink_Tbl", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MoveLink_Tbl_PokeData_Tbl_PokemonDataId",
                        column: x => x.PokemonDataId,
                        principalTable: "PokeData_Tbl",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MoveLink_Tbl_PokemonDataId",
                table: "MoveLink_Tbl",
                column: "PokemonDataId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MoveData_Tbl");

            migrationBuilder.DropTable(
                name: "MoveLink_Tbl");

            migrationBuilder.DropTable(
                name: "PokeBattleData_Tbl");

            migrationBuilder.DropTable(
                name: "PokeData_Tbl");
        }
    }
}
