using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BabyFeedingRecordWebApplication.Migrations
{
    public partial class mssqllocal_migration_569 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FeedingStatistics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FeedingDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MotherMilkTotal = table.Column<int>(type: "int", nullable: false),
                    FormularMilkTotal = table.Column<int>(type: "int", nullable: false),
                    Total = table.Column<int>(type: "int", nullable: false),
                    TimeIntervalAvg = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeedingStatistics", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FeedingStatistics");
        }
    }
}
