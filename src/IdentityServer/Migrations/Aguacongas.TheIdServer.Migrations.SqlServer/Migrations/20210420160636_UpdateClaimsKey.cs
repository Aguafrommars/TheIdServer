// Project: Aguafrommars/TheIdServer
// Copyright (c) 2022 @Olivier Lefebvre
using Microsoft.EntityFrameworkCore.Migrations;

namespace Aguacongas.TheIdServer.SqlServer.Migrations
{
    public partial class UpdateClaimsKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey("PK_AspNetUserClaims", "AspNetUserClaims");
            migrationBuilder.DropColumn("Id", "AspNetUserClaims");
            migrationBuilder.AddColumn<string>("Id", "AspNetUserClaims", type: "nvarchar(450)", defaultValueSql: "newid()");
            migrationBuilder.AddPrimaryKey("PK_AspNetUserClaims", "AspNetUserClaims", "Id");

            migrationBuilder.DropPrimaryKey("PK_AspNetRoleClaims", "AspNetRoleClaims");
            migrationBuilder.DropColumn("Id", "AspNetRoleClaims");
            migrationBuilder.AddColumn<string>("Id", "AspNetRoleClaims", type: "nvarchar(450)", defaultValueSql: "newid()");
            migrationBuilder.AddPrimaryKey("PK_AspNetRoleClaims", "AspNetRoleClaims", "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey("PK_AspNetUserClaims", "AspNetUserClaims");
            migrationBuilder.DropColumn("Id", "AspNetUserClaims");
            migrationBuilder.AddColumn<int>("Id", "AspNetUserClaims", type: "int")
                .Annotation("SqlServer:Identity", "1, 1");
            migrationBuilder.AddPrimaryKey("PK_AspNetUserClaims", "AspNetUserClaims", "Id");

            migrationBuilder.DropPrimaryKey("PK_AspNetRoleClaims", "AspNetRoleClaims");
            migrationBuilder.DropColumn("Id", "AspNetRoleClaims");
            migrationBuilder.AddColumn<int>("Id", "AspNetRoleClaims", type: "int")
                .Annotation("SqlServer:Identity", "1, 1");
            migrationBuilder.AddPrimaryKey("PK_AspNetRoleClaims", "AspNetRoleClaims", "Id");
        }
    }
}
