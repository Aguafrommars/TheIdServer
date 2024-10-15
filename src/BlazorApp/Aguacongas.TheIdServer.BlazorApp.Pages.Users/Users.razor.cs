// Project: Aguafrommars/TheIdServer
// Copyright (c) 2023 @Olivier Lefebvre
using Microsoft.AspNetCore.Components;

using System.Collections.Generic;
using System.Linq;

using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.TheIdServer.BlazorApp.Pages.Users
{
    public partial class Users
    {
        protected override string SelectProperties => $"{nameof(Entity.User.Id)},{nameof(Entity.User.UserName)}";

        protected override string ExportExpand => $"{nameof(Entity.User.UserClaims)},{nameof(Entity.User.UserRoles)}";

        [Parameter]
        public int PageSize { get; set; } = 10;

        [Parameter]
        public int CurrentPage { get; set; } = 1;

        public int TotalPages => (int)(EntityList?.Count() / (double)PageSize) + 1;

        protected IEnumerable<Entity.User> PagedEntityList => EntityList?.Skip((CurrentPage - 1) * PageSize).Take(PageSize);

        protected void NavigateToPage(int page)
        {
            if (page < 1 || page > TotalPages)
                return;

            CurrentPage = page;
            StateHasChanged();
        }
    }
}
