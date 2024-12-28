using Duende.IdentityServer.Models;
using System;

namespace Aguacongas.IdentityServer.Store.Entity
{
    public static class UserSessionExtensions
    {
        public static ServerSideSession ToServerSideSession(this UserSession session)
        => new()
        {
            Created = session.Created,
            DisplayName = session.DisplayName,
            Expires = session.Expires.HasValue ? DateTime.SpecifyKind(session.Expires.Value, DateTimeKind.Unspecified) : null,
            Key = session.Id,
            Renewed = DateTime.SpecifyKind(session.Renewed, DateTimeKind.Unspecified),
            Scheme = session.Scheme,
            SessionId = session.SessionId,
            SubjectId = session.UserId,
            Ticket = session.Ticket,
        };
    }
}
