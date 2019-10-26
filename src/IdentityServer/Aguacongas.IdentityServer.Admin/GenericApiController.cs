using Aguacongas.IdentityServer.Store;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.Admin
{
    /// <summary>
    /// Api controller base.
    /// </summary>
    /// <typeparam name="T">Type of entity</typeparam>
    /// <seealso cref="Controller" />
    [Produces("application/json")]
    [Route("[controller]")]
    [GenericApiControllerNameConvention]
    [Authorize]
    public class GenericApiController<T> : Controller where T: class
    {
        private readonly IAdminStore<T> _store;

        public GenericApiController(IAdminStore<T> store)
            => _store = store ?? throw new ArgumentNullException(nameof(store));

        /// <summary>
        /// Gets a an entity
        /// </summary>
        /// <param name="id">entity id</param>
        /// <returns>An entity</returns>
        /// <response code="200">Returns an entity</response>
        /// <response code="404">The entity is not found</response>
        [HttpGet("{id}")]
        public Task<T> GetAsync(string id) 
            => _store.GetAsync(id);

        /// <summary>
        /// Gets a page of <see cref="T"/>
        /// </summary>
        /// <param name="request">A page request of <see cref="T"/></param>
        /// <returns>A page of <see cref="T"/></returns>
        /// <response code="200">Returns a page of entites</response>
        [HttpGet]
        public Task<PageResponse<T>> GetAsync(PageRequest request) 
            => _store.GetAsync(request);

        /// <summary>
        /// Creates the entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        [HttpPost]
        public Task<T> CreateAsync([FromBody] T entity)
            => _store.CreateAsync(entity);

        /// <summary>
        /// Updates the entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public Task<T> UpdateAsync(string id, [FromBody] T entity)
            => _store.UpdateAsync(entity);

        /// <summary>
        /// Deletes the entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public Task DeleteAsync(string id)
            => _store.DeleteAsync(id);
    }
}
