using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aguacongas.TheIdServer.Oracle.Migrations.OperationalDb
{
    /// <inheritdoc />
    public partial class PushAuthorizationRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PushedAuthorizationRequests",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar2(450)", nullable: false),
                    ExpiresAtUtc = table.Column<DateTime>(type: "timestamp", nullable: false),
                    Parameters = table.Column<string>(type: "nclob", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "timestamp", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PushedAuthorizationRequests", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PushedAuthorizationRequests");
        }
    }
}
