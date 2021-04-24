// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.AspNetCore.Authentication;
using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.Admin.Filters
{
    class ExternalProviderFilter : IAsyncResultFilter
    {
        private readonly IAuthenticationSchemeOptionsSerializer _serializer;
        public ExternalProviderFilter(IAuthenticationSchemeOptionsSerializer serializer)
        {
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        }
        public Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            var result = context.Result as ObjectResult;
            var value = result?.Value;
            if (value is PageResponse<ExternalProvider> page)
            {
                foreach (var item in page.Items)
                {
                    SetKindName(item);
                }
            } else if (value is ExternalProvider provider)
            {
                SetKindName(provider);
            }
            return next();
        }

        private void SetKindName(ExternalProvider provider)
        {
            var hanlderType = _serializer.DeserializeType(provider.SerializedHandlerType);
            var optionsType = hanlderType.GetAuthenticationSchemeOptionsType();
            provider.KindName = optionsType.Name.Replace("Options", "");
        }
    }
}
