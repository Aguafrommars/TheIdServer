﻿// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using System;
using Aguacongas.IdentityServer.EntityFramework.Store;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Aguacongas.TheIdServer.PostgreSQL.Migrations.ConfigurationDb
{
    [DbContext(typeof(ConfigurationDbContext))]
    [Migration("20210503161726_AddRelyingPartyDescription")]
    partial class AddRelyingPartyDescription
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.5")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            modelBuilder.Entity("Aguacongas.IdentityServer.Store.Entity.ApiApiScope", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("ApiId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("ApiScopeId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnType("timestamp without time zone");

                    b.HasKey("Id");

                    b.HasIndex("ApiId");

                    b.HasIndex("ApiScopeId");

                    b.ToTable("ApiApiScope");
                });

            modelBuilder.Entity("Aguacongas.IdentityServer.Store.Entity.ApiClaim", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("ApiId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("character varying(250)");

                    b.HasKey("Id");

                    b.HasIndex("ApiId", "Type")
                        .IsUnique();

                    b.ToTable("ApiClaims");
                });

            modelBuilder.Entity("Aguacongas.IdentityServer.Store.Entity.ApiLocalizedResource", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("ApiId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("CultureId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int>("ResourceKind")
                        .HasColumnType("integer");

                    b.Property<string>("Value")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("ApiId");

                    b.ToTable("ApiLocalizedResources");
                });

            modelBuilder.Entity("Aguacongas.IdentityServer.Store.Entity.ApiProperty", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("ApiId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Key")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("character varying(250)");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Value")
                        .HasMaxLength(2000)
                        .HasColumnType("character varying(2000)");

                    b.HasKey("Id");

                    b.HasIndex("ApiId", "Key")
                        .IsUnique();

                    b.ToTable("ApiProperty");
                });

            modelBuilder.Entity("Aguacongas.IdentityServer.Store.Entity.ApiScope", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Description")
                        .HasMaxLength(1000)
                        .HasColumnType("character varying(1000)");

                    b.Property<string>("DisplayName")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)");

                    b.Property<bool>("Emphasize")
                        .HasColumnType("boolean");

                    b.Property<bool>("Enabled")
                        .HasColumnType("boolean");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<bool>("Required")
                        .HasColumnType("boolean");

                    b.Property<bool>("ShowInDiscoveryDocument")
                        .HasColumnType("boolean");

                    b.HasKey("Id");

                    b.ToTable("ApiScopes");
                });

            modelBuilder.Entity("Aguacongas.IdentityServer.Store.Entity.ApiScopeClaim", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("ApiScopeId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("character varying(250)");

                    b.HasKey("Id");

                    b.HasIndex("ApiScopeId", "Type")
                        .IsUnique();

                    b.ToTable("ApiScopeClaims");
                });

            modelBuilder.Entity("Aguacongas.IdentityServer.Store.Entity.ApiScopeLocalizedResource", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("ApiScopeId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("CultureId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int>("ResourceKind")
                        .HasColumnType("integer");

                    b.Property<string>("Value")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("ApiScopeId");

                    b.ToTable("ApiScopeLocalizedResources");
                });

            modelBuilder.Entity("Aguacongas.IdentityServer.Store.Entity.ApiScopeProperty", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("ApiScopeId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Key")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("character varying(250)");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Value")
                        .HasMaxLength(2000)
                        .HasColumnType("character varying(2000)");

                    b.HasKey("Id");

                    b.HasIndex("ApiScopeId", "Key")
                        .IsUnique();

                    b.ToTable("ApiScopeProperty");
                });

            modelBuilder.Entity("Aguacongas.IdentityServer.Store.Entity.ApiSecret", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("ApiId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Description")
                        .HasMaxLength(1000)
                        .HasColumnType("character varying(1000)");

                    b.Property<DateTime?>("Expiration")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("character varying(250)");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("ApiId");

                    b.ToTable("ApiSecrets");
                });

            modelBuilder.Entity("Aguacongas.IdentityServer.Store.Entity.Client", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<int>("AbsoluteRefreshTokenLifetime")
                        .HasColumnType("integer");

                    b.Property<int>("AccessTokenLifetime")
                        .HasColumnType("integer");

                    b.Property<int>("AccessTokenType")
                        .HasColumnType("integer");

                    b.Property<bool>("AllowAccessTokensViaBrowser")
                        .HasColumnType("boolean");

                    b.Property<bool>("AllowOfflineAccess")
                        .HasColumnType("boolean");

                    b.Property<bool>("AllowPlainTextPkce")
                        .HasColumnType("boolean");

                    b.Property<bool>("AllowRememberConsent")
                        .HasColumnType("boolean");

                    b.Property<bool>("AlwaysIncludeUserClaimsInIdToken")
                        .HasColumnType("boolean");

                    b.Property<bool>("AlwaysSendClientClaims")
                        .HasColumnType("boolean");

                    b.Property<int>("AuthorizationCodeLifetime")
                        .HasColumnType("integer");

                    b.Property<bool>("BackChannelLogoutSessionRequired")
                        .HasColumnType("boolean");

                    b.Property<string>("BackChannelLogoutUri")
                        .HasMaxLength(2000)
                        .HasColumnType("character varying(2000)");

                    b.Property<string>("ClientClaimsPrefix")
                        .HasMaxLength(250)
                        .HasColumnType("character varying(250)");

                    b.Property<string>("ClientName")
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)");

                    b.Property<string>("ClientUri")
                        .HasMaxLength(2000)
                        .HasColumnType("character varying(2000)");

                    b.Property<int?>("ConsentLifetime")
                        .HasColumnType("integer");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Description")
                        .HasMaxLength(1000)
                        .HasColumnType("character varying(1000)");

                    b.Property<int>("DeviceCodeLifetime")
                        .HasColumnType("integer");

                    b.Property<bool>("EnableLocalLogin")
                        .HasColumnType("boolean");

                    b.Property<bool>("Enabled")
                        .HasColumnType("boolean");

                    b.Property<bool>("FrontChannelLogoutSessionRequired")
                        .HasColumnType("boolean");

                    b.Property<string>("FrontChannelLogoutUri")
                        .HasMaxLength(2000)
                        .HasColumnType("character varying(2000)");

                    b.Property<int>("IdentityTokenLifetime")
                        .HasColumnType("integer");

                    b.Property<bool>("IncludeJwtId")
                        .HasColumnType("boolean");

                    b.Property<string>("LogoUri")
                        .HasMaxLength(2000)
                        .HasColumnType("character varying(2000)");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<bool>("NonEditable")
                        .HasColumnType("boolean");

                    b.Property<string>("PairWiseSubjectSalt")
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)");

                    b.Property<string>("PolicyUri")
                        .HasColumnType("text");

                    b.Property<string>("ProtocolType")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)");

                    b.Property<int>("RefreshTokenExpiration")
                        .HasColumnType("integer");

                    b.Property<int>("RefreshTokenUsage")
                        .HasColumnType("integer");

                    b.Property<Guid?>("RegistrationToken")
                        .HasColumnType("uuid");

                    b.Property<string>("RelyingPartyId")
                        .HasColumnType("text");

                    b.Property<bool>("RequireClientSecret")
                        .HasColumnType("boolean");

                    b.Property<bool>("RequireConsent")
                        .HasColumnType("boolean");

                    b.Property<bool>("RequirePkce")
                        .HasColumnType("boolean");

                    b.Property<int>("SlidingRefreshTokenLifetime")
                        .HasColumnType("integer");

                    b.Property<string>("TosUri")
                        .HasColumnType("text");

                    b.Property<bool>("UpdateAccessTokenClaimsOnRefresh")
                        .HasColumnType("boolean");

                    b.Property<string>("UserCodeType")
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<int?>("UserSsoLifetime")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("RelyingPartyId");

                    b.ToTable("Clients");
                });

            modelBuilder.Entity("Aguacongas.IdentityServer.Store.Entity.ClientClaim", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("ClientId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("character varying(250)");

                    b.Property<string>("Value")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("ClientId");

                    b.ToTable("ClientClaims");
                });

            modelBuilder.Entity("Aguacongas.IdentityServer.Store.Entity.ClientGrantType", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("ClientId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("GrantType")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("character varying(250)");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnType("timestamp without time zone");

                    b.HasKey("Id");

                    b.HasIndex("ClientId", "GrantType")
                        .IsUnique();

                    b.ToTable("ClientGrantTypes");
                });

            modelBuilder.Entity("Aguacongas.IdentityServer.Store.Entity.ClientIdpRestriction", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("ClientId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Provider")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)");

                    b.HasKey("Id");

                    b.HasIndex("ClientId", "Provider")
                        .IsUnique();

                    b.ToTable("ClientIdpRestriction");
                });

            modelBuilder.Entity("Aguacongas.IdentityServer.Store.Entity.ClientLocalizedResource", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("ClientId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("CultureId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int>("ResourceKind")
                        .HasColumnType("integer");

                    b.Property<string>("Value")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("ClientId");

                    b.ToTable("ClientLocalizedResources");
                });

            modelBuilder.Entity("Aguacongas.IdentityServer.Store.Entity.ClientProperty", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("ClientId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Key")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("character varying(250)");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Value")
                        .HasMaxLength(2000)
                        .HasColumnType("character varying(2000)");

                    b.HasKey("Id");

                    b.HasIndex("ClientId", "Key")
                        .IsUnique();

                    b.ToTable("ClientProperties");
                });

            modelBuilder.Entity("Aguacongas.IdentityServer.Store.Entity.ClientScope", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("ClientId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Scope")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)");

                    b.HasKey("Id");

                    b.HasIndex("ClientId", "Scope")
                        .IsUnique();

                    b.ToTable("ClientScopes");
                });

            modelBuilder.Entity("Aguacongas.IdentityServer.Store.Entity.ClientSecret", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("ClientId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Description")
                        .HasMaxLength(2000)
                        .HasColumnType("character varying(2000)");

                    b.Property<DateTime?>("Expiration")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("character varying(250)");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("ClientId");

                    b.ToTable("ClientSecrets");
                });

            modelBuilder.Entity("Aguacongas.IdentityServer.Store.Entity.ClientUri", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("ClientId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int>("Kind")
                        .HasColumnType("integer");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("SanetizedCorsUri")
                        .HasMaxLength(2000)
                        .HasColumnType("character varying(2000)");

                    b.Property<string>("Uri")
                        .IsRequired()
                        .HasMaxLength(2000)
                        .HasColumnType("character varying(2000)");

                    b.HasKey("Id");

                    b.HasIndex("ClientId", "Uri")
                        .IsUnique();

                    b.ToTable("ClientUris");
                });

            modelBuilder.Entity("Aguacongas.IdentityServer.Store.Entity.Culture", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnType("timestamp without time zone");

                    b.HasKey("Id");

                    b.ToTable("Cultures");

                    b.HasData(
                        new
                        {
                            Id = "en",
                            CreatedAt = new DateTime(2021, 5, 3, 16, 17, 25, 819, DateTimeKind.Utc).AddTicks(8476)
                        });
                });

            modelBuilder.Entity("Aguacongas.IdentityServer.Store.Entity.ExternalClaimTransformation", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<bool>("AsMultipleValues")
                        .HasColumnType("boolean");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("FromClaimType")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Scheme")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("ToClaimType")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("Scheme", "FromClaimType")
                        .IsUnique();

                    b.ToTable("ExternalClaimTransformations");
                });

            modelBuilder.Entity("Aguacongas.IdentityServer.Store.Entity.ExternalProvider", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text")
                        .HasColumnName("Scheme");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("DisplayName")
                        .HasColumnType("text");

                    b.Property<bool>("MapDefaultOutboundClaimType")
                        .HasColumnType("boolean");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("SerializedHandlerType")
                        .HasColumnType("text");

                    b.Property<string>("SerializedOptions")
                        .HasColumnType("text");

                    b.Property<bool>("StoreClaims")
                        .HasColumnType("boolean");

                    b.HasKey("Id");

                    b.ToTable("Providers");
                });

            modelBuilder.Entity("Aguacongas.IdentityServer.Store.Entity.IdentityClaim", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("IdentityId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Type")
                        .HasMaxLength(250)
                        .HasColumnType("character varying(250)");

                    b.HasKey("Id");

                    b.HasIndex("IdentityId", "Type")
                        .IsUnique();

                    b.ToTable("IdentityClaims");
                });

            modelBuilder.Entity("Aguacongas.IdentityServer.Store.Entity.IdentityLocalizedResource", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("CultureId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("IdentityId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int>("ResourceKind")
                        .HasColumnType("integer");

                    b.Property<string>("Value")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("IdentityId");

                    b.ToTable("IdentityLocalizedResources");
                });

            modelBuilder.Entity("Aguacongas.IdentityServer.Store.Entity.IdentityProperty", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("IdentityId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Key")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("character varying(250)");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Value")
                        .HasMaxLength(2000)
                        .HasColumnType("character varying(2000)");

                    b.HasKey("Id");

                    b.HasIndex("IdentityId", "Key")
                        .IsUnique();

                    b.ToTable("IdentityProperties");
                });

            modelBuilder.Entity("Aguacongas.IdentityServer.Store.Entity.IdentityResource", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Description")
                        .HasMaxLength(1000)
                        .HasColumnType("character varying(1000)");

                    b.Property<string>("DisplayName")
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)");

                    b.Property<bool>("Emphasize")
                        .HasColumnType("boolean");

                    b.Property<bool>("Enabled")
                        .HasColumnType("boolean");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<bool>("NonEditable")
                        .HasColumnType("boolean");

                    b.Property<bool>("Required")
                        .HasColumnType("boolean");

                    b.Property<bool>("ShowInDiscoveryDocument")
                        .HasColumnType("boolean");

                    b.HasKey("Id");

                    b.ToTable("Identities");
                });

            modelBuilder.Entity("Aguacongas.IdentityServer.Store.Entity.LocalizedResource", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("BaseName")
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("CultureId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Key")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Location")
                        .HasColumnType("text");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Value")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("CultureId");

                    b.ToTable("LocalizedResources");
                });

            modelBuilder.Entity("Aguacongas.IdentityServer.Store.Entity.ProtectResource", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Description")
                        .HasMaxLength(1000)
                        .HasColumnType("character varying(1000)");

                    b.Property<string>("DisplayName")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)");

                    b.Property<bool>("Enabled")
                        .HasColumnType("boolean");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<bool>("NonEditable")
                        .HasColumnType("boolean");

                    b.HasKey("Id");

                    b.ToTable("Apis");
                });

            modelBuilder.Entity("Aguacongas.IdentityServer.Store.Entity.RelyingParty", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<string>("DigestAlgorithm")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<byte[]>("EncryptionCertificate")
                        .HasColumnType("bytea");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("SamlNameIdentifierFormat")
                        .HasColumnType("text");

                    b.Property<string>("SignatureAlgorithm")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("TokenType")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("RelyingParties");
                });

            modelBuilder.Entity("Aguacongas.IdentityServer.Store.Entity.RelyingPartyClaimMapping", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("FromClaimType")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("RelyingPartyId")
                        .HasColumnType("text");

                    b.Property<string>("ToClaimType")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("RelyingPartyId");

                    b.ToTable("RelyingPartyClaimMappings");
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

                    b.Navigation("Api");

                    b.Navigation("ApiScope");
                });

            modelBuilder.Entity("Aguacongas.IdentityServer.Store.Entity.ApiClaim", b =>
                {
                    b.HasOne("Aguacongas.IdentityServer.Store.Entity.ProtectResource", "Api")
                        .WithMany("ApiClaims")
                        .HasForeignKey("ApiId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Api");
                });

            modelBuilder.Entity("Aguacongas.IdentityServer.Store.Entity.ApiLocalizedResource", b =>
                {
                    b.HasOne("Aguacongas.IdentityServer.Store.Entity.ProtectResource", "Api")
                        .WithMany("Resources")
                        .HasForeignKey("ApiId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Api");
                });

            modelBuilder.Entity("Aguacongas.IdentityServer.Store.Entity.ApiProperty", b =>
                {
                    b.HasOne("Aguacongas.IdentityServer.Store.Entity.ProtectResource", "Api")
                        .WithMany("Properties")
                        .HasForeignKey("ApiId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Api");
                });

            modelBuilder.Entity("Aguacongas.IdentityServer.Store.Entity.ApiScopeClaim", b =>
                {
                    b.HasOne("Aguacongas.IdentityServer.Store.Entity.ApiScope", "ApiScope")
                        .WithMany("ApiScopeClaims")
                        .HasForeignKey("ApiScopeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ApiScope");
                });

            modelBuilder.Entity("Aguacongas.IdentityServer.Store.Entity.ApiScopeLocalizedResource", b =>
                {
                    b.HasOne("Aguacongas.IdentityServer.Store.Entity.ApiScope", "ApiScope")
                        .WithMany("Resources")
                        .HasForeignKey("ApiScopeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ApiScope");
                });

            modelBuilder.Entity("Aguacongas.IdentityServer.Store.Entity.ApiScopeProperty", b =>
                {
                    b.HasOne("Aguacongas.IdentityServer.Store.Entity.ApiScope", "ApiScope")
                        .WithMany("Properties")
                        .HasForeignKey("ApiScopeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ApiScope");
                });

            modelBuilder.Entity("Aguacongas.IdentityServer.Store.Entity.ApiSecret", b =>
                {
                    b.HasOne("Aguacongas.IdentityServer.Store.Entity.ProtectResource", "Api")
                        .WithMany("Secrets")
                        .HasForeignKey("ApiId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Api");
                });

            modelBuilder.Entity("Aguacongas.IdentityServer.Store.Entity.Client", b =>
                {
                    b.HasOne("Aguacongas.IdentityServer.Store.Entity.RelyingParty", "RelyingParty")
                        .WithMany("Clients")
                        .HasForeignKey("RelyingPartyId");

                    b.Navigation("RelyingParty");
                });

            modelBuilder.Entity("Aguacongas.IdentityServer.Store.Entity.ClientClaim", b =>
                {
                    b.HasOne("Aguacongas.IdentityServer.Store.Entity.Client", "Client")
                        .WithMany("ClientClaims")
                        .HasForeignKey("ClientId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Client");
                });

            modelBuilder.Entity("Aguacongas.IdentityServer.Store.Entity.ClientGrantType", b =>
                {
                    b.HasOne("Aguacongas.IdentityServer.Store.Entity.Client", "Client")
                        .WithMany("AllowedGrantTypes")
                        .HasForeignKey("ClientId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Client");
                });

            modelBuilder.Entity("Aguacongas.IdentityServer.Store.Entity.ClientIdpRestriction", b =>
                {
                    b.HasOne("Aguacongas.IdentityServer.Store.Entity.Client", "Client")
                        .WithMany("IdentityProviderRestrictions")
                        .HasForeignKey("ClientId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Client");
                });

            modelBuilder.Entity("Aguacongas.IdentityServer.Store.Entity.ClientLocalizedResource", b =>
                {
                    b.HasOne("Aguacongas.IdentityServer.Store.Entity.Client", "Client")
                        .WithMany("Resources")
                        .HasForeignKey("ClientId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Client");
                });

            modelBuilder.Entity("Aguacongas.IdentityServer.Store.Entity.ClientProperty", b =>
                {
                    b.HasOne("Aguacongas.IdentityServer.Store.Entity.Client", "Client")
                        .WithMany("Properties")
                        .HasForeignKey("ClientId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Client");
                });

            modelBuilder.Entity("Aguacongas.IdentityServer.Store.Entity.ClientScope", b =>
                {
                    b.HasOne("Aguacongas.IdentityServer.Store.Entity.Client", "Client")
                        .WithMany("AllowedScopes")
                        .HasForeignKey("ClientId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Client");
                });

            modelBuilder.Entity("Aguacongas.IdentityServer.Store.Entity.ClientSecret", b =>
                {
                    b.HasOne("Aguacongas.IdentityServer.Store.Entity.Client", "Client")
                        .WithMany("ClientSecrets")
                        .HasForeignKey("ClientId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Client");
                });

            modelBuilder.Entity("Aguacongas.IdentityServer.Store.Entity.ClientUri", b =>
                {
                    b.HasOne("Aguacongas.IdentityServer.Store.Entity.Client", "Client")
                        .WithMany("RedirectUris")
                        .HasForeignKey("ClientId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Client");
                });

            modelBuilder.Entity("Aguacongas.IdentityServer.Store.Entity.ExternalClaimTransformation", b =>
                {
                    b.HasOne("Aguacongas.IdentityServer.Store.Entity.ExternalProvider", null)
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

                    b.Navigation("Identity");
                });

            modelBuilder.Entity("Aguacongas.IdentityServer.Store.Entity.IdentityLocalizedResource", b =>
                {
                    b.HasOne("Aguacongas.IdentityServer.Store.Entity.IdentityResource", "Identity")
                        .WithMany("Resources")
                        .HasForeignKey("IdentityId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Identity");
                });

            modelBuilder.Entity("Aguacongas.IdentityServer.Store.Entity.IdentityProperty", b =>
                {
                    b.HasOne("Aguacongas.IdentityServer.Store.Entity.IdentityResource", "Identity")
                        .WithMany("Properties")
                        .HasForeignKey("IdentityId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Identity");
                });

            modelBuilder.Entity("Aguacongas.IdentityServer.Store.Entity.LocalizedResource", b =>
                {
                    b.HasOne("Aguacongas.IdentityServer.Store.Entity.Culture", "Culture")
                        .WithMany("Resources")
                        .HasForeignKey("CultureId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Culture");
                });

            modelBuilder.Entity("Aguacongas.IdentityServer.Store.Entity.RelyingPartyClaimMapping", b =>
                {
                    b.HasOne("Aguacongas.IdentityServer.Store.Entity.RelyingParty", "RelyingParty")
                        .WithMany("ClaimMappings")
                        .HasForeignKey("RelyingPartyId");

                    b.Navigation("RelyingParty");
                });

            modelBuilder.Entity("Aguacongas.IdentityServer.Store.Entity.ApiScope", b =>
                {
                    b.Navigation("Apis");

                    b.Navigation("ApiScopeClaims");

                    b.Navigation("Properties");

                    b.Navigation("Resources");
                });

            modelBuilder.Entity("Aguacongas.IdentityServer.Store.Entity.Client", b =>
                {
                    b.Navigation("AllowedGrantTypes");

                    b.Navigation("AllowedScopes");

                    b.Navigation("ClientClaims");

                    b.Navigation("ClientSecrets");

                    b.Navigation("IdentityProviderRestrictions");

                    b.Navigation("Properties");

                    b.Navigation("RedirectUris");

                    b.Navigation("Resources");
                });

            modelBuilder.Entity("Aguacongas.IdentityServer.Store.Entity.Culture", b =>
                {
                    b.Navigation("Resources");
                });

            modelBuilder.Entity("Aguacongas.IdentityServer.Store.Entity.ExternalProvider", b =>
                {
                    b.Navigation("ClaimTransformations");
                });

            modelBuilder.Entity("Aguacongas.IdentityServer.Store.Entity.IdentityResource", b =>
                {
                    b.Navigation("IdentityClaims");

                    b.Navigation("Properties");

                    b.Navigation("Resources");
                });

            modelBuilder.Entity("Aguacongas.IdentityServer.Store.Entity.ProtectResource", b =>
                {
                    b.Navigation("ApiClaims");

                    b.Navigation("ApiScopes");

                    b.Navigation("Properties");

                    b.Navigation("Resources");

                    b.Navigation("Secrets");
                });

            modelBuilder.Entity("Aguacongas.IdentityServer.Store.Entity.RelyingParty", b =>
                {
                    b.Navigation("ClaimMappings");

                    b.Navigation("Clients");
                });
#pragma warning restore 612, 618
        }
    }
}
