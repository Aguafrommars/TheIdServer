using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aguacongas.TheIdServer.Oracle.Migrations.OperationalDb
{
    public partial class ServerSideSession : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserSessions",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar2(450)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar2(200)", nullable: false),
                    Scheme = table.Column<string>(type: "nclob", nullable: true),
                    SessionId = table.Column<string>(type: "nclob", nullable: true),
                    DisplayName = table.Column<string>(type: "nclob", nullable: true),
                    Created = table.Column<DateTime>(type: "timestamp", nullable: false),
                    Renewed = table.Column<DateTime>(type: "timestamp", nullable: false),
                    Expires = table.Column<DateTime>(type: "timestamp", nullable: true),
                    Ticket = table.Column<string>(type: "nclob", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSessions", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserSessions_UserId",
                table: "UserSessions",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserSessions");
        }
    }
}
