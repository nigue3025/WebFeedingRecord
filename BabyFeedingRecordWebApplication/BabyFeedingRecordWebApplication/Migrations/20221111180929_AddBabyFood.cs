using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BabyFeedingRecordWebApplication.Migrations
{
    public partial class AddBabyFood : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FeedingDate",
                table: "FeedingStatistics",
                newName: "FeedingTime");

            migrationBuilder.AddColumn<double>(
                name: "FormularMilkPercentage",
                table: "FeedingStatistics",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "MotherMilkPercentage",
                table: "FeedingStatistics",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "BabyFoodVolume",
                table: "FeedingRecord",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "MovingAvg",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Mnum = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovingAvg", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MovingAvg");

            migrationBuilder.DropColumn(
                name: "FormularMilkPercentage",
                table: "FeedingStatistics");

            migrationBuilder.DropColumn(
                name: "MotherMilkPercentage",
                table: "FeedingStatistics");

            migrationBuilder.DropColumn(
                name: "BabyFoodVolume",
                table: "FeedingRecord");

            migrationBuilder.RenameColumn(
                name: "FeedingTime",
                table: "FeedingStatistics",
                newName: "FeedingDate");
        }
    }
}
