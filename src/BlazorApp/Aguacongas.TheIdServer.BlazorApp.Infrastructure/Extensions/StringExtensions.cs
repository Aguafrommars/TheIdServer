// Project: Aguafrommars/TheIdServer
// Copyright (c) 2022 @Olivier Lefebvre
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
            using (var sha = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(value);
                var hash = sha.ComputeHash(bytes);

                return Convert.ToBase64String(hash);
            }
        }
    }
}
