// Project: Aguafrommars/TheIdServer
// Copyright (c) 2023 @Olivier Lefebvre
using System.Security.Cryptography;
using System.Text;

namespace System
{
    public static class StringExtensions
    {
        public static string Sha256(this string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }

            var bytes = Encoding.UTF8.GetBytes(value);
            var hash = SHA256.HashData(bytes);

            return Convert.ToBase64String(hash);
        }
    }
}
