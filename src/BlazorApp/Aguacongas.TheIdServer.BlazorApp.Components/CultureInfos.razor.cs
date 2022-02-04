// Project: Aguafrommars/TheIdServer
// Copyright (c) 2022 @Olivier Lefebvre
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Aguacongas.TheIdServer.BlazorApp.Components
{
    public partial class CultureInfos
    {
        private IEnumerable<CultureInfo> _cultureInfos = CultureInfo.GetCultures(CultureTypes.AllCultures);
        private IEnumerable<CultureInfo> _filterValues;
        protected override bool IsReadOnly => true;

        protected override string PropertyName => "Name";

        protected override Task<IEnumerable<string>> GetFilteredValues(string term, CancellationToken cancellationToken)
        {
            term = term ?? string.Empty;
            _filterValues = _cultureInfos
                .Where(c => c.Name.Contains(term, StringComparison.OrdinalIgnoreCase) || c.DisplayName.Contains(term, StringComparison.OrdinalIgnoreCase))
                .OrderBy(c => c.Name)
                .Take(5);

            return Task.FromResult(_filterValues.Select(c => c.Name));
        }

        protected override void SetValue(string inputValue)
        {
            var cultureInfo = _cultureInfos
                .FirstOrDefault(c => c.Name == inputValue);
            if (cultureInfo != null)                
            {
                Entity = cultureInfo;
            }
        }
    }
}
