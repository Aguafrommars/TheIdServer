// Project: Aguafrommars/TheIdServer
// Copyright (c) 2022 @Olivier Lefebvre
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace Aguacongas.TheIdServer.MAUIApp.Services
{
    public class RemoteAuthenticationService : AuthenticationStateProvider, IRemoteAuthenticationService<RemoteAuthenticationState>
    {

        public Task<RemoteAuthenticationResult<RemoteAuthenticationState>> CompleteSignInAsync(RemoteAuthenticationContext<RemoteAuthenticationState> context)
        {
            return Success(context);
        }

        public Task<RemoteAuthenticationResult<RemoteAuthenticationState>> CompleteSignOutAsync(RemoteAuthenticationContext<RemoteAuthenticationState> context)
        {
            return Success(context);
        }

        public Task<RemoteAuthenticationResult<RemoteAuthenticationState>> SignInAsync(RemoteAuthenticationContext<RemoteAuthenticationState> context)
        {
            return Success(context);
        }

        public Task<RemoteAuthenticationResult<RemoteAuthenticationState>> SignOutAsync(RemoteAuthenticationContext<RemoteAuthenticationState> context)
        {
            return Success(context);
        }

        private static Task<RemoteAuthenticationResult<RemoteAuthenticationState>> Success(RemoteAuthenticationContext<RemoteAuthenticationState> context)
        {
            return Task.FromResult(new RemoteAuthenticationResult<RemoteAuthenticationState>
            {
                State = context.State,
                Status = RemoteAuthenticationStatus.Success
            });
        }

        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            throw null;
        }
    }
}
