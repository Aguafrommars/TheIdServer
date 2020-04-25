using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Community.OData.Linq;
using Community.OData.Linq.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.OData.Edm;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.Admin.Filters
{
    class SelectFilter : IAsyncResultFilter
    {
        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            var controlerType = context.Controller.GetType();
            var result = context.Result as ObjectResult;
            var value = result?.Value;
            var query = context.HttpContext.Request.Query;
            if (!context.Cancel &&
                value != null &&
                query.ContainsKey("select") &&
                controlerType.FullName
                .StartsWith("Aguacongas.IdentityServer.Admin.GenericApiController",
                    StringComparison.Ordinal))
            {
                var valueType = value.GetType();
                var entityType = controlerType.GetGenericArguments()[0];
                var pageResponseType = typeof(PageResponse<>).MakeGenericType(entityType);
                if (pageResponseType == valueType)
                {
                    var items = pageResponseType.GetProperty("Items").GetValue(value);
                    var selectResultType = typeof(SelectResult<>).MakeGenericType(entityType);
                    var selectResult = selectResultType.GetConstructors()[0]
                        .Invoke(Array.Empty<object>()) as SelectResult;
                    var pageResponse = new SelectedPageResponse
                    {
                        Items = selectResult.Select(items, query["select"], query["expand"]),
                        Count = (int)pageResponseType.GetProperty("Count").GetValue(value)
                    };
                    result.Value = pageResponse;
                }
            }

            await next().ConfigureAwait(false);
        }

        class SelectResult<T>: SelectResult
        {
            [SuppressMessage("Major Code Smell", "S2743:Static fields should not be used in generic types", Justification = "Won't fix")]
            private readonly static CamelCasePropertyNamesContractResolver _resolver = new CamelCasePropertyNamesContractResolver();
            public override JToken Select(object items, string select, string expand)
            {
                return Select(items as IEnumerable<T>, select, expand);
            }

            private JToken Select(IEnumerable<T> items, string select, string expand)
            {
                if (typeof(T) == typeof(ExternalProvider))
                {
                    var list = ((IEnumerable<ExternalProvider>)items).Select(e => new WrapProvider(e));
                    return list.AsQueryable().OData(edmModel: WrapProvider.GetEdmModel()).SelectExpand(select, expand).ToJson(options => options.ContractResolver = _resolver);
                }
                return items.AsQueryable().OData().SelectExpand(select, expand).ToJson(options => options.ContractResolver = _resolver);
            }
        }

        abstract class SelectResult
        {
            public abstract JToken Select(object items, string select, string expand);
        }

        class SelectedPageResponse
        {
            public int Count { get; set; }

            public JToken Items { get; set; }
        }

        class WrapProvider
        {
            private static IEdmModel _edmModel;
            public static IEdmModel GetEdmModel()
            {
                if (_edmModel != null)
                {
                    return _edmModel;
                }
                var builder = new ODataConventionModelBuilder();
                var entitySet = builder.EntitySet<WrapProvider>(typeof(WrapProvider).Name);
                var entityType = entitySet.EntityType;
                entityType.HasKey(e => e.Id);
                _edmModel =  builder.GetEdmModel();
                return _edmModel;
            }
            public WrapProvider(ExternalProvider provider)
            {
                Id = provider.Id;
                DisplayName = provider.DisplayName;
                KindName = provider.KindName;
            }

            public string Id { get; set; }
            public string DisplayName { get; set; }
            public string KindName { get; set; }
        }
    }
}
