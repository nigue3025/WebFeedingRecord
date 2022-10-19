using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BabyFeedingRecordWebApplication.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FeedingRecord",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FeedingDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FeedingTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MotherMilkVolume = table.Column<int>(type: "int", nullable: false),
                    FormularMilkVolume = table.Column<int>(type: "int", nullable: false),
                    Memo = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeedingRecord", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FeedingRecord");
        }
    }
}
