// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Aguacongas.IdentityServer.Extensions;
using Raven.Client.Documents;
using Raven.Client.Documents.Linq;
using Raven.Client.Documents.Session;
using Raven.Client.Documents.Session.Loaders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.RavenDb.Store
{
    public static class IAsyncDocumentSessionExtensions
    {
        public static async Task<ICollection<TSubEntity>> GetSubEntitiesAsync<TSubEntity>(this IAsyncDocumentSession session, ICollection<TSubEntity> idList) where TSubEntity : IEntityId
        {
            idList ??= new List<TSubEntity>();
            var itemList = await session.LoadAsync<TSubEntity>(idList.Select(c => c.Id)).ConfigureAwait(false);            
            var foundList =  itemList.Where(i => i.Value != null).Select(i => i.Value).ToList();
            var collectionName = typeof(TSubEntity).Name.ToLowerInvariant();
            foreach(var item in foundList)
            {
                idList.RemoveEntityId($"{collectionName}/{item.Id}");
                idList.Add(item);
            }

            return idList;
        }

        public static string GetSubEntityParentIdName(this Type type)
            => $"{type.Name.Replace("ProtectResource", "Api").Replace("IdentityResource", "Identity")}Id";

        public static IIncludeBuilder<T> Expand<T>(this IIncludeBuilder<T> builder, string expand)
        {
            if (expand != null)
            {
                var type = typeof(T);
                var pathList = expand.Split(',');
                foreach (var path in pathList)
                {
                    var pathBuilder = GetIncludePath(type, path);
                    builder = builder.IncludeDocuments(pathBuilder.ToString());
                }
            }
            return builder;
        }

        public static IRavenQueryable<T> Expand<T>(this IRavenQueryable<T> query, string expand)
        {
            if (expand != null)
            {
                var type = typeof(T);
                var pathList = expand.Split(',');
                foreach (var path in pathList)
                {
                    var pathBuilder = GetIncludePath(type, path);
                    query = query.Include(pathBuilder.ToString());
                }
            }
            return query;
        }

        public static IAsyncRawDocumentQuery<T> GetPage<T>(this IAsyncRawDocumentQuery<T> odataQuery, PageRequest request) where T : class
        {
            var page = odataQuery.Skip(request.Skip ?? 0)
                .Take(request.Take.Value);

            return page;
        }

        private static StringBuilder GetIncludePath(Type type, string path)
        {
            var pathBuilder = new StringBuilder();
            pathBuilder.Append(path);
            var property = type.GetProperty(path);
            if (property == null)
            {
                throw new InvalidOperationException($"{path} is not a property of '{type.Name}'.");
            }

            if (property.PropertyType.ImplementsGenericInterface(typeof(ICollection<>)))
            {
                pathBuilder.Append("[].Id");
                return pathBuilder;
            }

            if (property.PropertyType.GetProperty("Id") == null)
            {
                throw new InvalidOperationException($"{path} doesn't have a property named 'Id'.");
            }

            pathBuilder.Append("Id");
            return pathBuilder;
        }
    }
}
