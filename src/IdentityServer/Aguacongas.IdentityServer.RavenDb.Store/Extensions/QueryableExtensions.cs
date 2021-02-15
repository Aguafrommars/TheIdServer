// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store;
using Community.OData.Linq;
using Microsoft.OData.Edm;
using System.Linq;

namespace Microsoft.EntityFrameworkCore
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> Expand<T>(this IQueryable<T> query, string expand) where T: class
        {
            if (expand != null)
            {
                var pathList = expand.Split(',');
                foreach (var path in pathList)
                {
                    query = query.Include(path.Trim().Replace('/', '.'));
                }
            }

            return query;
        }

        public static ODataQuery<T> GetODataQuery<T>(this IQueryable<T> query, PageRequest request, IEdmModel edmModel = null) where T: class
        {
            var odataQuery = query
                .Expand(request?.Expand)
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
    }
}
