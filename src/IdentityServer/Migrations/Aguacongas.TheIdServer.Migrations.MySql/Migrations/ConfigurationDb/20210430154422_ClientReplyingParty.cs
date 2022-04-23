// Project: Aguafrommars/TheIdServer
// Copyright (c) 2022 @Olivier Lefebvre
using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Aguacongas.TheIdServer.MySql.Migrations.ConfigurationDb
{
    public partial class ClientReplyingParty : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RelyingPartyId",
                table: "Clients",
                type: "varchar(255)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Clients_RelyingPartyId",
                table: "Clients",
                column: "RelyingPartyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Clients_RelyingParties_RelyingPartyId",
                table: "Clients",
                column: "RelyingPartyId",
                principalTable: "RelyingParties",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Clients_RelyingParties_RelyingPartyId",
                table: "Clients");

            migrationBuilder.DropIndex(
                name: "IX_Clients_RelyingPartyId",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "RelyingPartyId",
                table: "Clients");
        }
    }
}
