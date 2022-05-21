using Duende.IdentityServer.Models;

namespace Aguacongas.IdentityServer.Store.Entity
{
    public static class UserSessionExtensions
    {
        public static ServerSideSession ToServerSideSession(this UserSession session)
        => new()
        {
            Created = session.Created,
            DisplayName = session.DisplayName,
            Expires = session.Expires,
            Key = session.Id,
            Renewed = session.Renewed,
            Scheme = session.Scheme,
            SessionId = session.SessionId,
            SubjectId = session.UserId,
            Ticket = session.Ticket,
        };
    }
}
