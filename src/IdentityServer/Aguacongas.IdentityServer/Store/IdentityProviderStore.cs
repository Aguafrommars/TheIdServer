// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store.Entity;
using Community.OData.Linq;
using Microsoft.AspNetCore.Authentication;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.Store
{
    /// <summary>
    /// Identity provider store
    /// </summary>
    public class IdentityProviderStore : IIdentityProviderStore
    {
        private readonly IAuthenticationSchemeProvider _provider;

        /// <summary>
        /// Initializes a new instance of the <see cref="IdentityProviderStore"/> class.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <exception cref="ArgumentNullException">provider</exception>
        public IdentityProviderStore(IAuthenticationSchemeProvider provider)
        {
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
        }

        /// <summary>
        /// Gets a page of identity provider corresponding to the request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        public async Task<PageResponse<IdentityProvider>> GetAsync(PageRequest request)
        {
            var schenes = await _provider.GetAllSchemesAsync()
                .ConfigureAwait(false);

            var odataQuery = schenes
                .Where(s => IsAssignableToRemoteAuthenticationHandler(s.HandlerType))
                .Select(s => s.ToIdentityProvider())
                .AsQueryable()
                .OData();

            if (!string.IsNullOrEmpty(request.Filter))
            {
                odataQuery = odataQuery.Filter(request.Filter);
            }
            if (!string.IsNullOrEmpty(request.OrderBy))
            {
                odataQuery = odataQuery.OrderBy(request.OrderBy);
            }

            var count = odataQuery.Count();

            var page = odataQuery.Skip(request.Skip ?? 0).Take(request.Take ?? 1000);

            return new PageResponse<IdentityProvider>
            {
                Count = count,
                Items = page
            };
        }

        public async Task<IdentityProvider> GetAsync(string id)
        {
            var schenes = await _provider.GetAllSchemesAsync()
                .ConfigureAwait(false);
            return schenes.FirstOrDefault(s => s.Name == id && IsAssignableToRemoteAuthenticationHandler(s.HandlerType))
                .ToIdentityProvider();            
        }

        private static bool IsAssignableToRemoteAuthenticationHandler(Type givenType)
        {
            var remoteHandlerType = typeof(RemoteAuthenticationHandler<>);
            var interfaceTypes = givenType.GetInterfaces();

            if (givenType.IsGenericType && givenType.GetGenericTypeDefinition() == remoteHandlerType)
            {
                return true;
            }

            foreach (var it in interfaceTypes)
            {
                if (it.IsGenericType && it.GetGenericTypeDefinition() == remoteHandlerType)
                {
                    return true;
                }
            }

            Type baseType = givenType.BaseType;
            if (baseType == null)
            {
                return false;
            }

            return IsAssignableToRemoteAuthenticationHandler(baseType);
        }

    }
}
