// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Microsoft.EntityFrameworkCore.Migrations;

namespace Aguacongas.TheIdServer.Sqlite.Migrations
{
    public partial class UpdateClaimsKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>("Id", "AspNetUserClaims", type: "TEXT", defaultValueSql: "hex(randomblob(16))");
            migrationBuilder.AlterColumn<string>("Id", "AspNetRoleClaims", type: "TEXT", defaultValueSql: "hex(randomblob(16))");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>("Id", "AspNetUserClaims", type: "int");
            migrationBuilder.AlterColumn<string>("Id", "AspNetRoleClaims", type: "int");
        }
    }
}
