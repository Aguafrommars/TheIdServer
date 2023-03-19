// Project: Aguafrommars/TheIdServer
// Copyright (c) 2023 @Olivier Lefebvre
using Microsoft.AspNetCore.Components;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.TheIdServer.BlazorApp.Pages.Client.Components
{
    public partial class ClientAllowedIdentityTokenSigningAlgorithm
    {
        private bool _isReadOnly;

        private readonly IEnumerable<string> _allowedAlgorithms = new[]
        {
            SecurityAlgorithms.RsaSha256,
            SecurityAlgorithms.RsaSha384,
            SecurityAlgorithms.RsaSha512,

            SecurityAlgorithms.RsaSsaPssSha256,
            SecurityAlgorithms.RsaSsaPssSha384,
            SecurityAlgorithms.RsaSsaPssSha512,

            SecurityAlgorithms.EcdsaSha256,
            SecurityAlgorithms.EcdsaSha384,
            SecurityAlgorithms.EcdsaSha512
        };

        [Parameter]
        public Entity.Client Model { get; set; }

        protected override bool IsReadOnly => _isReadOnly;

        protected override string PropertyName => nameof(Entity.Algorithm);

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            _isReadOnly = Entity.Id != null;
        }

        protected override Task<IEnumerable<string>> GetFilteredValues(string term, CancellationToken cancellationToken)
        => Task.FromResult(_allowedAlgorithms.Where(a => (term is null || a.ToUpperInvariant().Contains(term.ToUpperInvariant()))
            && !Model.AllowedIdentityTokenSigningAlgorithms.Any(alg => alg.Algorithm == a)));

        protected override void SetValue(string inputValue)
        {
            Entity.Algorithm = inputValue;
        }
    }
}
