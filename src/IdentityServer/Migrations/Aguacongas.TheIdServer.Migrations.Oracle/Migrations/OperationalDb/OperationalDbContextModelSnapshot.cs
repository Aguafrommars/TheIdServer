﻿// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using System;
using Aguacongas.IdentityServer.EntityFramework.Store;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Aguacongas.TheIdServer.Oracle.Migrations.OperationalDb
{
    [DbContext(typeof(OperationalDbContext))]
    partial class OperationalDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.8");

            modelBuilder.Entity("Aguacongas.IdentityServer.KeysRotation.EntityFrameworkCore.KeyRotationKey", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("FriendlyName")
                        .HasColumnType("nclob");

                    b.Property<string>("Xml")
                        .HasColumnType("nclob");

                    b.HasKey("Id");

                    b.ToTable("KeyRotationKeys");
                });

            modelBuilder.Entity("Aguacongas.IdentityServer.Store.Entity.AuthorizationCode", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar2(450)");

                    b.Property<string>("ClientId")
                        .IsRequired()
                        .HasColumnType("nclob");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp");

                    b.Property<string>("Data")
                        .HasColumnType("nclob");

                    b.Property<DateTime?>("Expiration")
                        .HasColumnType("timestamp");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnType("timestamp");

                    b.Property<string>("SessionId")
                        .HasColumnType("nclob");

                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar2(200)")
                        .HasMaxLength(200);

                    b.HasKey("Id");

                    b.ToTable("AuthorizationCodes");
                });

            modelBuilder.Entity("Aguacongas.IdentityServer.Store.Entity.DeviceCode", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar2(450)");

                    b.Property<string>("ClientId")
                        .IsRequired()
                        .HasColumnType("nclob");

                    b.Property<string>("Code")
                        .HasColumnType("nvarchar2(200)")
                        .HasMaxLength(200);

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp");

                    b.Property<string>("Data")
                        .HasColumnType("nclob");

                    b.Property<DateTime>("Expiration")
                        .HasColumnType("timestamp");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnType("timestamp");

                    b.Property<string>("SubjectId")
                        .HasColumnType("nvarchar2(200)")
                        .HasMaxLength(200);

                    b.Property<string>("UserCode")
                        .HasColumnType("nvarchar2(200)")
                        .HasMaxLength(200);

                    b.HasKey("Id");

                    b.ToTable("DeviceCodes");
                });

            modelBuilder.Entity("Aguacongas.IdentityServer.Store.Entity.OneTimeToken", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar2(450)");

                    b.Property<string>("ClientId")
                        .IsRequired()
                        .HasColumnType("nclob");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp");

                    b.Property<string>("Data")
                        .HasColumnType("nclob");

                    b.Property<DateTime?>("Expiration")
                        .HasColumnType("timestamp");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnType("timestamp");

                    b.Property<string>("SessionId")
                        .HasColumnType("nclob");

                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar2(200)")
                        .HasMaxLength(200);

                    b.HasKey("Id");

                    b.ToTable("OneTimeTokens");
                });

            modelBuilder.Entity("Aguacongas.IdentityServer.Store.Entity.ReferenceToken", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar2(450)");

                    b.Property<string>("ClientId")
                        .IsRequired()
                        .HasColumnType("nclob");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp");

                    b.Property<string>("Data")
                        .HasColumnType("nclob");

                    b.Property<DateTime?>("Expiration")
                        .HasColumnType("timestamp");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnType("timestamp");

                    b.Property<string>("SessionId")
                        .HasColumnType("nclob");

                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar2(200)")
                        .HasMaxLength(200);

                    b.HasKey("Id");

                    b.ToTable("ReferenceTokens");
                });

            modelBuilder.Entity("Aguacongas.IdentityServer.Store.Entity.RefreshToken", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar2(450)");

                    b.Property<string>("ClientId")
                        .IsRequired()
                        .HasColumnType("nclob");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp");

                    b.Property<string>("Data")
                        .HasColumnType("nclob");

                    b.Property<DateTime?>("Expiration")
                        .HasColumnType("timestamp");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnType("timestamp");

                    b.Property<string>("SessionId")
                        .HasColumnType("nclob");

                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar2(200)")
                        .HasMaxLength(200);

                    b.HasKey("Id");

                    b.ToTable("RefreshTokens");
                });

            modelBuilder.Entity("Aguacongas.IdentityServer.Store.Entity.UserConsent", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar2(450)");

                    b.Property<string>("ClientId")
                        .IsRequired()
                        .HasColumnType("nclob");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp");

                    b.Property<string>("Data")
                        .HasColumnType("nclob");

                    b.Property<DateTime?>("Expiration")
                        .HasColumnType("timestamp");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnType("timestamp");

                    b.Property<string>("SessionId")
                        .HasColumnType("nclob");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar2(200)")
                        .HasMaxLength(200);

                    b.HasKey("Id");

                    b.ToTable("UserConstents");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.DataProtection.EntityFrameworkCore.DataProtectionKey", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("FriendlyName")
                        .HasColumnType("nclob");

                    b.Property<string>("Xml")
                        .HasColumnType("nclob");

                    b.HasKey("Id");

                    b.ToTable("DataProtectionKeys");
                });
#pragma warning restore 612, 618
        }
    }
}
