// Project: Aguafrommars/TheIdServer
// Copyright (c) 2022 @Olivier Lefebvre
using Aguacongas.TheIdServer;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System.Linq;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((hostingContext, configuration) =>
                        configuration.ReadFrom.Configuration(hostingContext.Configuration));

var configuration = builder.Configuration;

var services = builder.Services;

services.AddTheIdServer(configuration);

var seed = args.Any(x => x == "/seed");
if (seed)
{
    args = args.Except(new[] { "/seed" }).ToArray();
}

var app = builder.Build();

if (seed)
{
    var config = app.Services.GetRequiredService<IConfiguration>();
    SeedData.EnsureSeedData(config, app.Services);
    return;
}

app.UseTheIdServer(app.Environment, configuration);

app.Run();
