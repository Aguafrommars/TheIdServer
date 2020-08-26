// Project: Aguafrommars/TheIdServer
// Copyright (c) 2020 @Olivier Lefebvre
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Collections.Generic;
using Aguacongas.IdentityServer.Admin.Models;
using Newtonsoft.Json.Linq;
using IdentityServer4.Models;
using System.Reflection;

namespace Aguacongas.IdentityServer.Admin.Services
{
    /// <summary>
    /// 
    /// </summary>
    public class ClientRegisterationConverter : JsonConverter<ClientRegisteration>
    {
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
            var properties = objectType.GetProperties();
            while (reader.Read())
            {
                if (reader.TokenType != JsonToken.PropertyName)
                {
                    continue;
                }
                var propertyInfo = ((string)reader.Value).Split('#');
                var propertyName = propertyInfo[0];
                var property = properties.FirstOrDefault(p => p.GetCustomAttributes(typeof(JsonPropertyAttribute), false)
                    .Any(a => a is JsonPropertyAttribute jsonProperty && jsonProperty.PropertyName == propertyName));

                if (property == null)
                {
                    continue;
                }

                if (property.PropertyType == typeof(IEnumerable<LocalizableProperty>))
                {
                    ReadLocalizableProperty(reader, existingValue, propertyInfo, property);
                    continue;
                }
                if (property.PropertyType == typeof(string))
                {
                    property.SetValue(existingValue, reader.ReadAsString());
                    continue;
                }
                if (property.PropertyType == typeof(IEnumerable<string>))
                {
                    ReadEnumarableString(reader, existingValue, property);
                    continue;
                }
                if (property.PropertyType == typeof(JsonWebKeys))
                {
                    DeserializeJwks(reader, existingValue, property);
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

            JObject jObject = SerializedProperties(value);

            jObject.WriteTo(writer);
        }

        private static void ReadEnumarableString(JsonReader reader, ClientRegisteration existingValue, PropertyInfo property)
        {
            var value = new List<string>();
            reader.Read();
            while (reader.Read() && reader.TokenType != JsonToken.EndArray)
            {
                value.Add(reader.Value as string);
            }
            property.SetValue(existingValue, value);
        }

        private static void ReadLocalizableProperty(JsonReader reader, ClientRegisteration existingValue, string[] propertyInfo, PropertyInfo property)
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
        }

        private static void DeserializeJwks(JsonReader reader, ClientRegisteration existingValue, System.Reflection.PropertyInfo property)
        {
            do
            {
                reader.Read();
            } while (reader.TokenType != JsonToken.StartArray);

            var keys = new List<JsonWebKey>();
            var propertyList = typeof(JsonWebKey).GetProperties();

            while (reader.TokenType != JsonToken.EndArray)
            {
                reader.Read();
                if (reader.TokenType != JsonToken.StartObject)
                {
                    break;
                }
                var jwk = new JsonWebKey();
                while (reader.TokenType != JsonToken.EndObject)
                {
                    reader.Read();
                    if (reader.TokenType != JsonToken.PropertyName)
                    {
                        continue;
                    }

                    var p = propertyList.FirstOrDefault(p => p.Name == (string)reader.Value);

                    if (p == null)
                    {
                        continue;
                    }

                    p.SetValue(jwk, reader.ReadAsString());
                }
                keys.Add(jwk);
            }
            property.SetValue(existingValue, new JsonWebKeys
            {
                Keys = keys
            });
        }

        private JObject SerializedProperties(object value)
        {
            var jObject = new JObject();
            var properties = value.GetType().GetProperties();

            foreach (var property in properties)
            {
                var jsonPropertyAttribute = property.GetCustomAttributes(typeof(JsonPropertyAttribute), false)
                    .FirstOrDefault(a => a is JsonPropertyAttribute jsonProperty) as JsonPropertyAttribute;
                var propertyName = jsonPropertyAttribute?.PropertyName ?? property.Name;
                if (property.PropertyType == typeof(IEnumerable<LocalizableProperty>))
                {
                    SerializeLocalizableProperty(property, value, propertyName, jObject);
                    continue;
                }
                var v = property.GetValue(value);
                if (v == null)
                {
                    continue;
                }
                if (v is JsonWebKeys jsonWebKeys)
                {
                    SerializeJwks(jObject, propertyName, jsonWebKeys);
                    continue;
                }
                jObject.Add(new JProperty(propertyName, v));
            }

            return jObject;
        }

        private void SerializeLocalizableProperty(PropertyInfo property, object value, string propertyName, JObject jObject)
        {
            var propertyValues = property.GetValue(value) as IEnumerable<LocalizableProperty>;
            if (propertyValues == null)
            {
                return;
            }
            foreach (var propertyValue in propertyValues)
            {
                var name = string.IsNullOrEmpty(propertyValue.Culture) ? propertyName : $"{propertyName}#{propertyValue.Culture}";
                jObject.Add(new JProperty(name, propertyValue.Value));
            }
        }

        private void SerializeJwks(JObject jObject, string propertyName, JsonWebKeys jsonWebKeys)
        {
            var array = new JArray();
            foreach (var key in jsonWebKeys.Keys)
            {
                array.Add(SerializedProperties(key));
            }
            var j = new JObject
                    {
                        new JProperty("keys", array)
                    };
            jObject.Add(new JProperty(propertyName, j));
        }
    }
}
