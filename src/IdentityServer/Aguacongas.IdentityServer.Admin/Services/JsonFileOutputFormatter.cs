using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
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
        public const string SupportedContentType = "application/export-json";

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
            }, ArrayPool<char>.Shared, mvcOptions)
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
            return context.HttpContext.Request.Query.ContainsKey("format");
        }
        /// <summary>
        /// Sets the headers on <see cref="T:Microsoft.AspNetCore.Http.HttpResponse" /> object.
        /// </summary>
        /// <param name="context">The formatter context associated with the call.</param>
        public override void WriteResponseHeaders(OutputFormatterWriteContext context)
        {
            base.WriteResponseHeaders(context);
            context.HttpContext.Response.Headers["Content-Disposition"] = "attachment; filename=\"export.json\"";
        }
    }
}
