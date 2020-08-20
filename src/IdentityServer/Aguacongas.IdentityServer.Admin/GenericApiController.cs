using Aguacongas.IdentityServer.Admin.Services;
using Aguacongas.IdentityServer.Store;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel;
using System.Net;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.Admin
{
    /// <summary>
    /// Api controller base.
    /// </summary>
    /// <typeparam name="T">Type of entity</typeparam>
    /// <seealso cref="Controller" />
    [Produces(JsonFileOutputFormatter.SupportedContentType, "application/json")]
    [Route("[controller]")]
    [GenericApiControllerNameConvention]
    public class GenericApiController<T> : Controller where T : class
    {
        private readonly IAdminStore<T> _store;

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericApiController{T}"/> class.
        /// </summary>
        /// <param name="store">The store.</param>
        /// <exception cref="ArgumentNullException">store</exception>
        public GenericApiController(IAdminStore<T> store)
            => _store = store ?? throw new ArgumentNullException(nameof(store));

        /// <summary>
        /// Gets an entity.
        /// </summary>
        /// <param name="id">entity id.</param>
        /// <param name="request">The request.</param>
        /// <returns>
        /// An entity.
        /// </returns>
        /// <response code="200">Returns an entity.</response>
        /// <response code="400">Bad request.</response>
        /// <response code="204">No content.</response>
        [HttpGet("{id}")]
        [Description("Gets an entity")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.BadRequest)]
        [Authorize(Policy = SharedConstants.READER)]
        public Task<T> GetAsync(string id, GetRequest request) 
            => _store.GetAsync(id, request);

        /// <summary>
        /// Gets a page of entites.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>Returns a page of entites for the request.</returns>
        /// <response code="200">Returns a page of entites.</response>
        [HttpGet]
        [Description("Search entities using OData style query string (wihtout $)")]
        [Authorize(Policy = SharedConstants.READER)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        public Task<PageResponse<T>> FindAsync(PageRequest request) 
            => _store.GetAsync(request);

        /// <summary>
        /// Creates the entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>The entity.</returns>
        /// <response code="201">Returns the entity.</response>
        /// <response code="400">Bad request.</response>
        [HttpPost]
        [Description("Creates an entity")]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [Authorize(Policy = SharedConstants.WRITER)]
        public Task<T> CreateAsync([FromBody] T entity)
            => _store.CreateAsync(entity);

        /// <summary>
        /// Updates the entity.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="entity">The entity.</param>
        /// <returns>The entity.</returns>
        /// <response code="200">Returns the entity.</response>
        /// <response code="400">Bad request.</response>
        /// <response code="409">Conflict.</response>
        [HttpPut("{id}")]
        [Description("Updates an entity")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Conflict)]
        [Authorize(Policy = SharedConstants.WRITER)]
#pragma warning disable IDE0060 // Remove unused parameter. The id is used in ActionsFilter, id and entity.Id MUST match. 
        public Task<T> UpdateAsync(string id, [FromBody] T entity)
#pragma warning restore IDE0060 // Remove unused parameter
            => _store.UpdateAsync(entity);

        /// <summary>
        /// Deletes the entity.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        /// <response code="200">Ok.</response>
        /// <response code="409">Conflict.</response>
        [HttpDelete("{id}")]
        [Description("Deletes an entity")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [Authorize(Policy = SharedConstants.WRITER)]
        public Task DeleteAsync(string id)
            => _store.DeleteAsync(id);
    }
}
