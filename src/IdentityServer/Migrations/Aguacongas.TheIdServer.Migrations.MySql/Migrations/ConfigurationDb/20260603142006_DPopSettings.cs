using Microsoft.EntityFrameworkCore.Migrations;
using System;

#nullable disable

namespace Aguacongas.TheIdServer.MySql.Migrations.ConfigurationDb
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
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 5, 0, 0));

            migrationBuilder.AddColumn<int>(
                name: "DPoPValidationMode",
                table: "Clients",
                type: "int",
                nullable: false,
                defaultValue: 0);
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

        }
    }
}
