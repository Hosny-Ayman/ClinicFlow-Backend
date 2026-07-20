using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClinicFlow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SysteamSetting_SettingKey_Unique : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SysteamSettings_SettingKey",
                table: "SysteamSettings");

            migrationBuilder.CreateIndex(
                name: "IX_SysteamSettings_SettingKey",
                table: "SysteamSettings",
                column: "SettingKey",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SysteamSettings_SettingKey",
                table: "SysteamSettings");

            migrationBuilder.CreateIndex(
                name: "IX_SysteamSettings_SettingKey",
                table: "SysteamSettings",
                column: "SettingKey");
        }
    }
}
