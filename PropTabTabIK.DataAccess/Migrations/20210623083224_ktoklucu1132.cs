using Microsoft.EntityFrameworkCore.Migrations;

namespace PropTabTabIK.DataAccess.Migrations
{
    public partial class ktoklucu1132 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte>(
                name: "AllowCount",
                table: "Employees",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<int>(
                name: "AllowType",
                table: "AllowRequests",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<byte>(
                name: "TotalAllowTime",
                table: "AllowRequests",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AllowCount",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "AllowType",
                table: "AllowRequests");

            migrationBuilder.DropColumn(
                name: "TotalAllowTime",
                table: "AllowRequests");
        }
    }
}
