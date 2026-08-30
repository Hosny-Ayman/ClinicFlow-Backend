using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClinicFlow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateInvoice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_Clinics_ClinicId",
                table: "Invoices");

            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_Patients_PatientId",
                table: "Invoices");

            migrationBuilder.DropIndex(
                name: "IX_Invoices_ClinicId",
                table: "Invoices");

            migrationBuilder.DropIndex(
                name: "IX_Invoices_ClinicId_Status",
                table: "Invoices");

            migrationBuilder.DropIndex(
                name: "IX_Invoices_PatientId",
                table: "Invoices");

            migrationBuilder.DropIndex(
                name: "IX_Invoices_PatientId_IssuedAt",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "ClinicId",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "PatientId",
                table: "Invoices");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ClinicId",
                table: "Invoices",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PatientId",
                table: "Invoices",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_ClinicId",
                table: "Invoices",
                column: "ClinicId");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_ClinicId_Status",
                table: "Invoices",
                columns: new[] { "ClinicId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_PatientId",
                table: "Invoices",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_PatientId_IssuedAt",
                table: "Invoices",
                columns: new[] { "PatientId", "IssuedAt" });

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_Clinics_ClinicId",
                table: "Invoices",
                column: "ClinicId",
                principalTable: "Clinics",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_Patients_PatientId",
                table: "Invoices",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
