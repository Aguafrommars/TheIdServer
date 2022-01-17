using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aguacongas.TheIdServer.Sqlite.Migrations.ConfigurationDb
{
    public partial class Ciba : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CibaLifetime",
                table: "Clients",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PollingInterval",
                table: "Clients",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Cultures",
                keyColumn: "Id",
                keyValue: "en",
                column: "CreatedAt",
                value: new DateTime(2022, 1, 15, 10, 11, 22, 359, DateTimeKind.Utc).AddTicks(3759));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CibaLifetime",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "PollingInterval",
                table: "Clients");

            migrationBuilder.UpdateData(
                table: "Cultures",
                keyColumn: "Id",
                keyValue: "en",
                column: "CreatedAt",
                value: new DateTime(2021, 5, 3, 16, 17, 38, 537, DateTimeKind.Utc).AddTicks(8840));
        }
    }
}
