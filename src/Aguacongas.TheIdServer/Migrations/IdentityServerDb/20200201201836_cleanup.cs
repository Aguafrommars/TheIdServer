using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Aguacongas.TheIdServer.Migrations.IdentityServerDb
{
    public partial class cleanup : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "Expiration",
                table: "UserConstents",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Expiration",
                table: "RefreshTokens",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Expiration",
                table: "ReferenceTokens",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Expiration",
                table: "AuthorizationCodes",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Expiration",
                table: "UserConstents");

            migrationBuilder.DropColumn(
                name: "Expiration",
                table: "RefreshTokens");

            migrationBuilder.DropColumn(
                name: "Expiration",
                table: "ReferenceTokens");

            migrationBuilder.DropColumn(
                name: "Expiration",
                table: "AuthorizationCodes");
        }
    }
}
