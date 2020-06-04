using Aguacongas.IdentityServer.Store.Entity;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Globalization;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Aguacongas.TheIdServer.BlazorApp.Components
{
    public partial class EntityResourceTranslatation
    {
        private CultureInfo _cultureInfo = CultureInfo.InvariantCulture;

        [Parameter]
        public IEntityResource Resource { get; set; }

        [Parameter]
        public EventCallback<IEntityResource> DeleteResource { get; set; }

        [Parameter]
        public Expression<Func<object>> FieldExpression { get; set; }

        protected override void OnInitialized()
        {
            Localizer.OnResourceReady = () => InvokeAsync(StateHasChanged);
            if (Resource.CultureId != null)
            {
                _cultureInfo = CultureInfo.GetCultureInfo(Resource.CultureId);
            }
            base.OnInitialized();
        }

        private Task OnDeleteClick(MouseEventArgs args)
        {
            return DeleteResource.InvokeAsync(Resource);
        }

        private void CultureSelected(CultureInfo cultureInfo)
        {
            _cultureInfo = cultureInfo;
            Resource.CultureId = cultureInfo.Name;
        }
    }
}
