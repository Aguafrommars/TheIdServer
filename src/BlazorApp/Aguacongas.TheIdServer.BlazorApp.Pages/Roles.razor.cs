namespace Aguacongas.TheIdServer.BlazorApp.Pages
{
    public partial class Roles
    {
        protected override string SelectProperties => $"{nameof(Models.Role.Id)},{nameof(Models.Role.Name)}";

        protected override string ExportExpand => null;
    }
}
