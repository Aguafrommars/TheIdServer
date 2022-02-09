// Project: Aguafrommars/TheIdServer
// Copyright (c) 2022 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store.Entity;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Aguacongas.IdentityServer.Admin.Services
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="JsonConverter" />
    public class MetadataJsonConverter : JsonConverter
    {
        private readonly JsonSerializer _internaleSerializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="MetadataJsonConverter"/> class.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <exception cref="ArgumentNullException">settings</exception>
        public MetadataJsonConverter(JsonSerializerSettings settings)
        {
            settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _internaleSerializer = JsonSerializer.Create(settings);
        }
        /// <summary>
        /// Gets a value indicating whether this <see cref="T:Newtonsoft.Json.JsonConverter" /> can read JSON.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this <see cref="T:Newtonsoft.Json.JsonConverter" /> can read JSON; otherwise, <c>false</c>.
        /// </value>
        public override bool CanRead
        {
            get { return false; }
        }

        /// <summary>
        /// Determines whether this instance can convert the specified object type.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <returns>
        /// <c>true</c> if this instance can convert the specified object type; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanConvert(Type objectType)
        {
            if (objectType.IsGenericType)
            {
                objectType = objectType.GetGenericArguments()[0];
            }
            return objectType.GetInterface(typeof(IEntityId).FullName) != null;
        }

        /// <summary>
        /// Reads the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T:Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="existingValue">The existing value of object being read.</param>
        /// <param name="serializer">The calling serializer.</param>
        /// <returns>
        /// The object value.
        /// </returns>
        /// <exception cref="NotImplementedException"></exception>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Writes the JSON representation of the object.
        /// </summary>
        /// <param name="writer">The <see cref="T:Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="value">The value.</param>
        /// <param name="serializer">The calling serializer.</param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            var t = JToken.FromObject(value, _internaleSerializer);

            if (t.Type != JTokenType.Object)
            {
                t.WriteTo(writer);
            }
            else
            {
                var o = (JObject)t;

                var type = value.GetType();
                if (type.IsGenericType)
                {
                    type = type.GetGenericArguments()[0];
                }

                o.AddFirst(new JProperty("_metadata", JObject.FromObject(new { typeName = type.AssemblyQualifiedName }, _internaleSerializer)));

                o.WriteTo(writer);
            }
        }
    }
}
