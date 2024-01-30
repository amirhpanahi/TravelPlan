using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TravelPlan.Migrations
{
    public partial class addTblTrip : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Trips",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddressAndDetails = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CountryId = table.Column<int>(type: "int", nullable: false),
                    Slug = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CityId = table.Column<int>(type: "int", nullable: false),
                    IndexImage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImageAlt = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImageTitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VideoAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WriterId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsSelected = table.Column<bool>(type: "bit", nullable: false),
                    TripStatus = table.Column<int>(type: "int", nullable: true),
                    TripDuration = table.Column<int>(type: "int", nullable: true),
                    RejectDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RejectDatePersian = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeleteDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeleteDatePersian = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PublishDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PublishDatePersian = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RegisterDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RegisterDatePersian = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TripSummary = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    KeyWords = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Tags = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trips", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Trips");
        }
    }
}
