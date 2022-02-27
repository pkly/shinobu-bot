﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Shinobu.Database;

namespace Shinobu.Migrations
{
    [DbContext(typeof(ShinobuDbContext))]
    [Migration("20211122172334_AddWallet")]
    partial class AddWallet
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 64)
                .HasAnnotation("ProductVersion", "5.0.8");

            modelBuilder.Entity("Shinobu.Database.Entity.Command.Block", b =>
                {
                    b.Property<ulong>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint unsigned");

                    b.Property<ulong>("Blocked")
                        .HasColumnType("bigint unsigned");

                    b.Property<ulong>("Requester")
                        .HasColumnType("bigint unsigned");

                    b.HasKey("Id");

                    b.ToTable("c__block");
                });

            modelBuilder.Entity("Shinobu.Database.Entity.Profile.Wallet", b =>
                {
                    b.Property<ulong>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint unsigned");

                    b.Property<ulong>("Points")
                        .HasColumnType("bigint unsigned");

                    b.Property<ulong>("UserId")
                        .HasColumnType("bigint unsigned");

                    b.HasKey("Id");

                    b.ToTable("p__wallet");
                });
#pragma warning restore 612, 618
        }
    }
}