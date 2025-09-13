// Project: Aguafrommars/TheIdServer
// Copyright (c) 2025 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store.Entity;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aguacongas.TheIdServer.BlazorApp.Pages.User.Components
{
    public partial class Sessions
    {
        private IEnumerable<UserSession> UserSessions => Collection.Where(t => t.SessionId.Contains(HandleModificationState.FilterTerm) || t.DisplayName.Contains(HandleModificationState.FilterTerm));

        private readonly JsonSerializerSettings _serializerOptions = new()
        {
            Formatting = Formatting.Indented,
        };

        private string _selectedData;

        private ValueTask ShowData(UserSession row)
        {
            var token = JsonConvert.DeserializeObject<JObject>(row.Ticket);
            _selectedData = JsonConvert.SerializeObject(token, _serializerOptions);
            return _jsRuntime.InvokeVoidAsync("bootstrapInteropt.showModal", "token-data");
        }
    }
}
