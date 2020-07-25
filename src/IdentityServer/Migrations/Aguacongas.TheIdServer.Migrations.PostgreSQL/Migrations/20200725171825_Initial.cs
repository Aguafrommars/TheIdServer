using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Aguacongas.TheIdServer.PostgreSQL.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Apis",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Enabled = table.Column<bool>(nullable: false),
                    DisplayName = table.Column<string>(maxLength: 200, nullable: false),
                    Description = table.Column<string>(maxLength: 1000, nullable: true),
                    NonEditable = table.Column<bool>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Apis", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ApiScopes",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Enabled = table.Column<bool>(nullable: false),
                    DisplayName = table.Column<string>(maxLength: 200, nullable: false),
                    Description = table.Column<string>(maxLength: 1000, nullable: true),
                    Required = table.Column<bool>(nullable: false),
                    Emphasize = table.Column<bool>(nullable: false),
                    ShowInDiscoveryDocument = table.Column<bool>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiScopes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Clients",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    AllowOfflineAccess = table.Column<bool>(nullable: false),
                    IdentityTokenLifetime = table.Column<int>(nullable: false),
                    AccessTokenLifetime = table.Column<int>(nullable: false),
                    AuthorizationCodeLifetime = table.Column<int>(nullable: false),
                    AbsoluteRefreshTokenLifetime = table.Column<int>(nullable: false),
                    SlidingRefreshTokenLifetime = table.Column<int>(nullable: false),
                    ConsentLifetime = table.Column<int>(nullable: true),
                    RefreshTokenUsage = table.Column<int>(nullable: false),
                    UpdateAccessTokenClaimsOnRefresh = table.Column<bool>(nullable: false),
                    RefreshTokenExpiration = table.Column<int>(nullable: false),
                    AccessTokenType = table.Column<int>(nullable: false),
                    EnableLocalLogin = table.Column<bool>(nullable: false),
                    IncludeJwtId = table.Column<bool>(nullable: false),
                    AlwaysSendClientClaims = table.Column<bool>(nullable: false),
                    ClientClaimsPrefix = table.Column<string>(maxLength: 250, nullable: true),
                    PairWiseSubjectSalt = table.Column<string>(maxLength: 200, nullable: true),
                    UserSsoLifetime = table.Column<int>(nullable: true),
                    UserCodeType = table.Column<string>(maxLength: 100, nullable: true),
                    DeviceCodeLifetime = table.Column<int>(nullable: false),
                    AlwaysIncludeUserClaimsInIdToken = table.Column<bool>(nullable: false),
                    BackChannelLogoutSessionRequired = table.Column<bool>(nullable: false),
                    Enabled = table.Column<bool>(nullable: false),
                    ProtocolType = table.Column<string>(maxLength: 200, nullable: false),
                    RequireClientSecret = table.Column<bool>(nullable: false),
                    ClientName = table.Column<string>(maxLength: 200, nullable: true),
                    Description = table.Column<string>(maxLength: 1000, nullable: true),
                    ClientUri = table.Column<string>(maxLength: 2000, nullable: true),
                    LogoUri = table.Column<string>(maxLength: 2000, nullable: true),
                    RequireConsent = table.Column<bool>(nullable: false),
                    RequirePkce = table.Column<bool>(nullable: false),
                    AllowPlainTextPkce = table.Column<bool>(nullable: false),
                    AllowAccessTokensViaBrowser = table.Column<bool>(nullable: false),
                    FrontChannelLogoutUri = table.Column<string>(maxLength: 2000, nullable: true),
                    FrontChannelLogoutSessionRequired = table.Column<bool>(nullable: false),
                    BackChannelLogoutUri = table.Column<string>(maxLength: 2000, nullable: true),
                    AllowRememberConsent = table.Column<bool>(nullable: false),
                    NonEditable = table.Column<bool>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clients", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Cultures",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cultures", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Identities",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Enabled = table.Column<bool>(nullable: false),
                    DisplayName = table.Column<string>(maxLength: 200, nullable: true),
                    Description = table.Column<string>(maxLength: 1000, nullable: true),
                    Required = table.Column<bool>(nullable: false),
                    Emphasize = table.Column<bool>(nullable: false),
                    ShowInDiscoveryDocument = table.Column<bool>(nullable: false),
                    NonEditable = table.Column<bool>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Identities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Providers",
                columns: table => new
                {
                    Scheme = table.Column<string>(nullable: false),
                    DisplayName = table.Column<string>(nullable: true),
                    SerializedHandlerType = table.Column<string>(nullable: true),
                    SerializedOptions = table.Column<string>(nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    StoreClaims = table.Column<bool>(nullable: false),
                    MapDefaultOutboundClaimType = table.Column<bool>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Providers", x => x.Scheme);
                });

            migrationBuilder.CreateTable(
                name: "ApiClaims",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    ApiId = table.Column<string>(nullable: false),
                    Type = table.Column<string>(maxLength: 250, nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApiClaims_Apis_ApiId",
                        column: x => x.ApiId,
                        principalTable: "Apis",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ApiLocalizedResources",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: true),
                    CultureId = table.Column<string>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: true),
                    ApiId = table.Column<string>(nullable: false),
                    ResourceKind = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiLocalizedResources", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApiLocalizedResources_Apis_ApiId",
                        column: x => x.ApiId,
                        principalTable: "Apis",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ApiProperty",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    ApiId = table.Column<string>(nullable: false),
                    Key = table.Column<string>(maxLength: 250, nullable: false),
                    Value = table.Column<string>(maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiProperty", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApiProperty_Apis_ApiId",
                        column: x => x.ApiId,
                        principalTable: "Apis",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ApiSecrets",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    ApiId = table.Column<string>(nullable: false),
                    Description = table.Column<string>(maxLength: 1000, nullable: true),
                    Value = table.Column<string>(nullable: false),
                    Expiration = table.Column<DateTime>(nullable: true),
                    Type = table.Column<string>(maxLength: 250, nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiSecrets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApiSecrets_Apis_ApiId",
                        column: x => x.ApiId,
                        principalTable: "Apis",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ApiApiScope",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    ApiId = table.Column<string>(nullable: false),
                    ApiScopeId = table.Column<string>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiApiScope", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApiApiScope_Apis_ApiId",
                        column: x => x.ApiId,
                        principalTable: "Apis",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApiApiScope_ApiScopes_ApiScopeId",
                        column: x => x.ApiScopeId,
                        principalTable: "ApiScopes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ApiScopeClaims",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    ApiScopeId = table.Column<string>(nullable: false),
                    Type = table.Column<string>(maxLength: 250, nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiScopeClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApiScopeClaims_ApiScopes_ApiScopeId",
                        column: x => x.ApiScopeId,
                        principalTable: "ApiScopes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ApiScopeLocalizedResources",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: true),
                    CultureId = table.Column<string>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: true),
                    ApiScopeId = table.Column<string>(nullable: false),
                    ResourceKind = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiScopeLocalizedResources", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApiScopeLocalizedResources_ApiScopes_ApiScopeId",
                        column: x => x.ApiScopeId,
                        principalTable: "ApiScopes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ApiScopeProperty",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    ApiScopeId = table.Column<string>(nullable: false),
                    Key = table.Column<string>(maxLength: 250, nullable: false),
                    Value = table.Column<string>(maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiScopeProperty", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApiScopeProperty_ApiScopes_ApiScopeId",
                        column: x => x.ApiScopeId,
                        principalTable: "ApiScopes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClientClaims",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    ClientId = table.Column<string>(nullable: false),
                    Type = table.Column<string>(maxLength: 250, nullable: false),
                    Value = table.Column<string>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClientClaims_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClientGrantTypes",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    ClientId = table.Column<string>(nullable: false),
                    GrantType = table.Column<string>(maxLength: 250, nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientGrantTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClientGrantTypes_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClientIdpRestriction",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    ClientId = table.Column<string>(nullable: false),
                    Provider = table.Column<string>(maxLength: 200, nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientIdpRestriction", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClientIdpRestriction_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClientLocalizedResources",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: true),
                    CultureId = table.Column<string>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: true),
                    ClientId = table.Column<string>(nullable: false),
                    ResourceKind = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientLocalizedResources", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClientLocalizedResources_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClientProperties",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    ClientId = table.Column<string>(nullable: false),
                    Key = table.Column<string>(maxLength: 250, nullable: false),
                    Value = table.Column<string>(maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientProperties", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClientProperties_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClientScopes",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    ClientId = table.Column<string>(nullable: false),
                    Scope = table.Column<string>(maxLength: 200, nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientScopes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClientScopes_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClientSecrets",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    ClientId = table.Column<string>(nullable: false),
                    Description = table.Column<string>(maxLength: 2000, nullable: true),
                    Value = table.Column<string>(nullable: false),
                    Expiration = table.Column<DateTime>(nullable: true),
                    Type = table.Column<string>(maxLength: 250, nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientSecrets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClientSecrets_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClientUris",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    ClientId = table.Column<string>(nullable: false),
                    Uri = table.Column<string>(maxLength: 2000, nullable: false),
                    SanetizedCorsUri = table.Column<string>(maxLength: 2000, nullable: true),
                    Kind = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientUris", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClientUris_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LocalizedResources",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: true),
                    CultureId = table.Column<string>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: true),
                    Key = table.Column<string>(nullable: false),
                    BaseName = table.Column<string>(nullable: true),
                    Location = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocalizedResources", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LocalizedResources_Cultures_CultureId",
                        column: x => x.CultureId,
                        principalTable: "Cultures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IdentityClaims",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    IdentityId = table.Column<string>(nullable: false),
                    Type = table.Column<string>(maxLength: 250, nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IdentityClaims_Identities_IdentityId",
                        column: x => x.IdentityId,
                        principalTable: "Identities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IdentityLocalizedResources",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: true),
                    CultureId = table.Column<string>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: true),
                    IdentityId = table.Column<string>(nullable: false),
                    ResourceKind = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityLocalizedResources", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IdentityLocalizedResources_Identities_IdentityId",
                        column: x => x.IdentityId,
                        principalTable: "Identities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IdentityProperties",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    IdentityId = table.Column<string>(nullable: false),
                    Key = table.Column<string>(maxLength: 250, nullable: false),
                    Value = table.Column<string>(maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityProperties", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IdentityProperties_Identities_IdentityId",
                        column: x => x.IdentityId,
                        principalTable: "Identities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExternalClaimTransformations",
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
                    table.PrimaryKey("PK_ExternalClaimTransformations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExternalClaimTransformations_Providers_Scheme",
                        column: x => x.Scheme,
                        principalTable: "Providers",
                        principalColumn: "Scheme",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Cultures",
                columns: new[] { "Id", "CreatedAt", "ModifiedAt" },
                values: new object[] { "en", new DateTime(2020, 7, 25, 17, 18, 25, 551, DateTimeKind.Utc).AddTicks(9111), null });

            migrationBuilder.CreateIndex(
                name: "IX_ApiApiScope_ApiId",
                table: "ApiApiScope",
                column: "ApiId");

            migrationBuilder.CreateIndex(
                name: "IX_ApiApiScope_ApiScopeId",
                table: "ApiApiScope",
                column: "ApiScopeId");

            migrationBuilder.CreateIndex(
                name: "IX_ApiClaims_ApiId_Type",
                table: "ApiClaims",
                columns: new[] { "ApiId", "Type" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ApiLocalizedResources_ApiId",
                table: "ApiLocalizedResources",
                column: "ApiId");

            migrationBuilder.CreateIndex(
                name: "IX_ApiProperty_ApiId_Key",
                table: "ApiProperty",
                columns: new[] { "ApiId", "Key" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ApiScopeClaims_ApiScopeId_Type",
                table: "ApiScopeClaims",
                columns: new[] { "ApiScopeId", "Type" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ApiScopeLocalizedResources_ApiScopeId",
                table: "ApiScopeLocalizedResources",
                column: "ApiScopeId");

            migrationBuilder.CreateIndex(
                name: "IX_ApiScopeProperty_ApiScopeId_Key",
                table: "ApiScopeProperty",
                columns: new[] { "ApiScopeId", "Key" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ApiSecrets_ApiId",
                table: "ApiSecrets",
                column: "ApiId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientClaims_ClientId",
                table: "ClientClaims",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientGrantTypes_ClientId_GrantType",
                table: "ClientGrantTypes",
                columns: new[] { "ClientId", "GrantType" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClientIdpRestriction_ClientId_Provider",
                table: "ClientIdpRestriction",
                columns: new[] { "ClientId", "Provider" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClientLocalizedResources_ClientId",
                table: "ClientLocalizedResources",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientProperties_ClientId_Key",
                table: "ClientProperties",
                columns: new[] { "ClientId", "Key" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClientScopes_ClientId_Scope",
                table: "ClientScopes",
                columns: new[] { "ClientId", "Scope" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClientSecrets_ClientId",
                table: "ClientSecrets",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientUris_ClientId_Uri",
                table: "ClientUris",
                columns: new[] { "ClientId", "Uri" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ExternalClaimTransformations_Scheme_FromClaimType",
                table: "ExternalClaimTransformations",
                columns: new[] { "Scheme", "FromClaimType" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_IdentityClaims_IdentityId_Type",
                table: "IdentityClaims",
                columns: new[] { "IdentityId", "Type" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_IdentityLocalizedResources_IdentityId",
                table: "IdentityLocalizedResources",
                column: "IdentityId");

            migrationBuilder.CreateIndex(
                name: "IX_IdentityProperties_IdentityId_Key",
                table: "IdentityProperties",
                columns: new[] { "IdentityId", "Key" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LocalizedResources_CultureId",
                table: "LocalizedResources",
                column: "CultureId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApiApiScope");

            migrationBuilder.DropTable(
                name: "ApiClaims");

            migrationBuilder.DropTable(
                name: "ApiLocalizedResources");

            migrationBuilder.DropTable(
                name: "ApiProperty");

            migrationBuilder.DropTable(
                name: "ApiScopeClaims");

            migrationBuilder.DropTable(
                name: "ApiScopeLocalizedResources");

            migrationBuilder.DropTable(
                name: "ApiScopeProperty");

            migrationBuilder.DropTable(
                name: "ApiSecrets");

            migrationBuilder.DropTable(
                name: "ClientClaims");

            migrationBuilder.DropTable(
                name: "ClientGrantTypes");

            migrationBuilder.DropTable(
                name: "ClientIdpRestriction");

            migrationBuilder.DropTable(
                name: "ClientLocalizedResources");

            migrationBuilder.DropTable(
                name: "ClientProperties");

            migrationBuilder.DropTable(
                name: "ClientScopes");

            migrationBuilder.DropTable(
                name: "ClientSecrets");

            migrationBuilder.DropTable(
                name: "ClientUris");

            migrationBuilder.DropTable(
                name: "ExternalClaimTransformations");

            migrationBuilder.DropTable(
                name: "IdentityClaims");

            migrationBuilder.DropTable(
                name: "IdentityLocalizedResources");

            migrationBuilder.DropTable(
                name: "IdentityProperties");

            migrationBuilder.DropTable(
                name: "LocalizedResources");

            migrationBuilder.DropTable(
                name: "ApiScopes");

            migrationBuilder.DropTable(
                name: "Apis");

            migrationBuilder.DropTable(
                name: "Clients");

            migrationBuilder.DropTable(
                name: "Providers");

            migrationBuilder.DropTable(
                name: "Identities");

            migrationBuilder.DropTable(
                name: "Cultures");
        }
    }
}
