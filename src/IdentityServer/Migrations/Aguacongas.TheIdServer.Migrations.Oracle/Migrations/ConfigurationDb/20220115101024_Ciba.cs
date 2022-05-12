using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aguacongas.TheIdServer.Oracle.Migrations.ConfigurationDb
{
    public partial class Ciba : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CibaLifetime",
                table: "Clients",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PollingInterval",
                table: "Clients",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CibaLifetime",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "PollingInterval",
                table: "Clients");
        }
    }
}
