using Aguacongas.TheIdServer.BlazorApp.Models;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.TheIdServer.BlazorApp.Components.ClientComponents
{
    public partial class GrantType
    {
        private bool _isReadOnly;

        [Parameter]
        public Entity.Client Model { get; set; }

        protected override bool IsReadOnly => _isReadOnly;

        protected override string PropertyName => "GrantType";


        protected override Task<IEnumerable<string>> GetFilteredValues(string term)
        {
            term ??= string.Empty;
            var grantTypes = GrantTypes.Instance;
            var allowedGrantTypes = Model.AllowedGrantTypes;

            var result = grantTypes.Where(kv => !allowedGrantTypes.Any(g => g.GrantType == kv.Key) &&
                    !(allowedGrantTypes.Any(g => g.GrantType == "implicit") &&
                        (kv.Key == "authorization_code" || kv.Key == "hybrid")) &&
                    !(allowedGrantTypes.Any(g => g.GrantType == "authorization_code") &&
                        (kv.Key == "hybrid" || kv.Key == "implicit")) &&
                    !(allowedGrantTypes.Any(g => g.GrantType == "hybrid") &&
                        (kv.Key == "authorization_code" || kv.Key == "implicit")) &&
                    (kv.Value.Contains(term) || kv.Key.Contains(term)))
                .Select(kv => kv.Key);
            return Task.FromResult(result);
        }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            _isReadOnly = Entity.Id != null;
        }

        private string GetGrantTypeName()
        {
            return GetGrantTypeName(Entity.GrantType);
        }

        private string GetGrantTypeName(string key)
        {
            return GrantTypes.GetGrantTypeName(key);
        }

        protected override void SetValue(string inputValue)
        {
            Entity.GrantType = inputValue;
        }
    }
}
