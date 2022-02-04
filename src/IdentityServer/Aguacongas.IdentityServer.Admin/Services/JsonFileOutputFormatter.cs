// Project: Aguafrommars/TheIdServer
// Copyright (c) 2022 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Buffers;

namespace Aguacongas.IdentityServer.Admin.Services
{
    /// <summary>
    /// Format response in a json file
    /// </summary>
    /// <seealso cref="NewtonsoftJsonOutputFormatter" />
    public class JsonFileOutputFormatter : NewtonsoftJsonOutputFormatter
    {
        /// <summary>
        /// The supported content type
        /// </summary>
        public const string SupportedContentType = "application/json";

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonFileOutputFormatter"/> class.
        /// </summary>
        /// <param name="mvcOptions">The <see cref="T:Microsoft.AspNetCore.Mvc.MvcOptions" />.</param>
        public JsonFileOutputFormatter(MvcOptions mvcOptions) 
            : base(new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                NullValueHandling = NullValueHandling.Ignore,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            }, ArrayPool<char>.Shared, mvcOptions, null)
        {
            SerializerSettings.Converters.Add(new MetadataJsonConverter(SerializerSettings));
            SupportedMediaTypes.Clear();
            SupportedMediaTypes.Add(SupportedContentType);
        }

        /// <summary>
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        /// <inheritdoc />
        public override bool CanWriteResult(OutputFormatterCanWriteContext context)
        {
            return context.HttpContext.Request.Query.TryGetValue("format", out StringValues value) && value == "export";
        }
        /// <summary>
        /// Sets the headers on <see cref="T:Microsoft.AspNetCore.Http.HttpResponse" /> object.
        /// </summary>
        /// <param name="context">The formatter context associated with the call.</param>
        public override void WriteResponseHeaders(OutputFormatterWriteContext context)
        {
            var type = context.ObjectType;
            var fileName = type.Name;
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(PageResponse<>))
            {
                fileName = $"{type.GetGenericArguments()[0].Name}s";
            }
            if (context.Object is IEntityId entity)
            {
                fileName = $"{fileName}-{entity.Id}";
            }
            context.HttpContext.Response.Headers["Content-Disposition"] = $"attachment; filename=\"{fileName}.json\"";
            base.WriteResponseHeaders(context);
        }
    }
}
