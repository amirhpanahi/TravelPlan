using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TravelPlan.Migrations
{
    public partial class EditTblHotel3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "Hotels");

            migrationBuilder.RenameColumn(
                name: "Phone",
                table: "Hotels",
                newName: "AddressAndDetails");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AddressAndDetails",
                table: "Hotels",
                newName: "Phone");

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Hotels",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
