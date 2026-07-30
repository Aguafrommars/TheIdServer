using Microsoft.EntityFrameworkCore.Migrations;
using System;

#nullable disable

namespace Aguacongas.TheIdServer.Sqlite.Migrations.ConfigurationDb
{
    /// <inheritdoc />
    public partial class DPopSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<TimeSpan>(
                name: "DPoPClockSkew",
                table: "Clients",
                type: "TEXT",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 5, 0, 0));

            migrationBuilder.AddColumn<int>(
                name: "DPoPValidationMode",
                table: "Clients",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Cultures",
                keyColumn: "Id",
                keyValue: "en",
                column: "CreatedAt",
                value: new DateTime(2026, 6, 3, 14, 21, 0, 786, DateTimeKind.Utc).AddTicks(583));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DPoPClockSkew",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "DPoPValidationMode",
                table: "Clients");

            migrationBuilder.UpdateData(
                table: "Cultures",
                keyColumn: "Id",
                keyValue: "en",
                column: "CreatedAt",
                value: new DateTime(2023, 11, 16, 21, 2, 54, 734, DateTimeKind.Utc).AddTicks(8256));
        }
    }
}
