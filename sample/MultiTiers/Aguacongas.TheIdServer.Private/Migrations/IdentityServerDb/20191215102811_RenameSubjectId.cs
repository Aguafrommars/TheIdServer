using Microsoft.EntityFrameworkCore.Migrations;

namespace Aguacongas.TheIdServer.Migrations.IdentityServerDb
{
    public partial class RenameSubjectId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClientIdPRestriction_Clients_ClientId",
                table: "ClientIdPRestriction");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ClientIdPRestriction",
                table: "ClientIdPRestriction");

            migrationBuilder.DropColumn(
                name: "SubjectId",
                table: "UserConstents");

            migrationBuilder.DropColumn(
                name: "SubjectId",
                table: "RefreshTokens");

            migrationBuilder.DropColumn(
                name: "SubjectId",
                table: "ReferenceTokens");

            migrationBuilder.RenameTable(
                name: "ClientIdPRestriction",
                newName: "ClientIdpRestriction");

            migrationBuilder.RenameIndex(
                name: "IX_ClientIdPRestriction_ClientId_Provider",
                table: "ClientIdpRestriction",
                newName: "IX_ClientIdpRestriction_ClientId_Provider");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "UserConstents",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "RefreshTokens",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "ReferenceTokens",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ClientIdpRestriction",
                table: "ClientIdpRestriction",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ClientIdpRestriction_Clients_ClientId",
                table: "ClientIdpRestriction",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClientIdpRestriction_Clients_ClientId",
                table: "ClientIdpRestriction");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ClientIdpRestriction",
                table: "ClientIdpRestriction");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "UserConstents");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "RefreshTokens");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "ReferenceTokens");

            migrationBuilder.RenameTable(
                name: "ClientIdpRestriction",
                newName: "ClientIdPRestriction");

            migrationBuilder.RenameIndex(
                name: "IX_ClientIdpRestriction_ClientId_Provider",
                table: "ClientIdPRestriction",
                newName: "IX_ClientIdPRestriction_ClientId_Provider");

            migrationBuilder.AddColumn<string>(
                name: "SubjectId",
                table: "UserConstents",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SubjectId",
                table: "RefreshTokens",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SubjectId",
                table: "ReferenceTokens",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ClientIdPRestriction",
                table: "ClientIdPRestriction",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ClientIdPRestriction_Clients_ClientId",
                table: "ClientIdPRestriction",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
