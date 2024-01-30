using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TravelPlan.Migrations
{
    public partial class EditTBLHotel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Phone",
                table: "Hotels",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeleteDate",
                table: "Hotels",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeleteDatePersian",
                table: "Hotels",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HotelSummary",
                table: "Hotels",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsSelected",
                table: "Hotels",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "PublishDate",
                table: "Hotels",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PublishDatePersian",
                table: "Hotels",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RegisterDate",
                table: "Hotels",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RegisterDatePersian",
                table: "Hotels",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RejectDate",
                table: "Hotels",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RejectDatePersian",
                table: "Hotels",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "Hotels",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Hotels",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WriterId",
                table: "Hotels",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeleteDate",
                table: "Hotels");

            migrationBuilder.DropColumn(
                name: "DeleteDatePersian",
                table: "Hotels");

            migrationBuilder.DropColumn(
                name: "HotelSummary",
                table: "Hotels");

            migrationBuilder.DropColumn(
                name: "IsSelected",
                table: "Hotels");

            migrationBuilder.DropColumn(
                name: "PublishDate",
                table: "Hotels");

            migrationBuilder.DropColumn(
                name: "PublishDatePersian",
                table: "Hotels");

            migrationBuilder.DropColumn(
                name: "RegisterDate",
                table: "Hotels");

            migrationBuilder.DropColumn(
                name: "RegisterDatePersian",
                table: "Hotels");

            migrationBuilder.DropColumn(
                name: "RejectDate",
                table: "Hotels");

            migrationBuilder.DropColumn(
                name: "RejectDatePersian",
                table: "Hotels");

            migrationBuilder.DropColumn(
                name: "Slug",
                table: "Hotels");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Hotels");

            migrationBuilder.DropColumn(
                name: "WriterId",
                table: "Hotels");

            migrationBuilder.AlterColumn<int>(
                name: "Phone",
                table: "Hotels",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
