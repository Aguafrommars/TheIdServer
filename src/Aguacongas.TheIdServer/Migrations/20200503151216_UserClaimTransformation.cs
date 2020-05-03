using Microsoft.EntityFrameworkCore.Migrations;

namespace Aguacongas.TheIdServer.Migrations
{
    public partial class UserClaimTransformation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Issuer",
                table: "AspNetUserClaims",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OriginalValue",
                table: "AspNetUserClaims",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Issuer",
                table: "AspNetUserClaims");

            migrationBuilder.DropColumn(
                name: "OriginalValue",
                table: "AspNetUserClaims");
        }
    }
}
