using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Aguacongas.TheIdServer.Migrations.ConfigurationDb
{
    public partial class ClaimTransformations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "StoreClaims",
                table: "Providers",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "ExternalClaimTransformation",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Scheme = table.Column<string>(nullable: false),
                    FromClaimType = table.Column<string>(nullable: false),
                    ToClaimType = table.Column<string>(nullable: false),
                    AsMultipleValues = table.Column<bool>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExternalClaimTransformation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExternalClaimTransformation_Providers_Scheme",
                        column: x => x.Scheme,
                        principalTable: "Providers",
                        principalColumn: "Scheme",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExternalClaimTransformation_Scheme_FromClaimType",
                table: "ExternalClaimTransformation",
                columns: new[] { "Scheme", "FromClaimType" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExternalClaimTransformation");

            migrationBuilder.DropColumn(
                name: "StoreClaims",
                table: "Providers");
        }
    }
}
