// Project: Aguafrommars/TheIdServer
// Copyright (c) 2020 @Olivier Lefebvre
using Aguacongas.TheIdServer.BlazorApp.Models;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.TheIdServer.BlazorApp.Components.ClientComponents
{
    public partial class ClientUris
    {
        private IEnumerable<Entity.ClientUri> Uris => Collection.Where(u => u.Id == null || u.Uri != null && u.Uri.Contains(HandleModificationState.FilterTerm));

        [Parameter]
        public Entity.Client Model { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            Collection = Collection.OrderBy(u => u.Uri).ToList();
        }

        protected override void OnStort(SortEventArgs arg)
        {
            if (arg.OrderBy.StartsWith(nameof(Entity.ClientUri.Uri)))
            {
                base.OnStort(arg);
                return;
            }
            Collection = Collection.AsQueryable().Sort(arg.OrderBy, (uri, field, direction) =>
            {
                if (direction.ToUpper() == "DESC")
                {
                    return Collection.OrderByDescending(u => u.Kind, new UriComparer(field)).AsQueryable();
                }
                return Collection.OrderBy(u => u.Kind, new UriComparer(field)).AsQueryable();
            }).ToList();
        }

        class UriComparer : IComparer<Entity.UriKinds>
        {
            private readonly string _field;

            public UriComparer(string field)
            {
                _field = field;
            }

            public int Compare(Entity.UriKinds x, Entity.UriKinds y)
            {
                return _field switch
                {
                    "Cors" => (y & Entity.UriKinds.Cors).CompareTo(x & Entity.UriKinds.Cors),
                    "Redirect" => (y & Entity.UriKinds.Redirect).CompareTo(x & Entity.UriKinds.Redirect),
                    "PostLogout" => (y & Entity.UriKinds.PostLogout).CompareTo(x & Entity.UriKinds.PostLogout),
                    _ => throw new InvalidOperationException("Invalid kind."),
                };
            }
        }
    }
}
