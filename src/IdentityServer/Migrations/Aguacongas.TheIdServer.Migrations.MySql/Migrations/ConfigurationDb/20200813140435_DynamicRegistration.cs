using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Aguacongas.TheIdServer.MySql.Migrations.ConfigurationDb
{
    public partial class DynamicRegistration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PolicyUri",
                table: "Clients",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "RegistrationToken",
                table: "Clients",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TosUri",
                table: "Clients",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Cultures",
                keyColumn: "Id",
                keyValue: "en",
                column: "CreatedAt",
                value: new DateTime(2020, 8, 13, 14, 4, 35, 460, DateTimeKind.Utc).AddTicks(9133));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PolicyUri",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "RegistrationToken",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "TosUri",
                table: "Clients");

            migrationBuilder.UpdateData(
                table: "Cultures",
                keyColumn: "Id",
                keyValue: "en",
                column: "CreatedAt",
                value: new DateTime(2020, 7, 25, 17, 44, 19, 192, DateTimeKind.Utc).AddTicks(5978));
        }
    }
}
