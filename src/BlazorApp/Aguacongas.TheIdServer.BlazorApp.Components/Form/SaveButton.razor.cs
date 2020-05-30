using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace Aguacongas.TheIdServer.BlazorApp.Components.Form
{
    public partial class SaveButton
    {
        private bool _disabled;

        [CascadingParameter]
        public EditContext EditContext { get; set; }

        protected override void OnInitialized()
        {
            CssSubClass ??= "btn-primary";
            SetIsDisabled();
            EditContext.OnValidationStateChanged += EditContext_OnValidationStateChanged;
            EditContext.OnFieldChanged += EditContext_OnFieldChanged;
            EditContext.OnValidationRequested += EditContext_OnValidationRequested;
            Localizer.OnResourceReady = () => InvokeAsync(StateHasChanged);
            base.OnInitialized();
        }

        private void EditContext_OnValidationRequested(object sender, ValidationRequestedEventArgs e)
        {
            SetIsDisabled();
        }

        private void EditContext_OnFieldChanged(object sender, FieldChangedEventArgs e)
        {
            SetIsDisabled();
        }

        private void EditContext_OnValidationStateChanged(object sender, ValidationStateChangedEventArgs e)
        {
            SetIsDisabled();
        }

        private void SetIsDisabled()
        {
            _disabled = !EditContext.IsModified();
            InvokeAsync(StateHasChanged);
        }
    }
}
