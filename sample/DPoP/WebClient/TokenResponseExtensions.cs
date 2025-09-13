// Project: Aguafrommars/TheIdServer
// Copyright (c) 2025 @Olivier Lefebvre

using System.Text.Json;

namespace WebClient
{
    public static class TokenResponseExtensions
    {
        public static string PrettyPrintJson(this string raw)
        {
            var doc = JsonDocument.Parse(raw).RootElement;
            return JsonSerializer.Serialize(doc, new JsonSerializerOptions { WriteIndented = true });
        }
    }
}