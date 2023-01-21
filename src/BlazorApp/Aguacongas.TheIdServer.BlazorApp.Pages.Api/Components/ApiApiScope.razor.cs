// Project: Aguafrommars/TheIdServer
// Copyright (c) 2023 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using StoreEntity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.TheIdServer.BlazorApp.Pages.Api.Components
{
    public partial class ApiApiScope
    {
        private bool _isReadOnly;

        private readonly PageRequest _pageRequest = new PageRequest
        {
            Select = nameof(StoreEntity.ApiScope.Id),
            Take = 5
        };

        protected override bool IsReadOnly => _isReadOnly;

        protected override string PropertyName => "Id";

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            _isReadOnly = Entity.Id != null;
        }

        protected override async Task<IEnumerable<string>> GetFilteredValues(string term, CancellationToken cancellationToken)
        {
            _pageRequest.Filter = $"contains({nameof(Entity.Id)},'{term}')";
            var response = await _store.GetAsync(_pageRequest, cancellationToken)
                .ConfigureAwait(false);

            return response.Items.Select(c => c.Id);
        }

        protected override void SetValue(string inputValue)
        {
            Entity.ApiScopeId = inputValue;
        }
    }
}
