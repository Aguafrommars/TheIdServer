// Project: Aguafrommars/TheIdServer
// Copyright (c) 2023 @Olivier Lefebvre
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Aguacongas.TheIdServer.PostgreSQL.Migrations
{
    public partial class UpdateClaimsKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("CREATE EXTENSION IF NOT EXISTS \"uuid-ossp\";");
            migrationBuilder.DropPrimaryKey("PK_AspNetUserClaims", "AspNetUserClaims");
            migrationBuilder.DropColumn("Id", "AspNetUserClaims");
            migrationBuilder.AddColumn<string>("Id", "AspNetUserClaims", type: "text", defaultValueSql: "uuid_generate_v4()");
            migrationBuilder.AddPrimaryKey("PK_AspNetUserClaims", "AspNetUserClaims", "Id");

            migrationBuilder.DropPrimaryKey("PK_AspNetRoleClaims", "AspNetRoleClaims");
            migrationBuilder.DropColumn("Id", "AspNetRoleClaims");
            migrationBuilder.AddColumn<string>("Id", "AspNetRoleClaims", type: "text", defaultValueSql: "uuid_generate_v4()");
            migrationBuilder.AddPrimaryKey("PK_AspNetRoleClaims", "AspNetRoleClaims", "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey("PK_AspNetUserClaims", "AspNetUserClaims");
            migrationBuilder.DropColumn("Id", "AspNetUserClaims");
            migrationBuilder.AddColumn<int>("Id", "AspNetUserClaims", type: "int")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);
            migrationBuilder.AddPrimaryKey("PK_AspNetUserClaims", "AspNetUserClaims", "Id");

            migrationBuilder.DropPrimaryKey("PK_AspNetRoleClaims", "AspNetRoleClaims");
            migrationBuilder.DropColumn("Id", "AspNetRoleClaims");
            migrationBuilder.AddColumn<int>("Id", "AspNetRoleClaims", type: "int")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);
            migrationBuilder.AddPrimaryKey("PK_AspNetRoleClaims", "AspNetRoleClaims", "Id");
        }
    }
}
