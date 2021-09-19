namespace Microsoft.Extensions.DependencyInjection
{
    public static class MvcBuilderExtensions
    {
        public static IMvcBuilder AddTheIdServer(this IMvcBuilder services)
        {
            return services.AddApplicationPart(typeof(MvcBuilderExtensions).Assembly);
        }
    }
}
