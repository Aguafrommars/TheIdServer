// Project: Aguafrommars/TheIdServer
// Copyright (c) 2023 @Olivier Lefebvre
using Aguacongas.IdentityServer.Admin.Services;
using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.Admin
{
    /// <summary>
    /// Generic key controller
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="Controller" />
    [Produces(JsonFileOutputFormatter.SupportedContentType, "application/json")]
    [Route("[controller]")]
    [GenericControllerNameConvention]
    public class GenericKeyController<T> : Controller where T : IAuthenticatedEncryptorDescriptor
    {
        private readonly KeyManagerWrapper<T> _wrapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericKeyController{T}"/> class.
        /// </summary>
        /// <param name="wrapper">The manager.</param>
        /// <exception cref="ArgumentNullException">manager</exception>
        public GenericKeyController(KeyManagerWrapper<T> wrapper)
        {
            _wrapper = wrapper ?? throw new ArgumentNullException(nameof(wrapper));
        }

        /// <summary>
        /// Gets all keys.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Policy = SharedConstants.READERPOLICY)]
        public PageResponse<Key> Get()
            => _wrapper.GetAllKeys();

        /// <summary>
        ///  Revokes a specific key and persists the revocation to the underlying repository.
        /// </summary>
        /// <param name="id"> The id of the key to revoke.</param>
        /// <param name="reason">An optional human-readable reason for revocation.</param>
        [HttpDelete("{id}")]
        [Authorize(Policy = SharedConstants.WRITERPOLICY)]
        public Task RevokeKey(Guid id, string reason)
            =>  _wrapper.RevokeKey(id, reason);
    }
}
