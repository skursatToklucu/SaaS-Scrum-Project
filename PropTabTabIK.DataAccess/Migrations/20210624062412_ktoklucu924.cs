using Microsoft.EntityFrameworkCore.Migrations;

namespace PropTabTabIK.DataAccess.Migrations
{
    public partial class ktoklucu924 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RemainingAllowTime",
                table: "AllowRequests",
                newName: "AllowTime");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AllowTime",
                table: "AllowRequests",
                newName: "RemainingAllowTime");
        }
    }
}
