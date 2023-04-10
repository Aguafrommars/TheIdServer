// Project: Aguafrommars/TheIdServer
// Copyright (c) 2023 @Olivier Lefebvre
using System;
using Aguacongas.IdentityServer.EntityFramework.Store;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Aguacongas.TheIdServer.Oracle.Migrations.ConfigurationDb
{
    [DbContext(typeof(ConfigurationDbContext))]
    [Migration("20210430154516_ClientReplyingParty")]
    partial class ClientReplyingParty
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.6");

            modelBuilder.Entity("Aguacongas.IdentityServer.EntityFramework.Store.SchemeDefinition", b =>
            {
                b.Property<string>("Scheme")
                    .HasColumnType("nvarchar2(450)");

                b.Property<string>("ConcurrencyStamp")
                    .IsConcurrencyToken()
                    .HasColumnType("nclob");

                b.Property<DateTime>("CreatedAt")
                    .HasColumnType("timestamp");

                b.Property<string>("DisplayName")
                    .HasColumnType("nclob");

                b.Property<bool>("MapDefaultOutboundClaimType")
                    .HasColumnType("bool");

                b.Property<DateTime?>("ModifiedAt")
                    .HasColumnType("timestamp");

                b.Property<string>("SerializedHandlerType")
                    .HasColumnType("nclob");

                b.Property<string>("SerializedOptions")
                    .HasColumnType("nclob");

                b.Property<bool>("StoreClaims")
                    .HasColumnType("bool");

                b.HasKey("Scheme");

                b.ToTable("Providers");
            });

            modelBuilder.Entity("Aguacongas.IdentityServer.Store.Entity.ApiApiScope", b =>
            {
                b.Property<string>("Id")
                    .HasColumnType("nvarchar2(450)");

                b.Property<string>("ApiId")
                    .IsRequired()
                    .HasColumnType("nvarchar2(450)");

                b.Property<string>("ApiScopeId")
                    .IsRequired()
                    .HasColumnType("nvarchar2(450)");

                b.Property<DateTime>("CreatedAt")
                    .HasColumnType("timestamp");

                b.Property<DateTime?>("ModifiedAt")
                    .HasColumnType("timestamp");

                b.HasKey("Id");

                b.HasIndex("ApiId");

                b.HasIndex("ApiScopeId");

                b.ToTable("ApiApiScope");
            });

            modelBuilder.Entity("Aguacongas.IdentityServer.Store.Entity.ApiClaim", b =>
            {
                b.Property<string>("Id")
                    .HasColumnType("nvarchar2(450)");

                b.Property<string>("ApiId")
                    .IsRequired()
                    .HasColumnType("nvarchar2(450)");

                b.Property<DateTime>("CreatedAt")
                    .HasColumnType("timestamp");

                b.Property<DateTime?>("ModifiedAt")
                    .HasColumnType("timestamp");

                b.Property<string>("Type")
                    .IsRequired()
                    .HasColumnType("nvarchar2(250)")
                    .HasMaxLength(250);

                b.HasKey("Id");

                b.HasIndex("ApiId", "Type")
                    .IsUnique();

                b.ToTable("ApiClaims");
            });

            modelBuilder.Entity("Aguacongas.IdentityServer.Store.Entity.ApiLocalizedResource", b =>
            {
                b.Property<string>("Id")
                    .HasColumnType("nvarchar2(450)");

                b.Property<string>("ApiId")
                    .IsRequired()
                    .HasColumnType("nvarchar2(450)");

                b.Property<DateTime>("CreatedAt")
                    .HasColumnType("timestamp");

                b.Property<string>("CultureId")
                    .IsRequired()
                    .HasColumnType("nclob");

                b.Property<DateTime?>("ModifiedAt")
                    .HasColumnType("timestamp");

                b.Property<int>("ResourceKind")
                    .HasColumnType("int");

                b.Property<string>("Value")
                    .HasColumnType("nclob");

                b.HasKey("Id");

                b.HasIndex("ApiId");

                b.ToTable("ApiLocalizedResources");
            });

            modelBuilder.Entity("Aguacongas.IdentityServer.Store.Entity.ApiProperty", b =>
            {
                b.Property<string>("Id")
                    .HasColumnType("nvarchar2(450)");

                b.Property<string>("ApiId")
                    .IsRequired()
                    .HasColumnType("nvarchar2(450)");

                b.Property<DateTime>("CreatedAt")
                    .HasColumnType("timestamp");

                b.Property<string>("Key")
                    .IsRequired()
                    .HasColumnType("nvarchar2(250)")
                    .HasMaxLength(250);

                b.Property<DateTime?>("ModifiedAt")
                    .HasColumnType("timestamp");

                b.Property<string>("Value")
                    .HasColumnType("nvarchar2(2000)")
                    .HasMaxLength(2000);

                b.HasKey("Id");

                b.HasIndex("ApiId", "Key")
                    .IsUnique();

                b.ToTable("ApiProperty");
            });

            modelBuilder.Entity("Aguacongas.IdentityServer.Store.Entity.ApiScope", b =>
            {
                b.Property<string>("Id")
                    .HasColumnType("nvarchar2(450)");

                b.Property<DateTime>("CreatedAt")
                    .HasColumnType("timestamp");

                b.Property<string>("Description")
                    .HasColumnType("nvarchar2(1000)")
                    .HasMaxLength(1000);

                b.Property<string>("DisplayName")
                    .IsRequired()
                    .HasColumnType("nvarchar2(200)")
                    .HasMaxLength(200);

                b.Property<bool>("Emphasize")
                    .HasColumnType("bool");

                b.Property<bool>("Enabled")
                    .HasColumnType("bool");

                b.Property<DateTime?>("ModifiedAt")
                    .HasColumnType("timestamp");

                b.Property<bool>("Required")
                    .HasColumnType("bool");

                b.Property<bool>("ShowInDiscoveryDocument")
                    .HasColumnType("bool");

                b.HasKey("Id");

                b.ToTable("ApiScopes");
            });

            modelBuilder.Entity("Aguacongas.IdentityServer.Store.Entity.ApiScopeClaim", b =>
            {
                b.Property<string>("Id")
                    .HasColumnType("nvarchar2(450)");

                b.Property<string>("ApiScopeId")
                    .IsRequired()
                    .HasColumnType("nvarchar2(450)");

                b.Property<DateTime>("CreatedAt")
                    .HasColumnType("timestamp");

                b.Property<DateTime?>("ModifiedAt")
                    .HasColumnType("timestamp");

                b.Property<string>("Type")
                    .IsRequired()
                    .HasColumnType("nvarchar2(250)")
                    .HasMaxLength(250);

                b.HasKey("Id");

                b.HasIndex("ApiScopeId", "Type")
                    .IsUnique();

                b.ToTable("ApiScopeClaims");
            });

            modelBuilder.Entity("Aguacongas.IdentityServer.Store.Entity.ApiScopeLocalizedResource", b =>
            {
                b.Property<string>("Id")
                    .HasColumnType("nvarchar2(450)");

                b.Property<string>("ApiScopeId")
                    .IsRequired()
                    .HasColumnType("nvarchar2(450)");

                b.Property<DateTime>("CreatedAt")
                    .HasColumnType("timestamp");

                b.Property<string>("CultureId")
                    .IsRequired()
                    .HasColumnType("nclob");

                b.Property<DateTime?>("ModifiedAt")
                    .HasColumnType("timestamp");

                b.Property<int>("ResourceKind")
                    .HasColumnType("int");

                b.Property<string>("Value")
                    .HasColumnType("nclob");

                b.HasKey("Id");

                b.HasIndex("ApiScopeId");

                b.ToTable("ApiScopeLocalizedResources");
            });

            modelBuilder.Entity("Aguacongas.IdentityServer.Store.Entity.ApiScopeProperty", b =>
            {
                b.Property<string>("Id")
                    .HasColumnType("nvarchar2(450)");

                b.Property<string>("ApiScopeId")
                    .IsRequired()
                    .HasColumnType("nvarchar2(450)");

                b.Property<DateTime>("CreatedAt")
                    .HasColumnType("timestamp");

                b.Property<string>("Key")
                    .IsRequired()
                    .HasColumnType("nvarchar2(250)")
                    .HasMaxLength(250);

                b.Property<DateTime?>("ModifiedAt")
                    .HasColumnType("timestamp");

                b.Property<string>("Value")
                    .HasColumnType("nvarchar2(2000)")
                    .HasMaxLength(2000);

                b.HasKey("Id");

                b.HasIndex("ApiScopeId", "Key")
                    .IsUnique();

                b.ToTable("ApiScopeProperty");
            });

            modelBuilder.Entity("Aguacongas.IdentityServer.Store.Entity.ApiSecret", b =>
            {
                b.Property<string>("Id")
                    .HasColumnType("nvarchar2(450)");

                b.Property<string>("ApiId")
                    .IsRequired()
                    .HasColumnType("nvarchar2(450)");

                b.Property<DateTime>("CreatedAt")
                    .HasColumnType("timestamp");

                b.Property<string>("Description")
                    .HasColumnType("nvarchar2(1000)")
                    .HasMaxLength(1000);

                b.Property<DateTime?>("Expiration")
                    .HasColumnType("timestamp");

                b.Property<DateTime?>("ModifiedAt")
                    .HasColumnType("timestamp");

                b.Property<string>("Type")
                    .IsRequired()
                    .HasColumnType("nvarchar2(250)")
                    .HasMaxLength(250);

                b.Property<string>("Value")
                    .IsRequired()
                    .HasColumnType("nclob");

                b.HasKey("Id");

                b.HasIndex("ApiId");

                b.ToTable("ApiSecrets");
            });

            modelBuilder.Entity("Aguacongas.IdentityServer.Store.Entity.Client", b =>
            {
                b.Property<string>("Id")
                    .HasColumnType("nvarchar2(450)");

                b.Property<int>("AbsoluteRefreshTokenLifetime")
                    .HasColumnType("int");

                b.Property<int>("AccessTokenLifetime")
                    .HasColumnType("int");

                b.Property<int>("AccessTokenType")
                    .HasColumnType("int");

                b.Property<bool>("AllowAccessTokensViaBrowser")
                    .HasColumnType("bool");

                b.Property<bool>("AllowOfflineAccess")
                    .HasColumnType("bool");

                b.Property<bool>("AllowPlainTextPkce")
                    .HasColumnType("bool");

                b.Property<bool>("AllowRememberConsent")
                    .HasColumnType("bool");

                b.Property<bool>("AlwaysIncludeUserClaimsInIdToken")
                    .HasColumnType("bool");

                b.Property<bool>("AlwaysSendClientClaims")
                    .HasColumnType("bool");

                b.Property<int>("AuthorizationCodeLifetime")
                    .HasColumnType("int");

                b.Property<bool>("BackChannelLogoutSessionRequired")
                    .HasColumnType("bool");

                b.Property<string>("BackChannelLogoutUri")
                    .HasColumnType("nvarchar2(2000)")
                    .HasMaxLength(2000);

                b.Property<string>("ClientClaimsPrefix")
                    .HasColumnType("nvarchar2(250)")
                    .HasMaxLength(250);

                b.Property<string>("ClientName")
                    .HasColumnType("nvarchar2(200)")
                    .HasMaxLength(200);

                b.Property<string>("ClientUri")
                    .HasColumnType("nvarchar2(2000)")
                    .HasMaxLength(2000);

                b.Property<int?>("ConsentLifetime")
                    .HasColumnType("int");

                b.Property<DateTime>("CreatedAt")
                    .HasColumnType("timestamp");

                b.Property<string>("Description")
                    .HasColumnType("nvarchar2(1000)")
                    .HasMaxLength(1000);

                b.Property<int>("DeviceCodeLifetime")
                    .HasColumnType("int");

                b.Property<bool>("EnableLocalLogin")
                    .HasColumnType("bool");

                b.Property<bool>("Enabled")
                    .HasColumnType("bool");

                b.Property<bool>("FrontChannelLogoutSessionRequired")
                    .HasColumnType("bool");

                b.Property<string>("FrontChannelLogoutUri")
                    .HasColumnType("nvarchar2(2000)")
                    .HasMaxLength(2000);

                b.Property<int>("IdentityTokenLifetime")
                    .HasColumnType("int");

                b.Property<bool>("IncludeJwtId")
                    .HasColumnType("bool");

                b.Property<string>("LogoUri")
                    .HasColumnType("nvarchar2(2000)")
                    .HasMaxLength(2000);

                b.Property<DateTime?>("ModifiedAt")
                    .HasColumnType("timestamp");

                b.Property<bool>("NonEditable")
                    .HasColumnType("bool");

                b.Property<string>("PairWiseSubjectSalt")
                    .HasColumnType("nvarchar2(200)")
                    .HasMaxLength(200);

                b.Property<string>("PolicyUri")
                    .HasColumnType("nclob");

                b.Property<string>("ProtocolType")
                    .IsRequired()
                    .HasColumnType("nvarchar2(200)")
                    .HasMaxLength(200);

                b.Property<int>("RefreshTokenExpiration")
                    .HasColumnType("int");

                b.Property<int>("RefreshTokenUsage")
                    .HasColumnType("int");

                b.Property<Guid?>("RegistrationToken")
                    .HasColumnType("guid");

                b.Property<bool>("RequireClientSecret")
                    .HasColumnType("bool");

                b.Property<bool>("RequireConsent")
                    .HasColumnType("bool");

                b.Property<bool>("RequirePkce")
                    .HasColumnType("bool");

                b.Property<int>("SlidingRefreshTokenLifetime")
                    .HasColumnType("int");

                b.Property<string>("TosUri")
                    .HasColumnType("nclob");

                b.Property<bool>("UpdateAccessTokenClaimsOnRefresh")
                    .HasColumnType("bool");

                b.Property<string>("UserCodeType")
                    .HasColumnType("nvarchar2(100)")
                    .HasMaxLength(100);

                b.Property<int?>("UserSsoLifetime")
                    .HasColumnType("int");

                b.HasKey("Id");

                b.ToTable("Clients");
            });

            modelBuilder.Entity("Aguacongas.IdentityServer.Store.Entity.ClientClaim", b =>
            {
                b.Property<string>("Id")
                    .HasColumnType("nvarchar2(450)");

                b.Property<string>("ClientId")
                    .IsRequired()
                    .HasColumnType("nvarchar2(450)");

                b.Property<DateTime>("CreatedAt")
                    .HasColumnType("timestamp");

                b.Property<DateTime?>("ModifiedAt")
                    .HasColumnType("timestamp");

                b.Property<string>("Type")
                    .IsRequired()
                    .HasColumnType("nvarchar2(250)")
                    .HasMaxLength(250);

                b.Property<string>("Value")
                    .HasColumnType("nclob");

                b.HasKey("Id");

                b.HasIndex("ClientId");

                b.ToTable("ClientClaims");
            });

            modelBuilder.Entity("Aguacongas.IdentityServer.Store.Entity.ClientGrantType", b =>
            {
                b.Property<string>("Id")
                    .HasColumnType("nvarchar2(450)");

                b.Property<string>("ClientId")
                    .IsRequired()
                    .HasColumnType("nvarchar2(450)");

                b.Property<DateTime>("CreatedAt")
                    .HasColumnType("timestamp");

                b.Property<string>("GrantType")
                    .IsRequired()
                    .HasColumnType("nvarchar2(250)")
                    .HasMaxLength(250);

                b.Property<DateTime?>("ModifiedAt")
                    .HasColumnType("timestamp");

                b.HasKey("Id");

                b.HasIndex("ClientId", "GrantType")
                    .IsUnique();

                b.ToTable("ClientGrantTypes");
            });

            modelBuilder.Entity("Aguacongas.IdentityServer.Store.Entity.ClientIdpRestriction", b =>
            {
                b.Property<string>("Id")
                    .HasColumnType("nvarchar2(450)");

                b.Property<string>("ClientId")
                    .IsRequired()
                    .HasColumnType("nvarchar2(450)");

                b.Property<DateTime>("CreatedAt")
                    .HasColumnType("timestamp");

                b.Property<DateTime?>("ModifiedAt")
                    .HasColumnType("timestamp");

                b.Property<string>("Provider")
                    .IsRequired()
                    .HasColumnType("nvarchar2(200)")
                    .HasMaxLength(200);

                b.HasKey("Id");

                b.HasIndex("ClientId", "Provider")
                    .IsUnique();

                b.ToTable("ClientIdpRestriction");
            });

            modelBuilder.Entity("Aguacongas.IdentityServer.Store.Entity.ClientLocalizedResource", b =>
            {
                b.Property<string>("Id")
                    .HasColumnType("nvarchar2(450)");

                b.Property<string>("ClientId")
                    .IsRequired()
                    .HasColumnType("nvarchar2(450)");

                b.Property<DateTime>("CreatedAt")
                    .HasColumnType("timestamp");

                b.Property<string>("CultureId")
                    .IsRequired()
                    .HasColumnType("nclob");

                b.Property<DateTime?>("ModifiedAt")
                    .HasColumnType("timestamp");

                b.Property<int>("ResourceKind")
                    .HasColumnType("int");

                b.Property<string>("Value")
                    .HasColumnType("nclob");

                b.HasKey("Id");

                b.HasIndex("ClientId");

                b.ToTable("ClientLocalizedResources");
            });

            modelBuilder.Entity("Aguacongas.IdentityServer.Store.Entity.ClientProperty", b =>
            {
                b.Property<string>("Id")
                    .HasColumnType("nvarchar2(450)");

                b.Property<string>("ClientId")
                    .IsRequired()
                    .HasColumnType("nvarchar2(450)");

                b.Property<DateTime>("CreatedAt")
                    .HasColumnType("timestamp");

                b.Property<string>("Key")
                    .IsRequired()
                    .HasColumnType("nvarchar2(250)")
                    .HasMaxLength(250);

                b.Property<DateTime?>("ModifiedAt")
                    .HasColumnType("timestamp");

                b.Property<string>("Value")
                    .HasColumnType("nvarchar2(2000)")
                    .HasMaxLength(2000);

                b.HasKey("Id");

                b.HasIndex("ClientId", "Key")
                    .IsUnique();

                b.ToTable("ClientProperties");
            });

            modelBuilder.Entity("Aguacongas.IdentityServer.Store.Entity.ClientScope", b =>
            {
                b.Property<string>("Id")
                    .HasColumnType("nvarchar2(450)");

                b.Property<string>("ClientId")
                    .IsRequired()
                    .HasColumnType("nvarchar2(450)");

                b.Property<DateTime>("CreatedAt")
                    .HasColumnType("timestamp");

                b.Property<DateTime?>("ModifiedAt")
                    .HasColumnType("timestamp");

                b.Property<string>("Scope")
                    .IsRequired()
                    .HasColumnType("nvarchar2(200)")
                    .HasMaxLength(200);

                b.HasKey("Id");

                b.HasIndex("ClientId", "Scope")
                    .IsUnique();

                b.ToTable("ClientScopes");
            });

            modelBuilder.Entity("Aguacongas.IdentityServer.Store.Entity.ClientSecret", b =>
            {
                b.Property<string>("Id")
                    .HasColumnType("nvarchar2(450)");

                b.Property<string>("ClientId")
                    .IsRequired()
                    .HasColumnType("nvarchar2(450)");

                b.Property<DateTime>("CreatedAt")
                    .HasColumnType("timestamp");

                b.Property<string>("Description")
                    .HasColumnType("nvarchar2(2000)")
                    .HasMaxLength(2000);

                b.Property<DateTime?>("Expiration")
                    .HasColumnType("timestamp");

                b.Property<DateTime?>("ModifiedAt")
                    .HasColumnType("timestamp");

                b.Property<string>("Type")
                    .IsRequired()
                    .HasColumnType("nvarchar2(250)")
                    .HasMaxLength(250);

                b.Property<string>("Value")
                    .IsRequired()
                    .HasColumnType("nclob");

                b.HasKey("Id");

                b.HasIndex("ClientId");

                b.ToTable("ClientSecrets");
            });

            modelBuilder.Entity("Aguacongas.IdentityServer.Store.Entity.ClientUri", b =>
            {
                b.Property<string>("Id")
                    .HasColumnType("nvarchar2(450)");

                b.Property<string>("ClientId")
                    .IsRequired()
                    .HasColumnType("nvarchar2(450)");

                b.Property<DateTime>("CreatedAt")
                    .HasColumnType("timestamp");

                b.Property<int>("Kind")
                    .HasColumnType("int");

                b.Property<DateTime?>("ModifiedAt")
                    .HasColumnType("timestamp");

                b.Property<string>("SanetizedCorsUri")
                    .HasColumnType("nvarchar2(2000)")
                    .HasMaxLength(2000);

                b.Property<string>("Uri")
                    .IsRequired()
                    .HasColumnType("nvarchar2(2000)")
                    .HasMaxLength(2000);

                b.HasKey("Id");

                b.HasIndex("ClientId", "Uri")
                    .IsUnique();

                b.ToTable("ClientUris");
            });

            modelBuilder.Entity("Aguacongas.IdentityServer.Store.Entity.Culture", b =>
            {
                b.Property<string>("Id")
                    .HasColumnType("nvarchar2(450)");

                b.Property<DateTime>("CreatedAt")
                    .HasColumnType("timestamp");

                b.Property<DateTime?>("ModifiedAt")
                    .HasColumnType("timestamp");

                b.HasKey("Id");

                b.ToTable("Cultures");

                b.HasData(
                    new
                    {
                        Id = "en",
                        CreatedAt = new DateTime(2020, 8, 13, 14, 4, 41, 437, DateTimeKind.Utc).AddTicks(7595)
                    });
            });

            modelBuilder.Entity("Aguacongas.IdentityServer.Store.Entity.ExternalClaimTransformation", b =>
            {
                b.Property<string>("Id")
                    .HasColumnType("nvarchar2(450)");

                b.Property<bool>("AsMultipleValues")
                    .HasColumnType("bool");

                b.Property<DateTime>("CreatedAt")
                    .HasColumnType("timestamp");

                b.Property<string>("FromClaimType")
                    .IsRequired()
                    .HasColumnType("nvarchar2(450)");

                b.Property<DateTime?>("ModifiedAt")
                    .HasColumnType("timestamp");

                b.Property<string>("Scheme")
                    .IsRequired()
                    .HasColumnType("nvarchar2(450)");

                b.Property<string>("ToClaimType")
                    .IsRequired()
                    .HasColumnType("nclob");

                b.HasKey("Id");

                b.HasIndex("Scheme", "FromClaimType")
                    .IsUnique();

                b.ToTable("ExternalClaimTransformations");
            });

            modelBuilder.Entity("Aguacongas.IdentityServer.Store.Entity.IdentityClaim", b =>
            {
                b.Property<string>("Id")
                    .HasColumnType("nvarchar2(450)");

                b.Property<DateTime>("CreatedAt")
                    .HasColumnType("timestamp");

                b.Property<string>("IdentityId")
                    .IsRequired()
                    .HasColumnType("nvarchar2(450)");

                b.Property<DateTime?>("ModifiedAt")
                    .HasColumnType("timestamp");

                b.Property<string>("Type")
                    .HasColumnType("nvarchar2(250)")
                    .HasMaxLength(250);

                b.HasKey("Id");

                b.HasIndex("IdentityId", "Type")
                    .IsUnique();

                b.ToTable("IdentityClaims");
            });

            modelBuilder.Entity("Aguacongas.IdentityServer.Store.Entity.IdentityLocalizedResource", b =>
            {
                b.Property<string>("Id")
                    .HasColumnType("nvarchar2(450)");

                b.Property<DateTime>("CreatedAt")
                    .HasColumnType("timestamp");

                b.Property<string>("CultureId")
                    .IsRequired()
                    .HasColumnType("nclob");

                b.Property<string>("IdentityId")
                    .IsRequired()
                    .HasColumnType("nvarchar2(450)");

                b.Property<DateTime?>("ModifiedAt")
                    .HasColumnType("timestamp");

                b.Property<int>("ResourceKind")
                    .HasColumnType("int");

                b.Property<string>("Value")
                    .HasColumnType("nclob");

                b.HasKey("Id");

                b.HasIndex("IdentityId");

                b.ToTable("IdentityLocalizedResources");
            });

            modelBuilder.Entity("Aguacongas.IdentityServer.Store.Entity.IdentityProperty", b =>
            {
                b.Property<string>("Id")
                    .HasColumnType("nvarchar2(450)");

                b.Property<DateTime>("CreatedAt")
                    .HasColumnType("timestamp");

                b.Property<string>("IdentityId")
                    .IsRequired()
                    .HasColumnType("nvarchar2(450)");

                b.Property<string>("Key")
                    .IsRequired()
                    .HasColumnType("nvarchar2(250)")
                    .HasMaxLength(250);

                b.Property<DateTime?>("ModifiedAt")
                    .HasColumnType("timestamp");

                b.Property<string>("Value")
                    .HasColumnType("nvarchar2(2000)")
                    .HasMaxLength(2000);

                b.HasKey("Id");

                b.HasIndex("IdentityId", "Key")
                    .IsUnique();

                b.ToTable("IdentityProperties");
            });

            modelBuilder.Entity("Aguacongas.IdentityServer.Store.Entity.IdentityResource", b =>
            {
                b.Property<string>("Id")
                    .HasColumnType("nvarchar2(450)");

                b.Property<DateTime>("CreatedAt")
                    .HasColumnType("timestamp");

                b.Property<string>("Description")
                    .HasColumnType("nvarchar2(1000)")
                    .HasMaxLength(1000);

                b.Property<string>("DisplayName")
                    .HasColumnType("nvarchar2(200)")
                    .HasMaxLength(200);

                b.Property<bool>("Emphasize")
                    .HasColumnType("bool");

                b.Property<bool>("Enabled")
                    .HasColumnType("bool");

                b.Property<DateTime?>("ModifiedAt")
                    .HasColumnType("timestamp");

                b.Property<bool>("NonEditable")
                    .HasColumnType("bool");

                b.Property<bool>("Required")
                    .HasColumnType("bool");

                b.Property<bool>("ShowInDiscoveryDocument")
                    .HasColumnType("bool");

                b.HasKey("Id");

                b.ToTable("Identities");
            });

            modelBuilder.Entity("Aguacongas.IdentityServer.Store.Entity.LocalizedResource", b =>
            {
                b.Property<string>("Id")
                    .HasColumnType("nvarchar2(450)");

                b.Property<string>("BaseName")
                    .HasColumnType("nclob");

                b.Property<DateTime>("CreatedAt")
                    .HasColumnType("timestamp");

                b.Property<string>("CultureId")
                    .IsRequired()
                    .HasColumnType("nvarchar2(450)");

                b.Property<string>("Key")
                    .IsRequired()
                    .HasColumnType("nclob");

                b.Property<string>("Location")
                    .HasColumnType("nclob");

                b.Property<DateTime?>("ModifiedAt")
                    .HasColumnType("timestamp");

                b.Property<string>("Value")
                    .HasColumnType("nclob");

                b.HasKey("Id");

                b.HasIndex("CultureId");

                b.ToTable("LocalizedResources");
            });

            modelBuilder.Entity("Aguacongas.IdentityServer.Store.Entity.ProtectResource", b =>
            {
                b.Property<string>("Id")
                    .HasColumnType("nvarchar2(450)");

                b.Property<DateTime>("CreatedAt")
                    .HasColumnType("timestamp");

                b.Property<string>("Description")
                    .HasColumnType("nvarchar2(1000)")
                    .HasMaxLength(1000);

                b.Property<string>("DisplayName")
                    .IsRequired()
                    .HasColumnType("nvarchar2(200)")
                    .HasMaxLength(200);

                b.Property<bool>("Enabled")
                    .HasColumnType("bool");

                b.Property<DateTime?>("ModifiedAt")
                    .HasColumnType("timestamp");

                b.Property<bool>("NonEditable")
                    .HasColumnType("bool");

                b.HasKey("Id");

                b.ToTable("Apis");
            });

            modelBuilder.Entity("Aguacongas.IdentityServer.Store.Entity.ApiApiScope", b =>
            {
                b.HasOne("Aguacongas.IdentityServer.Store.Entity.ProtectResource", "Api")
                    .WithMany("ApiScopes")
                    .HasForeignKey("ApiId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();

                b.HasOne("Aguacongas.IdentityServer.Store.Entity.ApiScope", "ApiScope")
                    .WithMany("Apis")
                    .HasForeignKey("ApiScopeId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();
            });

            modelBuilder.Entity("Aguacongas.IdentityServer.Store.Entity.ApiClaim", b =>
            {
                b.HasOne("Aguacongas.IdentityServer.Store.Entity.ProtectResource", "Api")
                    .WithMany("ApiClaims")
                    .HasForeignKey("ApiId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();
            });

            modelBuilder.Entity("Aguacongas.IdentityServer.Store.Entity.ApiLocalizedResource", b =>
            {
                b.HasOne("Aguacongas.IdentityServer.Store.Entity.ProtectResource", "Api")
                    .WithMany("Resources")
                    .HasForeignKey("ApiId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();
            });

            modelBuilder.Entity("Aguacongas.IdentityServer.Store.Entity.ApiProperty", b =>
            {
                b.HasOne("Aguacongas.IdentityServer.Store.Entity.ProtectResource", "Api")
                    .WithMany("Properties")
                    .HasForeignKey("ApiId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();
            });

            modelBuilder.Entity("Aguacongas.IdentityServer.Store.Entity.ApiScopeClaim", b =>
            {
                b.HasOne("Aguacongas.IdentityServer.Store.Entity.ApiScope", "ApiScope")
                    .WithMany("ApiScopeClaims")
                    .HasForeignKey("ApiScopeId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();
            });

            modelBuilder.Entity("Aguacongas.IdentityServer.Store.Entity.ApiScopeLocalizedResource", b =>
            {
                b.HasOne("Aguacongas.IdentityServer.Store.Entity.ApiScope", "ApiScope")
                    .WithMany("Resources")
                    .HasForeignKey("ApiScopeId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();
            });

            modelBuilder.Entity("Aguacongas.IdentityServer.Store.Entity.ApiScopeProperty", b =>
            {
                b.HasOne("Aguacongas.IdentityServer.Store.Entity.ApiScope", "ApiScope")
                    .WithMany("Properties")
                    .HasForeignKey("ApiScopeId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();
            });

            modelBuilder.Entity("Aguacongas.IdentityServer.Store.Entity.ApiSecret", b =>
            {
                b.HasOne("Aguacongas.IdentityServer.Store.Entity.ProtectResource", "Api")
                    .WithMany("Secrets")
                    .HasForeignKey("ApiId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();
            });

            modelBuilder.Entity("Aguacongas.IdentityServer.Store.Entity.ClientClaim", b =>
            {
                b.HasOne("Aguacongas.IdentityServer.Store.Entity.Client", "Client")
                    .WithMany("ClientClaims")
                    .HasForeignKey("ClientId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();
            });

            modelBuilder.Entity("Aguacongas.IdentityServer.Store.Entity.ClientGrantType", b =>
            {
                b.HasOne("Aguacongas.IdentityServer.Store.Entity.Client", "Client")
                    .WithMany("AllowedGrantTypes")
                    .HasForeignKey("ClientId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();
            });

            modelBuilder.Entity("Aguacongas.IdentityServer.Store.Entity.ClientIdpRestriction", b =>
            {
                b.HasOne("Aguacongas.IdentityServer.Store.Entity.Client", "Client")
                    .WithMany("IdentityProviderRestrictions")
                    .HasForeignKey("ClientId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();
            });

            modelBuilder.Entity("Aguacongas.IdentityServer.Store.Entity.ClientLocalizedResource", b =>
            {
                b.HasOne("Aguacongas.IdentityServer.Store.Entity.Client", "Client")
                    .WithMany("Resources")
                    .HasForeignKey("ClientId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();
            });

            modelBuilder.Entity("Aguacongas.IdentityServer.Store.Entity.ClientProperty", b =>
            {
                b.HasOne("Aguacongas.IdentityServer.Store.Entity.Client", "Client")
                    .WithMany("Properties")
                    .HasForeignKey("ClientId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();
            });

            modelBuilder.Entity("Aguacongas.IdentityServer.Store.Entity.ClientScope", b =>
            {
                b.HasOne("Aguacongas.IdentityServer.Store.Entity.Client", "Client")
                    .WithMany("AllowedScopes")
                    .HasForeignKey("ClientId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();
            });

            modelBuilder.Entity("Aguacongas.IdentityServer.Store.Entity.ClientSecret", b =>
            {
                b.HasOne("Aguacongas.IdentityServer.Store.Entity.Client", "Client")
                    .WithMany("ClientSecrets")
                    .HasForeignKey("ClientId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();
            });

            modelBuilder.Entity("Aguacongas.IdentityServer.Store.Entity.ClientUri", b =>
            {
                b.HasOne("Aguacongas.IdentityServer.Store.Entity.Client", "Client")
                    .WithMany("RedirectUris")
                    .HasForeignKey("ClientId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();
            });

            modelBuilder.Entity("Aguacongas.IdentityServer.Store.Entity.ExternalClaimTransformation", b =>
            {
                b.HasOne("Aguacongas.IdentityServer.EntityFramework.Store.SchemeDefinition", null)
                    .WithMany("ClaimTransformations")
                    .HasForeignKey("Scheme")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();
            });

            modelBuilder.Entity("Aguacongas.IdentityServer.Store.Entity.IdentityClaim", b =>
            {
                b.HasOne("Aguacongas.IdentityServer.Store.Entity.IdentityResource", "Identity")
                    .WithMany("IdentityClaims")
                    .HasForeignKey("IdentityId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();
            });

            modelBuilder.Entity("Aguacongas.IdentityServer.Store.Entity.IdentityLocalizedResource", b =>
            {
                b.HasOne("Aguacongas.IdentityServer.Store.Entity.IdentityResource", "Identity")
                    .WithMany("Resources")
                    .HasForeignKey("IdentityId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();
            });

            modelBuilder.Entity("Aguacongas.IdentityServer.Store.Entity.IdentityProperty", b =>
            {
                b.HasOne("Aguacongas.IdentityServer.Store.Entity.IdentityResource", "Identity")
                    .WithMany("Properties")
                    .HasForeignKey("IdentityId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();
            });

            modelBuilder.Entity("Aguacongas.IdentityServer.Store.Entity.LocalizedResource", b =>
            {
                b.HasOne("Aguacongas.IdentityServer.Store.Entity.Culture", "Culture")
                    .WithMany("Resources")
                    .HasForeignKey("CultureId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();
            });
#pragma warning restore 612, 618
        }
    }
}
