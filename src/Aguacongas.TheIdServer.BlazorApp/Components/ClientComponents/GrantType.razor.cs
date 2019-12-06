using Aguacongas.TheIdServer.BlazorApp.Models;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.TheIdServer.BlazorApp.Components.ClientComponents
{
    public partial class GrantType : AutoCompleteModel<Entity.ClientGrantType>
    {
        [Parameter]
        public Entity.Client Model { get; set; }

        protected override bool IsReadOnly => !string.IsNullOrEmpty(Entity.GrantType);

        protected override Task<IEnumerable<string>> GetFilteredValues(string term)
        {
            term ??= string.Empty;
            var grantTypes = GrantTypes.Instance;
            var allowedGrantType = Model.AllowedGrantTypes;
            var result = grantTypes.Where(kv => !allowedGrantType.Any(g => g.GrantType == kv.Key) &&
                !(allowedGrantType.Any(g => g.GrantType == "implicit") &&
                    (kv.Key == "authorization_code" || kv.Key == "hybrid")) &&
                !(allowedGrantType.Any(g => g.GrantType == "authorization_code") &&
                    (kv.Key == "hybrid" || kv.Key == "implicit")) &&
                !(allowedGrantType.Any(g => g.GrantType == "hybrid") &&
                    (kv.Key == "authorization_code" || kv.Key == "implicit")) &&
                (kv.Value.Contains(term) || kv.Key.Contains(term)))
            .Select(kv => kv.Key);
            return Task.FromResult(result);
        }

        protected override void SetValue(string inputValue)
        {
            Entity.GrantType = inputValue;
        }

        private string GetGrantTypeName()
        {
            return GetGrantTypeName(Entity.GrantType);
        }

        private string GetGrantTypeName(string key)
        {
            return GrantTypes.GetGrantTypeName(key);
        }
    }
}
