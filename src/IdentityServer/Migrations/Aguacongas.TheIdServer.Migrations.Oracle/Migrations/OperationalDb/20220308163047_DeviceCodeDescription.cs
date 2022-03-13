using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aguacongas.TheIdServer.Oracle.Migrations.OperationalDb
{
    public partial class DeviceCodeDescription : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
               name: "Description",
               table: "DeviceCodes",
               maxLength: 200,
               nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SessionId",
                table: "DeviceCodes",
                maxLength: 100,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BackChannelAuthenticationRequests");
        }
    }
}
