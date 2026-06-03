using Microsoft.EntityFrameworkCore.Migrations;
using System;

#nullable disable

namespace Aguacongas.TheIdServer.PostgreSQL.Migrations.ConfigurationDb
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
                type: "interval",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 5, 0, 0));

            migrationBuilder.AddColumn<int>(
                name: "DPoPValidationMode",
                table: "Clients",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Cultures",
                keyColumn: "Id",
                keyValue: "en",
                column: "CreatedAt",
                value: new DateTime(2026, 6, 3, 14, 20, 5, 633, DateTimeKind.Utc).AddTicks(5184));
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
                value: new DateTime(2023, 11, 16, 21, 2, 44, 684, DateTimeKind.Utc).AddTicks(8194));
        }
    }
}
