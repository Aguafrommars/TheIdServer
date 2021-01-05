﻿// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using System;
using Aguacongas.IdentityServer.EntityFramework.Store;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Aguacongas.TheIdServer.Sqlite.Migrations.ConfigurationDb
{
    [DbContext(typeof(ConfigurationDbContext))]
    [Migration("20200813140453_DynamicRegistration")]
    partial class DynamicRegistration
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.6");

            modelBuilder.Entity("Aguacongas.IdentityServer.EntityFramework.Store.SchemeDefinition", b =>
                {
                    b.Property<string>("Scheme")
                        .HasColumnType("TEXT");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("DisplayName")
                        .HasColumnType("TEXT");

                    b.Property<bool>("MapDefaultOutboundClaimType")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("SerializedHandlerType")
                        .HasColumnType("TEXT");

                    b.Property<string>("SerializedOptions")
                        .HasColumnType("TEXT");

                    b.Property<bool>("StoreClaims")
                        .HasColumnType("INTEGER");

                    b.HasKey("Scheme");

                    b.ToTable("Providers");
                });

            modelBuilder.Entity("Aguacongas.IdentityServer.Store.Entity.ApiApiScope", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<string>("ApiId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("ApiScopeId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("ApiId");

                    b.HasIndex("ApiScopeId");

                    b.ToTable("ApiApiScope");
                });

            modelBuilder.Entity("Aguacongas.IdentityServer.Store.Entity.ApiClaim", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<string>("ApiId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasMaxLength(250);

                    b.HasKey("Id");

                    b.HasIndex("ApiId", "Type")
                        .IsUnique();

                    b.ToTable("ApiClaims");
                });

            modelBuilder.Entity("Aguacongas.IdentityServer.Store.Entity.ApiLocalizedResource", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<string>("ApiId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("CultureId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnType("TEXT");

                    b.Property<int>("ResourceKind")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Value")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("ApiId");

                    b.ToTable("ApiLocalizedResources");
                });

            modelBuilder.Entity("Aguacongas.IdentityServer.Store.Entity.ApiProperty", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<string>("ApiId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("Key")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasMaxLength(250);

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("Value")
                        .HasColumnType("TEXT")
                        .HasMaxLength(2000);

                    b.HasKey("Id");

                    b.HasIndex("ApiId", "Key")
                        .IsUnique();

                    b.ToTable("ApiProperty");
                });

            modelBuilder.Entity("Aguacongas.IdentityServer.Store.Entity.ApiScope", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .HasColumnType("TEXT")
                        .HasMaxLength(1000);

                    b.Property<string>("DisplayName")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasMaxLength(200);

                    b.Property<bool>("Emphasize")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("Enabled")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnType("TEXT");

                    b.Property<bool>("Required")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("ShowInDiscoveryDocument")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("ApiScopes");
                });

            modelBuilder.Entity("Aguacongas.IdentityServer.Store.Entity.ApiScopeClaim", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<string>("ApiScopeId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasMaxLength(250);

                    b.HasKey("Id");

                    b.HasIndex("ApiScopeId", "Type")
                        .IsUnique();

                    b.ToTable("ApiScopeClaims");
                });

            modelBuilder.Entity("Aguacongas.IdentityServer.Store.Entity.ApiScopeLocalizedResource", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<string>("ApiScopeId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("CultureId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnType("TEXT");

                    b.Property<int>("ResourceKind")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Value")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("ApiScopeId");

                    b.ToTable("ApiScopeLocalizedResources");
                });

            modelBuilder.Entity("Aguacongas.IdentityServer.Store.Entity.ApiScopeProperty", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<string>("ApiScopeId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("Key")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasMaxLength(250);

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("Value")
                        .HasColumnType("TEXT")
                        .HasMaxLength(2000);

                    b.HasKey("Id");

                    b.HasIndex("ApiScopeId", "Key")
                        .IsUnique();

                    b.ToTable("ApiScopeProperty");
                });

            modelBuilder.Entity("Aguacongas.IdentityServer.Store.Entity.ApiSecret", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<string>("ApiId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .HasColumnType("TEXT")
                        .HasMaxLength(1000);

                    b.Property<DateTime?>("Expiration")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasMaxLength(250);

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("ApiId");

                    b.ToTable("ApiSecrets");
                });

            modelBuilder.Entity("Aguacongas.IdentityServer.Store.Entity.Client", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<int>("AbsoluteRefreshTokenLifetime")
                        .HasColumnType("INTEGER");

                    b.Property<int>("AccessTokenLifetime")
                        .HasColumnType("INTEGER");

                    b.Property<int>("AccessTokenType")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("AllowAccessTokensViaBrowser")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("AllowOfflineAccess")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("AllowPlainTextPkce")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("AllowRememberConsent")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("AlwaysIncludeUserClaimsInIdToken")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("AlwaysSendClientClaims")
                        .HasColumnType("INTEGER");

                    b.Property<int>("AuthorizationCodeLifetime")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("BackChannelLogoutSessionRequired")
                        .HasColumnType("INTEGER");

                    b.Property<string>("BackChannelLogoutUri")
                        .HasColumnType("TEXT")
                        .HasMaxLength(2000);

                    b.Property<string>("ClientClaimsPrefix")
                        .HasColumnType("TEXT")
                        .HasMaxLength(250);

                    b.Property<string>("ClientName")
                        .HasColumnType("TEXT")
                        .HasMaxLength(200);

                    b.Property<string>("ClientUri")
                        .HasColumnType("TEXT")
                        .HasMaxLength(2000);

                    b.Property<int?>("ConsentLifetime")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .HasColumnType("TEXT")
                        .HasMaxLength(1000);

                    b.Property<int>("DeviceCodeLifetime")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("EnableLocalLogin")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("Enabled")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("FrontChannelLogoutSessionRequired")
                        .HasColumnType("INTEGER");

                    b.Property<string>("FrontChannelLogoutUri")
                        .HasColumnType("TEXT")
                        .HasMaxLength(2000);

                    b.Property<int>("IdentityTokenLifetime")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IncludeJwtId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("LogoUri")
                        .HasColumnType("TEXT")
                        .HasMaxLength(2000);

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnType("TEXT");

                    b.Property<bool>("NonEditable")
                        .HasColumnType("INTEGER");

                    b.Property<string>("PairWiseSubjectSalt")
                        .HasColumnType("TEXT")
                        .HasMaxLength(200);

                    b.Property<string>("PolicyUri")
                        .HasColumnType("TEXT");

                    b.Property<string>("ProtocolType")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasMaxLength(200);

                    b.Property<int>("RefreshTokenExpiration")
                        .HasColumnType("INTEGER");

                    b.Property<int>("RefreshTokenUsage")
                        .HasColumnType("INTEGER");

                    b.Property<Guid?>("RegistrationToken")
                        .HasColumnType("TEXT");

                    b.Property<bool>("RequireClientSecret")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("RequireConsent")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("RequirePkce")
                        .HasColumnType("INTEGER");

                    b.Property<int>("SlidingRefreshTokenLifetime")
                        .HasColumnType("INTEGER");

                    b.Property<string>("TosUri")
                        .HasColumnType("TEXT");

                    b.Property<bool>("UpdateAccessTokenClaimsOnRefresh")
                        .HasColumnType("INTEGER");

                    b.Property<string>("UserCodeType")
                        .HasColumnType("TEXT")
                        .HasMaxLength(100);

                    b.Property<int?>("UserSsoLifetime")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("Clients");
                });

            modelBuilder.Entity("Aguacongas.IdentityServer.Store.Entity.ClientClaim", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<string>("ClientId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasMaxLength(250);

                    b.Property<string>("Value")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("ClientId");

                    b.ToTable("ClientClaims");
                });

            modelBuilder.Entity("Aguacongas.IdentityServer.Store.Entity.ClientGrantType", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<string>("ClientId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("GrantType")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasMaxLength(250);

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("ClientId", "GrantType")
                        .IsUnique();

                    b.ToTable("ClientGrantTypes");
                });

            modelBuilder.Entity("Aguacongas.IdentityServer.Store.Entity.ClientIdpRestriction", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<string>("ClientId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("Provider")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasMaxLength(200);

                    b.HasKey("Id");

                    b.HasIndex("ClientId", "Provider")
                        .IsUnique();

                    b.ToTable("ClientIdpRestriction");
                });

            modelBuilder.Entity("Aguacongas.IdentityServer.Store.Entity.ClientLocalizedResource", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<string>("ClientId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("CultureId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnType("TEXT");

                    b.Property<int>("ResourceKind")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Value")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("ClientId");

                    b.ToTable("ClientLocalizedResources");
                });

            modelBuilder.Entity("Aguacongas.IdentityServer.Store.Entity.ClientProperty", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<string>("ClientId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("Key")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasMaxLength(250);

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("Value")
                        .HasColumnType("TEXT")
                        .HasMaxLength(2000);

                    b.HasKey("Id");

                    b.HasIndex("ClientId", "Key")
                        .IsUnique();

                    b.ToTable("ClientProperties");
                });

            modelBuilder.Entity("Aguacongas.IdentityServer.Store.Entity.ClientScope", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<string>("ClientId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("Scope")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasMaxLength(200);

                    b.HasKey("Id");

                    b.HasIndex("ClientId", "Scope")
                        .IsUnique();

                    b.ToTable("ClientScopes");
                });

            modelBuilder.Entity("Aguacongas.IdentityServer.Store.Entity.ClientSecret", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<string>("ClientId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .HasColumnType("TEXT")
                        .HasMaxLength(2000);

                    b.Property<DateTime?>("Expiration")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasMaxLength(250);

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("ClientId");

                    b.ToTable("ClientSecrets");
                });

            modelBuilder.Entity("Aguacongas.IdentityServer.Store.Entity.ClientUri", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<string>("ClientId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<int>("Kind")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("SanetizedCorsUri")
                        .HasColumnType("TEXT")
                        .HasMaxLength(2000);

                    b.Property<string>("Uri")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasMaxLength(2000);

                    b.HasKey("Id");

                    b.HasIndex("ClientId", "Uri")
                        .IsUnique();

                    b.ToTable("ClientUris");
                });

            modelBuilder.Entity("Aguacongas.IdentityServer.Store.Entity.Culture", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Cultures");

                    b.HasData(
                        new
                        {
                            Id = "en",
                            CreatedAt = new DateTime(2020, 8, 13, 14, 4, 53, 252, DateTimeKind.Utc).AddTicks(6770)
                        });
                });

            modelBuilder.Entity("Aguacongas.IdentityServer.Store.Entity.ExternalClaimTransformation", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<bool>("AsMultipleValues")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("FromClaimType")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("Scheme")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("ToClaimType")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("Scheme", "FromClaimType")
                        .IsUnique();

                    b.ToTable("ExternalClaimTransformations");
                });

            modelBuilder.Entity("Aguacongas.IdentityServer.Store.Entity.IdentityClaim", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("IdentityId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("Type")
                        .HasColumnType("TEXT")
                        .HasMaxLength(250);

                    b.HasKey("Id");

                    b.HasIndex("IdentityId", "Type")
                        .IsUnique();

                    b.ToTable("IdentityClaims");
                });

            modelBuilder.Entity("Aguacongas.IdentityServer.Store.Entity.IdentityLocalizedResource", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("CultureId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("IdentityId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnType("TEXT");

                    b.Property<int>("ResourceKind")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Value")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("IdentityId");

                    b.ToTable("IdentityLocalizedResources");
                });

            modelBuilder.Entity("Aguacongas.IdentityServer.Store.Entity.IdentityProperty", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("IdentityId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Key")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasMaxLength(250);

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("Value")
                        .HasColumnType("TEXT")
                        .HasMaxLength(2000);

                    b.HasKey("Id");

                    b.HasIndex("IdentityId", "Key")
                        .IsUnique();

                    b.ToTable("IdentityProperties");
                });

            modelBuilder.Entity("Aguacongas.IdentityServer.Store.Entity.IdentityResource", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .HasColumnType("TEXT")
                        .HasMaxLength(1000);

                    b.Property<string>("DisplayName")
                        .HasColumnType("TEXT")
                        .HasMaxLength(200);

                    b.Property<bool>("Emphasize")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("Enabled")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnType("TEXT");

                    b.Property<bool>("NonEditable")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("Required")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("ShowInDiscoveryDocument")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("Identities");
                });

            modelBuilder.Entity("Aguacongas.IdentityServer.Store.Entity.LocalizedResource", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<string>("BaseName")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("CultureId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Key")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Location")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("Value")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("CultureId");

                    b.ToTable("LocalizedResources");
                });

            modelBuilder.Entity("Aguacongas.IdentityServer.Store.Entity.ProtectResource", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .HasColumnType("TEXT")
                        .HasMaxLength(1000);

                    b.Property<string>("DisplayName")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasMaxLength(200);

                    b.Property<bool>("Enabled")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnType("TEXT");

                    b.Property<bool>("NonEditable")
                        .HasColumnType("INTEGER");

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
