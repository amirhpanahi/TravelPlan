using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TravelPlan.Migrations
{
    public partial class CreateAdsTbl : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Ads",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GifName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GifAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhotoLink = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BannerAlt = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BannerTitle = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ads", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Ads");
        }
    }
}
