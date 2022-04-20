using Aguacongas.IdentityServer.Store.Entity;

namespace Duende.IdentityServer.Models
{
    public static class ServerSideSessionExtensions
    {
        public static UserSession ToUserSession(this ServerSideSession session)
        => new()
        {
            Created = session.Created,
            DisplayName = session.DisplayName,
            Expires = session.Expires,
            Id = session.Key,
            Renewed = session.Renewed,
            Scheme = session.Scheme,
            SessionId = session.SessionId,
            Ticket = session.Ticket,
            UserId = session.SubjectId
        };
    }
}
