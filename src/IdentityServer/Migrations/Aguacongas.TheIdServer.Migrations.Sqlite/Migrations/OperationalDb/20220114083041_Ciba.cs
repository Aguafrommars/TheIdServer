using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aguacongas.TheIdServer.Sqlite.Migrations.OperationalDb
{
    public partial class Ciba : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BackChannelAuthenticationRequests",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    ClientId = table.Column<string>(type: "TEXT", nullable: false),
                    UserId = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Data = table.Column<string>(type: "TEXT", nullable: true),
                    SessionId = table.Column<string>(type: "TEXT", nullable: false),
                    Expiration = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BackChannelAuthenticationRequests", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BackChannelAuthenticationRequests");
        }
    }
}
