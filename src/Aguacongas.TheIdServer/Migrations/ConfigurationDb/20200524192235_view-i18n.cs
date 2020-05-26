using Microsoft.EntityFrameworkCore.Migrations;

namespace Aguacongas.TheIdServer.Migrations.ConfigurationDb
{
    public partial class viewi18n : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LocalizedResources_Cultures_CultureId",
                table: "LocalizedResources");

            migrationBuilder.AlterColumn<string>(
                name: "CultureId",
                table: "LocalizedResources",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BaseName",
                table: "LocalizedResources",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "LocalizedResources",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_LocalizedResources_Cultures_CultureId",
                table: "LocalizedResources",
                column: "CultureId",
                principalTable: "Cultures",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LocalizedResources_Cultures_CultureId",
                table: "LocalizedResources");

            migrationBuilder.DropColumn(
                name: "BaseName",
                table: "LocalizedResources");

            migrationBuilder.DropColumn(
                name: "Location",
                table: "LocalizedResources");

            migrationBuilder.AlterColumn<string>(
                name: "CultureId",
                table: "LocalizedResources",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AddForeignKey(
                name: "FK_LocalizedResources_Cultures_CultureId",
                table: "LocalizedResources",
                column: "CultureId",
                principalTable: "Cultures",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
