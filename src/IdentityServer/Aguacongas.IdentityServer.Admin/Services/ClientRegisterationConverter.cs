using Newtonsoft.Json;
using System;
using System.Linq;
using System.Collections.Generic;
using Aguacongas.IdentityServer.Admin.Models;
using Newtonsoft.Json.Linq;

namespace Aguacongas.IdentityServer.Admin.Services
{
    /// <summary>
    /// 
    /// </summary>
    public class ClientRegisterationConverter : JsonConverter<ClientRegisteration>
    {
        private readonly Type _type = typeof(ClientRegisteration);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="objectType"></param>
        /// <param name="existingValue"></param>
        /// <param name="hasExistingValue"></param>
        /// <param name="serializer"></param>
        /// <returns></returns>
        public override ClientRegisteration ReadJson(JsonReader reader, Type objectType, ClientRegisteration existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            existingValue = new ClientRegisteration();
            while (reader.Read())
            {
                if (reader.TokenType != JsonToken.PropertyName)
                {
                    continue;
                }
                var propertyInfo = ((string)reader.Value).Split('#');
                var propertyName = propertyInfo[0];
                var property = _type.GetProperties().FirstOrDefault(p => p.GetCustomAttributes(typeof(JsonPropertyAttribute), false)
                    .Any(a => a is JsonPropertyAttribute jsonProperty && jsonProperty.PropertyName == propertyName));

                if (property == null)
                {
                    continue;
                }

                if (property.PropertyType == typeof(IEnumerable<LocalizableProperty>))
                {
                    var value = property.GetValue(existingValue) as List<LocalizableProperty>;
                    if (value == null)
                    {
                        value = new List<LocalizableProperty>();
                    }
                    value.Add(new LocalizableProperty
                    {
                        Culture = propertyInfo.Length > 1 ? propertyInfo[1] : null,
                        Value = reader.ReadAsString()
                    });
                    property.SetValue(existingValue, value);
                    continue;
                }
                if (property.PropertyType == typeof(string))
                {
                    property.SetValue(existingValue, reader.ReadAsString());
                    continue;
                }
                if (property.PropertyType == typeof(IEnumerable<string>))
                {
                    var value = new List<string>();
                    reader.Read();
                    while(reader.Read() && reader.TokenType != JsonToken.EndArray)
                    {
                        value.Add(reader.Value as string);
                    }
                    property.SetValue(existingValue, value);
                }
            }

            return existingValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        /// <param name="serializer"></param>
        public override void WriteJson(JsonWriter writer, ClientRegisteration value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            var jObject = new JObject();

            foreach(var property in _type.GetProperties())
            {
                var jsonPropertyAttribute = property.GetCustomAttributes(typeof(JsonPropertyAttribute), false)
                    .FirstOrDefault(a => a is JsonPropertyAttribute jsonProperty) as JsonPropertyAttribute;
                var propertyName = jsonPropertyAttribute?.PropertyName ?? property.Name;
                if (property.PropertyType == typeof(IEnumerable<LocalizableProperty>))
                {
                    var propertyValues = property.GetValue(value) as IEnumerable<LocalizableProperty>;
                    if (propertyValues == null)
                    {
                        continue;
                    }
                    foreach(var propertyValue in propertyValues)
                    {
                        var name = string.IsNullOrEmpty(propertyValue.Culture) ? propertyName : $"{propertyName}#{propertyValue.Culture}";
                        jObject.Add(new JProperty(name, propertyValue.Value));
                    }
                    continue;
                }
                var v = property.GetValue(value);
                if (v == null)
                {
                    continue;
                }
                jObject.Add(new JProperty(propertyName, v));
            }

            jObject.WriteTo(writer);
        }
    }
}
