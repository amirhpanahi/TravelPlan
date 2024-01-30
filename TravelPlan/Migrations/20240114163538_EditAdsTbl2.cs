using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TravelPlan.Migrations
{
    public partial class EditAdsTbl2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "BannerTitle",
                table: "Ads",
                newName: "GifTitle");

            migrationBuilder.RenameColumn(
                name: "BannerAlt",
                table: "Ads",
                newName: "GifAlt");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "GifTitle",
                table: "Ads",
                newName: "BannerTitle");

            migrationBuilder.RenameColumn(
                name: "GifAlt",
                table: "Ads",
                newName: "BannerAlt");
        }
    }
}
