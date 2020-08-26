// Project: Aguafrommars/TheIdServer
// Copyright (c) 2020 @Olivier Lefebvre
using Aguacongas.TheIdServer.BlazorApp.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using System.Threading.Tasks;

namespace Aguacongas.TheIdServer.BlazorApp.Components.Form
{
    public partial class SaveButton
    {
        private bool _disabled = true;

        [Parameter]
        public string Id { get; set; }

        [CascadingParameter]
        public EditContext EditContext { get; set; }

        [CascadingParameter]
        public HandleModificationState HandleModificationState { get; set; }

        protected override void OnInitialized()
        {
            CssSubClass ??= "btn-primary";
            EditContext.OnValidationStateChanged += EditContext_OnValidationStateChanged;
            EditContext.OnFieldChanged += EditContext_OnFieldChanged;
            EditContext.OnValidationRequested += EditContext_OnValidationRequested;
            Localizer.OnResourceReady = () => InvokeAsync(StateHasChanged);
            HandleModificationState.OnStateChange += HandleModificationState_OnStateChange;
            base.OnInitialized();
        }

        private async Task OnClicked(MouseEventArgs args)
        {
            await Clicked.InvokeAsync(args).ConfigureAwait(false);
            _disabled = true;
        }

        private void HandleModificationState_OnStateChange(ModificationKind kind, object entity)
        {
            _disabled = false;
            StateHasChanged();
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
            StateHasChanged();
        }
    }
}
