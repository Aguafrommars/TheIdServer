using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aguacongas.TheIdServer.SqlServer.Migrations.ConfigurationDb
{
    public partial class NewClientOptions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "RequireRequestObject",
                table: "Clients",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "ClientAllowedIdentityTokenSigningAlgorithms",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClientId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Algorithm = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientAllowedIdentityTokenSigningAlgorithms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClientAllowedIdentityTokenSigningAlgorithms_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Cultures",
                keyColumn: "Id",
                keyValue: "en",
                column: "CreatedAt",
                value: new DateTime(2022, 4, 12, 6, 35, 48, 947, DateTimeKind.Utc).AddTicks(5403));

            migrationBuilder.CreateIndex(
                name: "IX_ClientAllowedIdentityTokenSigningAlgorithms_ClientId_Algorithm",
                table: "ClientAllowedIdentityTokenSigningAlgorithms",
                columns: new[] { "ClientId", "Algorithm" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClientAllowedIdentityTokenSigningAlgorithms");

            migrationBuilder.DropColumn(
                name: "RequireRequestObject",
                table: "Clients");

            migrationBuilder.UpdateData(
                table: "Cultures",
                keyColumn: "Id",
                keyValue: "en",
                column: "CreatedAt",
                value: new DateTime(2022, 1, 15, 10, 11, 36, 949, DateTimeKind.Utc).AddTicks(2767));
        }
    }
}
