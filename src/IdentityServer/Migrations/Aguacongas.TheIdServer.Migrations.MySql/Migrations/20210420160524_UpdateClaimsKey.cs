// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Aguacongas.TheIdServer.MySql.Migrations
{
    public partial class UpdateClaimsKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>("Id", "AspNetUserClaims", type: "varchar(255) CHARACTER SET utf8mb4", defaultValueSql: "(uuid())");
            migrationBuilder.AlterColumn<string>("Id", "AspNetRoleClaims", type: "varchar(255) CHARACTER SET utf8mb4", defaultValueSql: "(uuid())");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>("Id", "AspNetUserClaims", type: "int")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);
            migrationBuilder.AlterColumn<int>("Id", "AspNetRoleClaims", type: "int")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);
        }
    }
}
