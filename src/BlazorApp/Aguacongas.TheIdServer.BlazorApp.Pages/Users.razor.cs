namespace Aguacongas.TheIdServer.BlazorApp.Pages
{
    public partial class Users
    {
        protected override string SelectProperties => $"{nameof(Models.User.Id)},{nameof(Models.User.UserName)}";

        protected override string ExportExpand => null;
    }
}
