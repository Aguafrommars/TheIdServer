// Project: Aguafrommars/TheIdServer
// Copyright (c) 2023 @Olivier Lefebvre
using IdentityModel;
using Microsoft.AspNetCore.Authentication;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;

namespace Aguacongas.TheIdServer.UI
{
    public class DiagnosticsViewModel
    {
        public DiagnosticsViewModel(AuthenticateResult result)
        {
            AuthenticateResult = result;

            if (result.Properties?.Items is null || !result.Properties.Items.TryGetValue("client_list", out var encoded))
            {
                return;
            }

            var bytes = Base64Url.Decode(encoded!);
            var value = Encoding.UTF8.GetString(bytes);

            Clients = JsonConvert.DeserializeObject<string[]>(value);
        }

        public AuthenticateResult? AuthenticateResult { get; }
        public IEnumerable<string>? Clients { get; } = new List<string>();
    }
}