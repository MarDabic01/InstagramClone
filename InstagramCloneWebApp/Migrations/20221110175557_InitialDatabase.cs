using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace InstagramCloneWebApp.Migrations
{
    public partial class InitialDatabase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ImagesDetails",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ImageDecsription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImageAuthor = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImageDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImagesDetails", x => x.id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ImagesDetails");
        }
    }
}
