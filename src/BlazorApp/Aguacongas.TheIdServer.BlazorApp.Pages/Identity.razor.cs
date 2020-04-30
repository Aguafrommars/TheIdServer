using Aguacongas.IdentityServer.Store.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aguacongas.TheIdServer.BlazorApp.Pages
{
    public partial class Identity
    {
        protected override string Expand => "IdentityClaims,Properties";

        protected override bool NonEditable => Model.NonEditable;

        protected override string BackUrl => "identities";

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync().ConfigureAwait(false);
            AddEmpyClaimsTypes();
        }

        protected override IdentityResource Create()
        {
            return new IdentityResource
            {
                IdentityClaims = new List<IdentityClaim>(),
                Properties = new List<IdentityProperty>()
            };
        }

        protected override void SetNavigationProperty<TEntity>(TEntity entity)
        {
            if (entity is IIdentitySubEntity subEntity)
            {
                subEntity.IdentityId = Model.Id;
            }
        }

        protected override void SanetizeEntityToSaved<TEntity>(TEntity entity)
        {
            if (entity is IdentityResource identity)
            {
                identity.IdentityClaims = null;
                identity.Properties = null;
            }
        }

        private void AddEmpyClaimsTypes()
        {
            Model.IdentityClaims.Add(new IdentityClaim());
        }

        private void OnFilterChanged(string term)
        {
            Model.IdentityClaims = State.IdentityClaims
                .Where(c => c.Type != null && c.Type.Contains(term))
                .ToList();
            Model.Properties = State.Properties
                .Where(p => (p.Key != null && p.Key.Contains(term)) || (p.Value != null && p.Value.Contains(term)))
                .ToList();

            AddEmpyClaimsTypes();
        }
    }
}
