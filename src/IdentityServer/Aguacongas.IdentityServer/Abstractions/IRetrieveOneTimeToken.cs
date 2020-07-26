namespace Aguacongas.IdentityServer.Abstractions
{
    public interface IRetrieveOneTimeToken
    {
        string GetOneTimeToken(string id);
    }
}
