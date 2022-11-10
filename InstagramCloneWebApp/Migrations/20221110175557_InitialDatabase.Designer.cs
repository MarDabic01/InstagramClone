﻿// <auto-generated />
using System;
using InstagramCloneWebApp.DataLayer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace InstagramCloneWebApp.Migrations
{
    [DbContext(typeof(EmpDBContext))]
    [Migration("20221110175557_InitialDatabase")]
    partial class InitialDatabase
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.17")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("InstagramCloneWebApp.Entities.ImageEntity", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ImageAuthor")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("ImageDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("ImageDecsription")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("id");

                    b.ToTable("ImagesDetails");
                });
#pragma warning restore 612, 618
        }
    }
}
