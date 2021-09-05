using AutoMapper.Internal;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Aguacongas.IdentityServer.Store
{
    public static class EntityExtension
    {
        public static bool CorsMatch(this Uri cors, Uri uri)
        {
            cors = cors ?? throw new ArgumentNullException(nameof(cors));
            uri = uri ?? throw new ArgumentNullException(nameof(uri));

            return uri.Scheme.ToUpperInvariant() == cors.Scheme.ToUpperInvariant() &&
                uri.Host.ToUpperInvariant() == cors.Host.ToUpperInvariant() &&
                uri.Port == cors.Port;
        }

        public static void Copy<TEntity>(this TEntity from, TEntity to) where TEntity : Entity.IEntityId
        {
            var properties = typeof(TEntity).GetProperties()
                .Where(p => !p.PropertyType.ImplementsGenericInterface(typeof(ICollection<>)));
            foreach (var property in properties)
            {
                property.SetValue(to, property.GetValue(from));
            }
        }
    
    }
}
