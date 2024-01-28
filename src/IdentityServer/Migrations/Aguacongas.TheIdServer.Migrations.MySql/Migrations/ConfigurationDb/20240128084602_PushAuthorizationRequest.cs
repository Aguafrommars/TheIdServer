using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aguacongas.TheIdServer.MySql.Migrations.ConfigurationDb
{
    /// <inheritdoc />
    public partial class PushAuthorizationRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PushedAuthorizationLifetime",
                table: "Clients",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "RequirePushedAuthorization",
                table: "Clients",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "Cultures",
                keyColumn: "Id",
                keyValue: "en",
                column: "CreatedAt",
                value: new DateTime(2024, 1, 28, 8, 46, 1, 739, DateTimeKind.Utc).AddTicks(6037));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PushedAuthorizationLifetime",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "RequirePushedAuthorization",
                table: "Clients");

            migrationBuilder.UpdateData(
                table: "Cultures",
                keyColumn: "Id",
                keyValue: "en",
                column: "CreatedAt",
                value: new DateTime(2023, 5, 20, 8, 24, 17, 622, DateTimeKind.Utc).AddTicks(6681));
        }
    }
}
