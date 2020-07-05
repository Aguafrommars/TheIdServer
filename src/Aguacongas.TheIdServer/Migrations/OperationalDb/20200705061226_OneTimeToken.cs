using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Aguacongas.TheIdServer.Migrations.OperationalDb
{
    public partial class OneTimeToken : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OneTimeTokens",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    ClientId = table.Column<string>(nullable: false),
                    UserId = table.Column<string>(maxLength: 200, nullable: true),
                    Data = table.Column<string>(nullable: true),
                    SessionId = table.Column<string>(nullable: true),
                    Expiration = table.Column<DateTime>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OneTimeTokens", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OneTimeTokens");
        }
    }
}
