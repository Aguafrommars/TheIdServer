// Project: Aguafrommars/TheIdServer
// Copyright (c) 2025 @Olivier Lefebvre

using Microsoft.AspNetCore.Builder;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((hostingContext, configuration) =>
    configuration.MinimumLevel.Warning()
        .MinimumLevel.Override("IdentityModel", LogEventLevel.Debug)
        .MinimumLevel.Override("System.Net.Http", LogEventLevel.Information)
        .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
        .MinimumLevel.Override("Microsoft.AspNetCore.Authentication", LogEventLevel.Information)
        .MinimumLevel.Override("MvcDPoP", LogEventLevel.Debug)
        .Enrich.FromLogContext()
        .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}")
);

builder.AddWebClient();

var app = builder.Build();

app.UseWebClient();

await app.RunAsync().ConfigureAwait(false);
