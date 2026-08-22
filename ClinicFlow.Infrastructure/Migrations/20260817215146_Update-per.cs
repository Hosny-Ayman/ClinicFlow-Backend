using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClinicFlow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Updateper : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "DoctorVacations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                column: "Permissions",
                value: 1073086463L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "DoctorVacations");

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                column: "Permissions",
                value: 32899071L);
        }
    }
}
