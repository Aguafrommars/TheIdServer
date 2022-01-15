// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store.Entity;
using Microsoft.JSInterop;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace Aguacongas.TheIdServer.BlazorApp.Components
{
    public partial class TokensGrid<T> where T: IGrant
    {
        private IEnumerable<T> Tokens => Collection?.Where(t => t.ClientId.Contains(HandleModificationState.FilterTerm) || t.Data.Contains(HandleModificationState.FilterTerm));

        private readonly JsonSerializerSettings _serializerOptions = new()
        {
            Formatting = Formatting.Indented,
        };
        private string _selectedData;

        private async Task ShowData(T row)
        {
            var token = JsonConvert.DeserializeObject<JObject>(row.Data);
            _selectedData = JsonConvert.SerializeObject(token, _serializerOptions);
            await _jsRuntime.InvokeVoidAsync("bootstrapInteropt.showModal", "token-data");
        }
    }
}
