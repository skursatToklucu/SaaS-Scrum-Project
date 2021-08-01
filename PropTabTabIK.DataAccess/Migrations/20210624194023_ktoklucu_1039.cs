using Microsoft.EntityFrameworkCore.Migrations;

namespace PropTabTabIK.DataAccess.Migrations
{
    public partial class ktoklucu_1039 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsConfirmed",
                table: "AdvancePayments");

            migrationBuilder.AddColumn<double>(
                name: "AdvancePaymentRight",
                table: "Employees",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Salary",
                table: "Employees",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "AdvancePaymentType",
                table: "AdvancePayments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "CompanyDescription",
                table: "AdvancePayments",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdvancePaymentRight",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "Salary",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "AdvancePaymentType",
                table: "AdvancePayments");

            migrationBuilder.DropColumn(
                name: "CompanyDescription",
                table: "AdvancePayments");

            migrationBuilder.AddColumn<bool>(
                name: "IsConfirmed",
                table: "AdvancePayments",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
