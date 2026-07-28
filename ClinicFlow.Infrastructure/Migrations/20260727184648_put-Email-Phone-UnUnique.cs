using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClinicFlow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class putEmailPhoneUnUnique : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Clinics_Phone",
                table: "Clinics");

            migrationBuilder.CreateIndex(
                name: "IX_Clinics_Phone",
                table: "Clinics",
                column: "Phone");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Clinics_Phone",
                table: "Clinics");

            migrationBuilder.CreateIndex(
                name: "IX_Clinics_Phone",
                table: "Clinics",
                column: "Phone",
                unique: true);
        }
    }
}
