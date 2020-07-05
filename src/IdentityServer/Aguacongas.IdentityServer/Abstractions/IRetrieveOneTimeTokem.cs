namespace Aguacongas.IdentityServer.Abstractions
{
    public interface IRetrieveOneTimeTokem
    {
        string GetOneTimeToken(string id);
    }
}
