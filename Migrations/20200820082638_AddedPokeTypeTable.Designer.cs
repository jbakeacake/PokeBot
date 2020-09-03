﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PokeBot.Data;

namespace Pokebot.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20200820082638_AddedPokeTypeTable")]
    partial class AddedPokeTypeTable
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.0");

            modelBuilder.Entity("PokeBot.Models.MoveData", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<float>("Accuracy")
                        .HasColumnType("REAL");

                    b.Property<string>("AilmentName")
                        .HasColumnType("TEXT");

                    b.Property<float>("Effect_Chance")
                        .HasColumnType("REAL");

                    b.Property<int>("MoveId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<float>("PP")
                        .HasColumnType("REAL");

                    b.Property<float>("Power")
                        .HasColumnType("REAL");

                    b.Property<string>("StatChangeName")
                        .HasColumnType("TEXT");

                    b.Property<int>("StatChangeValue")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Target")
                        .HasColumnType("TEXT");

                    b.Property<string>("Type")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("MoveData_Tbl");
                });

            modelBuilder.Entity("PokeBot.Models.MoveLink", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("MoveId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("PokemonDataId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("PokemonDataId");

                    b.ToTable("MoveLink_Tbl");
                });

            modelBuilder.Entity("PokeBot.Models.PokeBattleData", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<Guid>("BattleTokenId")
                        .HasColumnType("TEXT");

                    b.Property<int>("UserOneId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("UserOnePokemonId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("UserTwoId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("UserTwoPokemonId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("PokeBattleData_Tbl");
                });

            modelBuilder.Entity("PokeBot.Models.PokeType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Delimited_Double_Damage_From")
                        .HasColumnType("TEXT");

                    b.Property<string>("Delimited_Double_Damage_To")
                        .HasColumnType("TEXT");

                    b.Property<string>("Delimited_Half_Damage_From")
                        .HasColumnType("TEXT");

                    b.Property<string>("Delimited_Half_Damage_To")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<int>("PokeTypeId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("PokeType_Tbl");
                });

            modelBuilder.Entity("PokeBot.Models.Pokemon", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<float>("Attack")
                        .HasColumnType("REAL");

                    b.Property<float>("Base_Experience")
                        .HasColumnType("REAL");

                    b.Property<float>("Defense")
                        .HasColumnType("REAL");

                    b.Property<float>("Experience")
                        .HasColumnType("REAL");

                    b.Property<float>("Level")
                        .HasColumnType("REAL");

                    b.Property<float>("MaxHP")
                        .HasColumnType("REAL");

                    b.Property<int>("MoveId_Four")
                        .HasColumnType("INTEGER");

                    b.Property<int>("MoveId_One")
                        .HasColumnType("INTEGER");

                    b.Property<int>("MoveId_Three")
                        .HasColumnType("INTEGER");

                    b.Property<int>("MoveId_Two")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<int>("PokeId")
                        .HasColumnType("INTEGER");

                    b.Property<float>("SpecialAttack")
                        .HasColumnType("REAL");

                    b.Property<float>("SpecialDefense")
                        .HasColumnType("REAL");

                    b.Property<float>("Speed")
                        .HasColumnType("REAL");

                    b.Property<string>("Type")
                        .HasColumnType("TEXT");

                    b.Property<int>("UserId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Pokemon_Tbl");
                });

            modelBuilder.Entity("PokeBot.Models.PokemonData", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<float>("Attack")
                        .HasColumnType("REAL");

                    b.Property<string>("BackSpriteUrl")
                        .HasColumnType("TEXT");

                    b.Property<float>("Base_Experience")
                        .HasColumnType("REAL");

                    b.Property<string>("BastionUrl")
                        .HasColumnType("TEXT");

                    b.Property<float>("Defense")
                        .HasColumnType("REAL");

                    b.Property<float>("Experience")
                        .HasColumnType("REAL");

                    b.Property<string>("FrontSpriteUrl")
                        .HasColumnType("TEXT");

                    b.Property<float>("Level")
                        .HasColumnType("REAL");

                    b.Property<float>("MaxHP")
                        .HasColumnType("REAL");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<int>("PokeId")
                        .HasColumnType("INTEGER");

                    b.Property<float>("SpecialAttack")
                        .HasColumnType("REAL");

                    b.Property<float>("SpecialDefense")
                        .HasColumnType("REAL");

                    b.Property<float>("Speed")
                        .HasColumnType("REAL");

                    b.Property<string>("Type")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("PokeData_Tbl");
                });

            modelBuilder.Entity("PokeBot.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<Guid>("BattleTokenId")
                        .HasColumnType("TEXT");

                    b.Property<ulong>("DiscordId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("Users_Tbl");
                });

            modelBuilder.Entity("PokeBot.Models.MoveLink", b =>
                {
                    b.HasOne("PokeBot.Models.PokemonData", "PokemonData")
                        .WithMany("MoveLinks")
                        .HasForeignKey("PokemonDataId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("PokeBot.Models.Pokemon", b =>
                {
                    b.HasOne("PokeBot.Models.User", "User")
                        .WithMany("PokeCollection")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
