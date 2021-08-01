using Microsoft.EntityFrameworkCore.Migrations;

namespace PropTabTabIK.DataAccess.Migrations
{
    public partial class ktoklucu_823 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsConfirmed",
                table: "AllowRequests");

            migrationBuilder.AddColumn<string>(
                name: "CompanyDescription",
                table: "AllowRequests",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompanyDescription",
                table: "AllowRequests");

            migrationBuilder.AddColumn<bool>(
                name: "IsConfirmed",
                table: "AllowRequests",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
