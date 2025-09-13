// Project: Aguafrommars/TheIdServer
// Copyright (c) 2025 @Olivier Lefebvre
using Aguacongas.TheIdServer.Options.OpenTelemetry;
using Microsoft.Extensions.Configuration;
using OpenTelemetry.Trace;
using OpenTelemetry.Metrics;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddOpenTelemetry(this IServiceCollection services, IConfiguration configuration)
        {
            var options = configuration.Get<OpenTelemetryOptions>();
            if (options is null)
            {
                return services;
            }
            return services.AddOpenTelemetry(options);
        }

        public static IServiceCollection AddOpenTelemetry(this IServiceCollection services, OpenTelemetryOptions options) {
            services.AddOpenTelemetry().WithTracing(builder => builder.AddTheIdServerTraces(options))
                .WithMetrics(builder => builder.AddTheIdServerMetrics(options));
            return services;
        }
    }
}
