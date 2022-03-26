using Aguacongas.TheIdServer.Options.OpenTelemetry;
using Microsoft.Extensions.Configuration;
using OpenTelemetry.Trace;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddOpenTelemetry(this IServiceCollection services, IConfiguration configuration)
        {
            var options = configuration.Get<OpenTelemetryOptions>();
            return services.AddOpenTelemetry(options);
        }

        public static IServiceCollection AddOpenTelemetry(this IServiceCollection services, OpenTelemetryOptions options)
        => services.AddOpenTelemetryTracing(builder => 
        {
            builder.AddTheIdServerTelemetry(options);            
        });
    }
}
