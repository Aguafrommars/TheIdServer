// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.AspNetCore.Authentication;
using Aguacongas.IdentityServer.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.TheIdServer.BlazorApp.Models
{
    public class ExternalProvider : ExternalProvider<RemoteAuthenticationOptions>
    {

    }

    public class ExternalProvider<TOptions> : Entity.ExternalProvider, ICloneable<ExternalProvider>, IExternalProvider<TOptions> where TOptions : RemoteAuthenticationOptions
    {
        [JsonIgnore]
        public virtual TOptions Options { get; set; }

        [JsonIgnore]
        public IEnumerable<Entity.ExternalProviderKind> Kinds { get; set; }

        [JsonIgnore]
        public TOptions DefaultOptions
        {
            get
            {
                var optionsType = GetOptionsType(this);
                return Deserialize(Kinds.First(k => k.KindName == KindName).SerializedDefaultOptions, optionsType);
            }
        }

        public override string SerializedHandlerType
        {
            get => Kinds.First(k => k.KindName == KindName).SerializedHandlerType;
            set => base.SerializedHandlerType = value;
        }

        public ExternalProvider Clone()
        {
            return MemberwiseClone() as ExternalProvider;
        }

        public static ExternalProvider FromEntity(Entity.ExternalProvider externalProvider)
        {
            if (string.IsNullOrEmpty(externalProvider.KindName))
            {
                var handlerTypeName = JsonSerializer.Deserialize<HandlerType>(externalProvider.SerializedHandlerType);
                externalProvider.KindName = handlerTypeName.Name.Split('.').Last().Replace("Handler", "");
            }
            var optionsType = GetOptionsType(externalProvider);
            return new ExternalProvider
            {
                DisplayName = externalProvider.DisplayName,
                Id = externalProvider.Id,
                KindName = externalProvider.KindName,
                Options = Deserialize(externalProvider.SerializedOptions, optionsType),
                StoreClaims = externalProvider.StoreClaims,
                MapDefaultOutboundClaimType = externalProvider.MapDefaultOutboundClaimType,
                ClaimTransformations = externalProvider.ClaimTransformations
            };
        }

        private static TOptions Deserialize(string options, Type optionsType)
        {
            return JsonSerializer.Deserialize(options, optionsType) as TOptions;
        }

        private static Type GetOptionsType(Entity.ExternalProvider externalProvider)
        {            
            var typeName = $"{typeof(RemoteAuthenticationOptions).Namespace}.{externalProvider.KindName}Options";
            var assembly = AppDomain.CurrentDomain.GetAssemblies().First(a => a.GetType(typeName) != null);
            return assembly.GetType(typeName);
        }

        class HandlerType
        {
            public string Name { get; set; }
        }
    }
}
