using System;
using System.Collections.Generic;
using System.Security.Cryptography.Xml;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Aguacongas.TheIdServer.BlazorApp.Services
{
    public class JsonRoleConverter : JsonConverter<IEnumerable<string>>
    {
        public override IEnumerable<string> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var isArray = reader.TokenType == JsonTokenType.StartArray;
            return isArray ? ReadArray(ref reader) : new string[] { reader.GetString() };
        }


        public override void Write(Utf8JsonWriter writer, IEnumerable<string> value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        private IEnumerable<string> ReadArray(ref Utf8JsonReader reader)
        {
            var roles = new List<string>();
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndArray)
                {
                    break;
                }

                roles.Add(reader.GetString());
            }

            return roles;
        }
    }
}
