using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TravelPlan.Migrations
{
    public partial class editTblResturant1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "HotelSummary",
                table: "Restaurants",
                newName: "RestaurantSummary");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RestaurantSummary",
                table: "Restaurants",
                newName: "HotelSummary");
        }
    }
}
