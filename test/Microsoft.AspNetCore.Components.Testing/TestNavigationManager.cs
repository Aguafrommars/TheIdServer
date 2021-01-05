// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using System;

namespace Microsoft.AspNetCore.Components.Testing
{
    public class TestNavigationManager : NavigationManager
    {
        private readonly string _baseUri;
        private readonly string _uri;

        public TestNavigationManager(string baseUri = "http://exemple.com/", string uri = "http://exemple.com/")
        {
            _baseUri = baseUri;
            _uri = uri;
        }

        public Action<string, bool> OnNavigateToCore { get; set; }
        protected override void NavigateToCore(string uri, bool forceLoad)
        {
            OnNavigateToCore?.Invoke(uri, forceLoad);
        }

        protected override void EnsureInitialized()
        {
            Initialize(_baseUri, _uri);
        }
    }
}
