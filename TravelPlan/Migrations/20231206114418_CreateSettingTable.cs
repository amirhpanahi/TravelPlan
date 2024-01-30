using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TravelPlan.Migrations
{
    public partial class CreateSettingTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Settings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SiteName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LogoAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FavIconAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SeoDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    KeyWords = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ScriptHeader = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TextForFooter = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FooterMenu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SocialMedia = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Permissions = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CopyrightText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ScriptFooter = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Settings", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Settings");
        }
    }
}
