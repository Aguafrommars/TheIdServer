using System;

namespace Aguacongas.IdentityServer.Store.Entity
{
    public interface IGrant : IAuditable
    {
        Client Client { get; set; }
        string Data { get; set; }
        string Id { get; set; }
        string SubjectId { get; set; }
    }
}