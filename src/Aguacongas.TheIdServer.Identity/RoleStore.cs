// Project: Aguafrommars/TheIdServer
// Copyright (c) 2022 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Aguacongas.TheIdServer.Identity
{
    /// <summary>
    /// Creates a new instance of a persistence store for roles.
    /// </summary>
    /// <typeparam name="TRole">The type of the class representing a role.</typeparam>
    [SuppressMessage("Major Code Smell", "S3881:\"IDisposable\" should be implemented correctly", Justification = "Nothing to dispose")]
    [SuppressMessage("Design", "CA1063:Implement IDisposable Correctly", Justification = "Nothing to dispose")]
    public class RoleStore<TRole> : IRoleClaimStore<TRole>
        where TRole : IdentityRole, new()
    {
        private readonly IAdminStore<Role> _roleStore;
        private readonly IAdminStore<RoleClaim> _claimStore;
        private bool _disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="RoleStore{TRole, TKey, TUserRole, TRoleClaim}"/> class.
        /// </summary>
        /// <param name="roleStore">The store.</param>
        /// <param name="describer">The describer.</param>
        /// <exception cref="System.ArgumentNullException">store</exception>
        public RoleStore(IAdminStore<Role> roleStore, IAdminStore<RoleClaim> claimStore, IdentityErrorDescriber describer = null)
        {
            _roleStore = roleStore ?? throw new ArgumentNullException(nameof(roleStore));
            _claimStore = claimStore ?? throw new ArgumentNullException(nameof(claimStore));
            ErrorDescriber = describer ?? new IdentityErrorDescriber();
        }

        /// <summary>
        /// Gets or sets the <see cref="IdentityErrorDescriber"/> for any error that occurred with the current operation.
        /// </summary>
        public IdentityErrorDescriber ErrorDescriber { get; set; }

        /// <summary>
        /// Creates a new role in a store as an asynchronous operation.
        /// </summary>
        /// <param name="role">The role to create in the store.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>A <see cref="Task{TResult}"/> that represents the <see cref="IdentityResult"/> of the asynchronous query.</returns>
        public async virtual Task<IdentityResult> CreateAsync(TRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            AssertNotNull(role, nameof(role));

            try
            {
                var created = await _roleStore.CreateAsync(role.ToRole(), cancellationToken).ConfigureAwait(false);
                role.Id = created.Id;
                return IdentityResult.Success;
            }
            catch(Exception e)
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Code = e.GetType().Name,
                    Description = e.Message
                });
            }
        }

        /// <summary>
        /// Updates a role in a store as an asynchronous operation.
        /// </summary>
        /// <param name="role">The role to update in the store.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>A <see cref="Task{TResult}"/> that represents the <see cref="IdentityResult"/> of the asynchronous query.</returns>
        public async virtual Task<IdentityResult> UpdateAsync(TRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            AssertNotNull(role, nameof(role));


            try
            {
                await _roleStore.UpdateAsync(role.ToRole(), cancellationToken).ConfigureAwait(false);

                return IdentityResult.Success;
            }
            catch (Exception e)
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Code = e.GetType().Name,
                    Description = e.Message
                });
            }
        }

        /// <summary>
        /// Deletes a role from the store as an asynchronous operation.
        /// </summary>
        /// <param name="role">The role to delete from the store.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>A <see cref="Task{TResult}"/> that represents the <see cref="IdentityResult"/> of the asynchronous query.</returns>
        public async virtual Task<IdentityResult> DeleteAsync(TRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            AssertNotNull(role, nameof(role));

            try
            {
                await _roleStore.DeleteAsync(role.Id, cancellationToken).ConfigureAwait(false);

                return IdentityResult.Success;
            }
            catch (Exception e)
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Code = e.GetType().Name,
                    Description = e.Message
                });
            }
        }

        /// <summary>
        /// Gets the ID for a role from the store as an asynchronous operation.
        /// </summary>
        /// <param name="role">The role whose ID should be returned.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>A <see cref="Task{TResult}"/> that contains the ID of the role.</returns>
        public virtual Task<string> GetRoleIdAsync(TRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            AssertNotNull(role, nameof(role));

            return Task.FromResult(role.Id);
        }

        /// <summary>
        /// Gets the name of a role from the store as an asynchronous operation.
        /// </summary>
        /// <param name="role">The role whose name should be returned.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>A <see cref="Task{TResult}"/> that contains the name of the role.</returns>
        public virtual Task<string> GetRoleNameAsync(TRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            AssertNotNull(role, nameof(role));

            return Task.FromResult(role.Name);
        }

        /// <summary>
        /// Sets the name of a role in the store as an asynchronous operation.
        /// </summary>
        /// <param name="role">The role whose name should be set.</param>
        /// <param name="roleName">The name of the role.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        public virtual Task SetRoleNameAsync(TRole role, string roleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            AssertNotNull(role, nameof(role));

            role.Name = roleName;
            return Task.CompletedTask;
        }

        /// <summary>
        /// Finds the role who has the specified ID as an asynchronous operation.
        /// </summary>
        /// <param name="roleId">The role ID to look for.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>A <see cref="Task{TResult}"/> that result of the look up.</returns>
        public virtual async Task<TRole> FindByIdAsync(string roleId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            var response = await _roleStore.GetAsync(new PageRequest
            {
                Filter = $"{nameof(Role.Id)} eq '{roleId}'"
            }, cancellationToken).ConfigureAwait(false);
            if (response.Items.Any())
            {
                return response.Items.First().ToIdentityRole<TRole>();
            }
            return null;
        }

        /// <summary>
        /// Finds the role who has the specified normalized name as an asynchronous operation.
        /// </summary>
        /// <param name="normalizedRoleName">The normalized role name to look for.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>A <see cref="Task{TResult}"/> that result of the look up.</returns>
        public virtual async Task<TRole> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            var response = await _roleStore.GetAsync(new PageRequest
            {
                Filter = $"{nameof(Role.NormalizedName)} eq '{normalizedRoleName}'"
            }, cancellationToken).ConfigureAwait(false);

            if (response.Items.Any())
            {
                return response.Items.First().ToIdentityRole<TRole>();
            }

            return default;
        }

        /// <summary>
        /// Get a role's normalized name as an asynchronous operation.
        /// </summary>
        /// <param name="role">The role whose normalized name should be retrieved.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>A <see cref="Task{TResult}"/> that contains the name of the role.</returns>
        public virtual Task<string> GetNormalizedRoleNameAsync(TRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            AssertNotNull(role, nameof(role));

            return Task.FromResult(role.NormalizedName);
        }

        /// <summary>
        /// Set a role's normalized name as an asynchronous operation.
        /// </summary>
        /// <param name="role">The role whose normalized name should be set.</param>
        /// <param name="normalizedName">The normalized name to set</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        public virtual Task SetNormalizedRoleNameAsync(TRole role, string normalizedName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            AssertNotNull(role, nameof(role));

            role.NormalizedName = normalizedName;
            return Task.CompletedTask;
        }

        /// <summary>
        /// Throws if this class has been disposed.
        /// </summary>
        protected void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }


        /// <summary>
        /// Dispose the stores
        /// </summary>
        [SuppressMessage("Blocker Code Smell", "S2953:Methods named \"Dispose\" should implement \"IDisposable.Dispose\"", Justification = "<Pending>")]
        public void Dispose()
        {
            _disposed = true;
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Get the claims associated with the specified <paramref name="role"/> as an asynchronous operation.
        /// </summary>
        /// <param name="role">The role whose claims should be retrieved.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>A <see cref="Task{TResult}"/> that contains the claims granted to a role.</returns>
        public async virtual Task<IList<Claim>> GetClaimsAsync(TRole role, CancellationToken cancellationToken = default)
        {
            ThrowIfDisposed();
            AssertNotNull(role, nameof(role));

            var response = await _claimStore.GetAsync(new PageRequest
            {
                Filter = $"{nameof(RoleClaim.RoleId)} eq '{role.Id}'"
            }, cancellationToken).ConfigureAwait(false);

            return response.Items.Select(CreateClaim).ToList();
        }

        /// <summary>
        /// Adds the <paramref name="claim"/> given to the specified <paramref name="role"/>.
        /// </summary>
        /// <param name="role">The role to add the claim to.</param>
        /// <param name="claim">The claim to add to the role.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        public virtual async Task AddClaimAsync(TRole role, Claim claim, CancellationToken cancellationToken =default)
        {
            ThrowIfDisposed();
            AssertNotNull(role, nameof(role));
            AssertNotNull(claim, nameof(claim));

            await _claimStore.CreateAsync(CreateRoleClaim(role, claim), cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Removes the <paramref name="claim"/> given from the specified <paramref name="role"/>.
        /// </summary>
        /// <param name="role">The role to remove the claim from.</param>
        /// <param name="claim">The claim to remove from the role.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        public async virtual Task RemoveClaimAsync(TRole role, Claim claim, CancellationToken cancellationToken = default)
        {
            ThrowIfDisposed();
            AssertNotNull(role, nameof(role));
            AssertNotNull(claim, nameof(claim));

            var response = await _claimStore.GetAsync(new PageRequest
            {
                Filter = $"{nameof(RoleClaim.RoleId)} eq '{role.Id}' and {nameof(RoleClaim.ClaimType)} eq '{claim.Type}' and {nameof(RoleClaim.ClaimValue)} eq '{claim.Value}'"
            }, cancellationToken).ConfigureAwait(false);

            foreach(var roleClaim in response.Items)
            {
                await _claimStore.DeleteAsync(roleClaim.Id.ToString(), cancellationToken).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Creates an entity representing a role claim.
        /// </summary>
        /// <param name="role">The associated role.</param>
        /// <param name="claim">The associated claim.</param>
        /// <returns>The role claim entity.</returns>
        protected virtual RoleClaim CreateRoleClaim(TRole role, Claim claim)
            => new RoleClaim { RoleId = role.Id, ClaimType = claim.Type, ClaimValue = claim.Value };

        private static void AssertNotNull(object p, string pName)
        {
            if (p == null)
            {
                throw new ArgumentNullException(pName);
            }
        }

        private Claim CreateClaim(RoleClaim claim)
        {
            return new Claim(claim.ClaimType, claim.ClaimValue);
        }
    }
}
