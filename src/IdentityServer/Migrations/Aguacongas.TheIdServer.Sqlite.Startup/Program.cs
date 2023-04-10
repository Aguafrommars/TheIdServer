// Project: Aguafrommars/TheIdServer
// Copyright (c) 2023 @Olivier Lefebvre
using Aguacongas.IdentityServer.EntityFramework.Store;
using Aguacongas.TheIdServer.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration(builder => builder.AddJsonFile("appsettings.json"))
    .ConfigureServices((hostContext, services) =>
    {
        var cn = hostContext.Configuration.GetConnectionString("db");

        Action<DbContextOptionsBuilder> optionsAction = options => options.UseSqlite(cn, options => options.MigrationsAssembly("Aguacongas.TheIdServer.Migrations.Sqlite"));
        services.AddDbContext<ApplicationDbContext>(optionsAction)
            .AddDbContext<ConfigurationDbContext>(optionsAction)
            .AddDbContext<OperationalDbContext>(optionsAction);
    });

var app = host.Build();

await app.RunAsync();