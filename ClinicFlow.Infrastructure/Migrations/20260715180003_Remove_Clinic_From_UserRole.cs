using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClinicFlow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Remove_Clinic_From_UserRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserRoles_Clinics_ClinicId",
                table: "UserRoles");

            migrationBuilder.DropIndex(
                name: "IX_UserRoles_ClinicId",
                table: "UserRoles");

            migrationBuilder.DropColumn(
                name: "ClinicId",
                table: "UserRoles");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ClinicId",
                table: "UserRoles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_ClinicId",
                table: "UserRoles",
                column: "ClinicId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserRoles_Clinics_ClinicId",
                table: "UserRoles",
                column: "ClinicId",
                principalTable: "Clinics",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
