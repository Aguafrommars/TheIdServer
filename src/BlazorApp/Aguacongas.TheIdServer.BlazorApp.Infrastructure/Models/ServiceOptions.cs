namespace Aguacongas.TheIdServer.BlazorApp.Models
{
    public class ServiceOptions
    {
        public string Name { get; set; }
        public string Namespace { get; set; }
        public string Version { get; set; }
        public bool AutoGenerateServiceInstanceId { get; set; } = true;
        public string InstanceId { get; set; }
    }
}