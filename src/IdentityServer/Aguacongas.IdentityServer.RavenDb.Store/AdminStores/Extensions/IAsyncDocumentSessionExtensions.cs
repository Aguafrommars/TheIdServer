// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store;
using Community.OData.Linq;
using Microsoft.OData.Edm;
using Raven.Client.Documents;
using Raven.Client.Documents.Linq;
using Raven.Client.Documents.Session.Loaders;
using System;
using System.Collections;
using System.Linq;
using System.Text;

namespace Aguacongas.IdentityServer.RavenDb.Store
{
    public static class IAsyncDocumentSessionExtensions
    {
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

        public static ODataQuery<T> GetODataQuery<T>(this IQueryable<T> query, PageRequest request, IEdmModel edmModel = null) where T : class
        {
            var odataQuery = query
                .OData(edmModel: edmModel);

            if (!string.IsNullOrEmpty(request?.Filter))
            {
                odataQuery = odataQuery.Filter(request.Filter);
            }
            if (!string.IsNullOrEmpty(request?.OrderBy))
            {
                odataQuery = odataQuery.OrderBy(request.OrderBy);
            }

            return odataQuery;
        }

        public static IQueryable<T> GetPage<T>(this ODataQuery<T> odataQuery, PageRequest request) where T : class
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

            if (property.PropertyType.IsAssignableTo(typeof(IEnumerable)))
            {
                pathBuilder.Append("[].Id");
                return pathBuilder;
            }

            if (property.PropertyType.GetProperty("Id") == null)
            {
                throw new InvalidOperationException($"{path} doesn't have a property named 'Id'.");
            }

            if (type.GetProperty($"{path}Id") == null)
            {
                throw new InvalidOperationException($"{type.Name} doesn't have a property named '{path}Id'.");
            }

            pathBuilder.Append("Id");
            return pathBuilder;
        }
    }
}
