using Aguacongas.IdentityServer.Store.Entity;
using Microsoft.JSInterop;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Aguacongas.TheIdServer.BlazorApp.Components
{
    public partial class TokensGrid<T> where T: IGrant
    {
        private readonly JsonSerializerSettings _serializerOptions = new JsonSerializerSettings
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
