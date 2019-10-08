using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace Aguacongas.IdentityServer.Admin
{
    public class ClientController : ODataController
    {
        private readonly IAdminStore<Client> _store;

        public ClientController(IAdminStore<Client> store)
        {
            _store = store ?? throw new ArgumentNullException(nameof(store));
        }

        [EnableQuery]
        public IActionResult Get()
        {
            return Ok(_store.Items);
        }

        [EnableQuery]
        public IActionResult Get(string key)
        {
            return Ok(_store.Items.FirstOrDefault(c => c.Id == key));
        }


    }
}
