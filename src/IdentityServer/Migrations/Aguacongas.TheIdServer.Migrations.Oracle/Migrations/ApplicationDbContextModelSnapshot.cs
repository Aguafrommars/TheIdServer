// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using System;
using Aguacongas.TheIdServer.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Aguacongas.TheIdServer.Oracle.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "5.0.5");

            modelBuilder.Entity("Aguacongas.IdentityServer.Store.Entity.UserClaim", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("ClaimType")
                        .HasColumnType("nclob");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nclob");

                    b.Property<string>("Issuer")
                        .HasColumnType("nclob");

                    b.Property<string>("OriginalType")
                        .HasColumnType("nclob");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar2(450)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Aguacongas.IdentityServer.Store.Entity.User", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar2(450)");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nclob");

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar2(256)")
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("bool");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("bool");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("NormalizedEmail")
                        .HasColumnType("nvarchar2(256)")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedUserName")
                        .HasColumnType("nvarchar2(256)")
                        .HasMaxLength(256);

                    b.Property<string>("PasswordHash")
                        .HasColumnType("nclob");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nclob");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("bool");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("nclob");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("bool");

                    b.Property<string>("UserName")
                        .HasColumnType("nvarchar2(256)")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("Aguacongas.IdentityServer.Store.Entity.Role", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar2(450)");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nclob");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar2(256)")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedName")
                        .HasColumnType("nvarchar2(256)")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasName("RoleNameIndex");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("Aguacongas.IdentityServer.Store.Entity.RoleClaim", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("ClaimType")
                        .HasColumnType("nclob");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nclob");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("nvarchar2(450)");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Aguacongas.IdentityServer.Store.Entity.UserLogin", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar2(450)");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar2(450)");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("nvarchar2(450)");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("nclob");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar2(450)");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Aguacongas.IdentityServer.Store.Entity.UserRole", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar2(450)");

                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar2(450)");

                    b.Property<string>("RoleId")
                        .HasColumnType("nvarchar2(450)");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Aguacongas.IdentityServer.Store.Entity.UserToken", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar2(450)");

                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar2(450)");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar2(450)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar2(450)");

                    b.Property<string>("Value")
                        .HasColumnType("nclob");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("Aguacongas.IdentityServer.EntityFramework.Store.UserClaim", b =>
                {
                    b.HasOne("Aguacongas.TheIdServer.Models.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Aguacongas.IdentityServer.Store.Entity.RoleClaim", b =>
                {
                    b.HasOne("Aguacongas.IdentityServer.Store.Entity.Role", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Aguacongas.IdentityServer.Store.Entity.UserLogin", b =>
                {
                    b.HasOne("Aguacongas.TheIdServer.Models.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Aguacongas.IdentityServer.Store.Entity.UserRole", b =>
                {
                    b.HasOne("Aguacongas.IdentityServer.Store.Entity.Role", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Aguacongas.TheIdServer.Models.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Aguacongas.IdentityServer.Store.Entity.UserToken", b =>
                {
                    b.HasOne("Aguacongas.TheIdServer.Models.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
