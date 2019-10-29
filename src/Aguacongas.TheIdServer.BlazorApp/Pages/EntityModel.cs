using Aguacongas.IdentityServer.Store;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Aguacongas.TheIdServer.BlazorApp.Pages
{
    public abstract class EntityModel<T> : ComponentBase where T: class, ICloneable<T>, new()
    {
        [Inject]
        public IAdminStore<T> AdminStore { get; set; }

        [Parameter]
        public string Id { get; set; }

        protected bool IsNew { get; private set; }

        protected T Model { get; private set; }

        protected T State { get; private set; }

        protected IReadOnlyDictionary<string, object> InputTextAttributes { get; set; }
            = new Dictionary<string, object>
            {
                ["class"] = "form-control"
            };
        protected abstract string Expand { get; }

        protected override async Task OnInitializedAsync()
        {
            if (Id == null)
            {
                IsNew = true;
                Model = Create();
                return;
            }

            State = await AdminStore.GetAsync(Id, new GetRequest
            {
                Expand = Expand
            }).ConfigureAwait(false);

            Model = State.Clone();
        }

        protected async Task HandleValidSubmit()
        {
            if (IsNew)
            {
                await AdminStore.CreateAsync(Model)
                    .ConfigureAwait(false);
                return;
            }

            await AdminStore.UpdateAsync(Model)
                .ConfigureAwait(false);
        }

        protected abstract T Create();
    }
}
