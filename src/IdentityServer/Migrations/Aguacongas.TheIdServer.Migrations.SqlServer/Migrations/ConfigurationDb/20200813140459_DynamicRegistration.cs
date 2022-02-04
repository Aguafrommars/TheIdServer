// Project: Aguafrommars/TheIdServer
// Copyright (c) 2022 @Olivier Lefebvre
using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Aguacongas.TheIdServer.SqlServer.Migrations.ConfigurationDb
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
                value: new DateTime(2020, 8, 13, 14, 4, 59, 203, DateTimeKind.Utc).AddTicks(2307));
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
                value: new DateTime(2020, 7, 25, 17, 46, 3, 962, DateTimeKind.Utc).AddTicks(2581));
        }
    }
}
