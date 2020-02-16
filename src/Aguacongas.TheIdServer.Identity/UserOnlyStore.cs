// Project: aguacongas/Identity.Firebase
// Copyright (c) 2020 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Aguacongas.TheIdServer.Identity
{
    /// <summary>
    /// Represents a new instance of a persistence store for <see cref="IdentityUser"/>.
    /// </summary>
    /// <seealso cref="UserOnlyStore{IdentityUser}" />
    public class UserOnlyStore : UserOnlyStore<IdentityUser>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserOnlyStore"/> class.
        /// </summary>
        /// <param name="store"></param>
        /// <param name="claimStore"></param>
        /// <param name="loginStore"></param>
        /// <param name="tokenStore"></param>
        /// <param name="describer">The <see cref="T:Microsoft.AspNetCore.Identity.IdentityErrorDescriber" /> used to describe store errors.</param>
        public UserOnlyStore(IAdminStore<User> store,
            IAdminStore<UserClaim> claimStore, 
            IAdminStore<UserLogin> loginStore, 
            IAdminStore<UserToken> tokenStore, 
            IdentityErrorDescriber describer = null) : base(store, claimStore, loginStore, tokenStore, describer)
        {
        }
    }

    /// <summary>
    /// Represents a new instance of a persistence store for <see cref="IdentityUser"/>.
    /// </summary>
    public class UserOnlyStore<TUser> : TheIdServerUserStoreBase<TUser, 
            string, 
            IdentityUserClaim<string>, 
            IdentityUserLogin<string>, 
            IdentityUserToken<string>>
        where TUser: IdentityUser, new()
    {
        private readonly IAdminStore<User> _userStore;
        private readonly IAdminStore<UserClaim> _claimStore;
        private readonly IAdminStore<UserLogin> _loginStore;
        private readonly IAdminStore<UserToken> _tokenStore;

        /// <summary>
        /// Creates a new instance of the store.
        /// </summary>
        /// <param name="db">The <see cref="IDatabase"/>.</param>
        /// <param name="describer">The <see cref="IdentityErrorDescriber"/> used to describe store errors.</param>
        public UserOnlyStore(IAdminStore<User> userStore,
            IAdminStore<UserClaim> claimStore,
            IAdminStore<UserLogin> loginStore,
            IAdminStore<UserToken> tokenStore,
            IdentityErrorDescriber describer = null) : base(describer ?? new IdentityErrorDescriber())
        {
            _userStore = userStore ?? throw new ArgumentNullException(nameof(userStore));
            _claimStore = claimStore ?? throw new ArgumentNullException(nameof(claimStore));
            _loginStore = loginStore ?? throw new ArgumentNullException(nameof(loginStore));
            _tokenStore = tokenStore ?? throw new ArgumentNullException(nameof(tokenStore));
        }


        /// <summary>
        /// Creates the specified <paramref name="user"/> in the user store.
        /// </summary>
        /// <param name="user">The user to create.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/> of the creation operation.</returns>
        public async override Task<IdentityResult> CreateAsync(TUser user, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            AssertNotNull(user, nameof(user));

            try
            {
                var created = await _userStore.CreateAsync(user.ToUser(), cancellationToken).ConfigureAwait(false);
                user.Id = created.Id;
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
        /// Updates the specified <paramref name="user"/> in the user store.
        /// </summary>
        /// <param name="user">The user to update.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/> of the update operation.</returns>
        public async override Task<IdentityResult> UpdateAsync(TUser user, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            AssertNotNull(user, nameof(user));

            try
            {
                await _userStore.UpdateAsync(user.ToUser(), cancellationToken).ConfigureAwait(false);
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
        /// Deletes the specified <paramref name="user"/> from the user store.
        /// </summary>
        /// <param name="user">The user to delete.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/> of the update operation.</returns>
        public async override Task<IdentityResult> DeleteAsync(TUser user, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            AssertNotNull(user, nameof(user));

            try
            {
                await _userStore.DeleteAsync(user.Id, cancellationToken).ConfigureAwait(false);
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

        public override Task SetNormalizedUserNameAsync(TUser user, string normalizedName, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            AssertNotNull(user, nameof(user));

            user.NormalizedUserName = normalizedName;

            return Task.CompletedTask;
        }

        public override Task SetNormalizedEmailAsync(TUser user, string normalizedEmail, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            AssertNotNull(user, nameof(user));

            user.NormalizedEmail = normalizedEmail;

            return Task.CompletedTask;
        }

        /// <summary>
        /// Finds and returns a user, if any, who has the specified <paramref name="userId"/>.
        /// </summary>
        /// <param name="userId">The user ID to search for.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The <see cref="Task"/> that represents the asynchronous operation, containing the user matching the specified <paramref name="userId"/> if it exists.
        /// </returns>
        public override async Task<TUser> FindByIdAsync(string userId, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            var response = await _userStore.GetAsync(new PageRequest
            {
                Filter = $"{nameof(User.Id)} eq '{userId}'"
            }, cancellationToken).ConfigureAwait(false);

            if (response.Count == 1)
            {
                return CreateUser(response.Items.First());
            }

            return default;
        }

        /// <summary>
        /// Finds and returns a user, if any, who has the specified normalized user name.
        /// </summary>
        /// <param name="normalizedUserName">The normalized user name to search for.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The <see cref="Task"/> that represents the asynchronous operation, containing the user matching the specified <paramref name="normalizedUserName"/> if it exists.
        /// </returns>
        public override async Task<TUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            var response = await _userStore.GetAsync(new PageRequest
            {
                Filter = $"{nameof(User.NormalizedUserName)} eq '{normalizedUserName}'"
            }, cancellationToken).ConfigureAwait(false);

            if (response.Count == 1)
            {

                return CreateUser(response.Items.First());
            }

            return default;
        }

        /// <summary>
        /// Get the claims associated with the specified <paramref name="user"/> as an asynchronous operation.
        /// </summary>
        /// <param name="user">The user whose claims should be retrieved.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>A <see cref="Task{TResult}"/> that contains the claims granted to a user.</returns>
        public async override Task<IList<Claim>> GetClaimsAsync(TUser user, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            AssertNotNull(user, nameof(user));

            var response = await _claimStore.GetAsync(new PageRequest
            {
                Filter = $"{nameof(UserClaim.UserId)} eq '{user.Id}'"
            }, cancellationToken).ConfigureAwait(false);

            return response.Items.Select(CreateClaim).ToList();
        }

        /// <summary>
        /// Adds the <paramref name="claims"/> given to the specified <paramref name="user"/>.
        /// </summary>
        /// <param name="user">The user to add the claim to.</param>
        /// <param name="claims">The claim to add to the user.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        public override async Task AddClaimsAsync(TUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            AssertNotNull(user, nameof(user));
            AssertNotNull(claims, nameof(claims));

            var taskList = new List<Task>(claims.Count());

            foreach(var claim in claims)
            {
                taskList.Add(_claimStore.CreateAsync(CreateUserClaim(user, claim), cancellationToken));
            }

            await Task.WhenAll(taskList).ConfigureAwait(false);

        }

        /// <summary>
        /// Replaces the <paramref name="claim"/> on the specified <paramref name="user"/>, with the <paramref name="newClaim"/>.
        /// </summary>
        /// <param name="user">The user to replace the claim on.</param>
        /// <param name="claim">The claim replace.</param>
        /// <param name="newClaim">The new claim replacing the <paramref name="claim"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        public async override Task ReplaceClaimAsync(TUser user, Claim claim, Claim newClaim, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            AssertNotNull(user, nameof(user));
            AssertNotNull(claim, nameof(claim));
            AssertNotNull(newClaim, nameof(newClaim));

            var response = await _claimStore.GetAsync(new PageRequest
            {
                Filter = $"{nameof(UserClaim.UserId)} eq '{user.Id}' and {nameof(UserClaim.ClaimType)} eq '{claim.Type}' and {nameof(UserClaim.ClaimValue)} eq '{claim.Value}'"
            }, cancellationToken).ConfigureAwait(false);

            var taskList = new List<Task>(response.Count);
            foreach (var roleClaim in response.Items)
            {
                roleClaim.ClaimType = newClaim.Type;
                roleClaim.ClaimValue = newClaim.Value;
                taskList.Add(_claimStore.UpdateAsync(roleClaim, cancellationToken));
            }

            await Task.WhenAll(taskList).ConfigureAwait(false);
        }

        /// <summary>
        /// Removes the <paramref name="claims"/> given from the specified <paramref name="user"/>.
        /// </summary>
        /// <param name="user">The user to remove the claims from.</param>
        /// <param name="claims">The claim to remove.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        public async override Task RemoveClaimsAsync(TUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken = default)
        {
            ThrowIfDisposed();
            AssertNotNull(user, nameof(user));
            AssertNotNull(claims, nameof(claims));

            var userClaims = await GetUserClaimsAsync(user).ConfigureAwait(false);
            var toRemove = userClaims.Where(c => claims.Any(cl => cl.Type == c.ClaimType && cl.Value == c.ClaimValue));
            var taskList = new List<Task>(toRemove.Count());
            foreach (var claim in toRemove)
            {
                taskList.Add(_claimStore.DeleteAsync(claim.Id.ToString(), cancellationToken));
            }

            await Task.WhenAll(taskList).ConfigureAwait(false);
        }

        /// <summary>
        /// Adds the <paramref name="login"/> given to the specified <paramref name="user"/>.
        /// </summary>
        /// <param name="user">The user to add the login to.</param>
        /// <param name="login">The login to add to the user.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        public override async Task AddLoginAsync(TUser user, UserLoginInfo login,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            AssertNotNull(user, nameof(user));
            AssertNotNull(login, nameof(login));

            await _loginStore.CreateAsync(CreateUserLogin(user, login), cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Removes the <paramref name="loginProvider"/> given from the specified <paramref name="user"/>.
        /// </summary>
        /// <param name="user">The user to remove the login from.</param>
        /// <param name="loginProvider">The login to remove from the user.</param>
        /// <param name="providerKey">The key provided by the <paramref name="loginProvider"/> to identify a user.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        public override async Task RemoveLoginAsync(TUser user, string loginProvider, string providerKey,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            AssertNotNull(user, nameof(user));

            var response = await _loginStore.GetAsync(new PageRequest
            {
                Filter = $"{nameof(UserLogin.UserId)} eq '{user.Id}' and {nameof(UserLogin.LoginProvider)} eq '{loginProvider}' and {nameof(UserLogin.ProviderKey)} eq '{providerKey}'"
            }).ConfigureAwait(false);

            foreach(var login in response.Items)
            {
                await _loginStore.DeleteAsync($"{login.UserId}@{login.LoginProvider}@{login.ProviderKey}").ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Retrieves the associated logins for the specified <param ref="user"/>.
        /// </summary>
        /// <param name="user">The user whose associated logins to retrieve.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The <see cref="Task"/> for the asynchronous operation, containing a list of <see cref="UserLoginInfo"/> for the specified <paramref name="user"/>, if any.
        /// </returns>
        public async override Task<IList<UserLoginInfo>> GetLoginsAsync(TUser user, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            AssertNotNull(user, nameof(user));

            var response = await _loginStore.GetAsync(new PageRequest
            {
                Filter = $"{nameof(UserLogin.UserId)} eq '{user.Id}'"
            }).ConfigureAwait(false);

            return response.Items.Select(l => new UserLoginInfo
            (
                l.LoginProvider,
                l.ProviderKey,
                l.ProviderDisplayName
            )).ToList();
        }

        /// <summary>
        /// Retrieves the user associated with the specified login provider and login provider key.
        /// </summary>
        /// <param name="loginProvider">The login provider who provided the <paramref name="providerKey"/>.</param>
        /// <param name="providerKey">The key provided by the <paramref name="loginProvider"/> to identify a user.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The <see cref="Task"/> for the asynchronous operation, containing the user, if any which matched the specified login provider and key.
        /// </returns>
        public async override Task<TUser> FindByLoginAsync(string loginProvider, string providerKey,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            var userLogin = await FindUserLoginAsync(loginProvider, providerKey, cancellationToken)
                .ConfigureAwait(false);
            if (userLogin != null)
            {
                return await FindUserAsync(userLogin.UserId, cancellationToken)
                    .ConfigureAwait(false);
            }
            return null;
        }

        /// <summary>
        /// Gets the user, if any, associated with the specified, normalized email address.
        /// </summary>
        /// <param name="normalizedEmail">The normalized email address to return the user for.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The task object containing the results of the asynchronous lookup operation, the user if any associated with the specified normalized email address.
        /// </returns>
        public override async Task<TUser> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            var response = await _userStore.GetAsync(new PageRequest
            {
                Filter = $"{nameof(User.NormalizedEmail)} eq '{normalizedEmail}'"
            }).ConfigureAwait(false);

            if (response.Count == 1)
            {
                return CreateUser(response.Items.First());
            }

            return default;
        }

        /// <summary>
        /// Retrieves all users with the specified claim.
        /// </summary>
        /// <param name="claim">The claim whose users should be retrieved.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The <see cref="Task"/> contains a list of users, if any, that contain the specified claim. 
        /// </returns>
        public async override Task<IList<TUser>> GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            AssertNotNull(claim, nameof(claim));

            var response = await _claimStore.GetAsync(new PageRequest
            {
                Filter = $"{nameof(UserClaim.ClaimType)} eq '{claim.Type}' and {nameof(UserClaim.ClaimValue)} eq '{claim.Value}'"
            }).ConfigureAwait(false);

            var users = new ConcurrentBag<TUser>();
            var taskList = new List<Task>(response.Count);
            foreach (var uc in response.Items)
            {
                taskList.Add(Task.Run(async () => {
                    var user = await FindByIdAsync(uc.UserId, cancellationToken)
                        .ConfigureAwait(false);
                    if (user != null)
                    {
                        users.Add(user);
                    }
                }));
            }

            await Task.WhenAll(taskList).ConfigureAwait(false);

            return users.ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="loginProvider"></param>
        /// <param name="name"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override Task RemoveTokenAsync(TUser user, string loginProvider, string name, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            AssertNotNull(user, nameof(user));

            return _tokenStore.DeleteAsync($"{user.Id}@{loginProvider}@{name}");
        }

        /// <summary>
        /// Return a user login with the matching userId, provider, providerKey if it exists.
        /// </summary>
        /// <param name="userId">The user's id.</param>
        /// <param name="loginProvider">The login provider name.</param>
        /// <param name="providerKey">The key provided by the <paramref name="loginProvider"/> to identify a user.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The user login if it exists.</returns>
        internal Task<IdentityUserLogin<string>> FindUserLoginInternalAsync(string userId, string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            return FindUserLoginAsync(userId, loginProvider, providerKey, cancellationToken);
        }

        /// <summary>
        /// Return a user login with  provider, providerKey if it exists.
        /// </summary>
        /// <param name="loginProvider">The login provider name.</param>
        /// <param name="providerKey">The key provided by the <paramref name="loginProvider"/> to identify a user.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The user login if it exists.</returns>
        internal Task<IdentityUserLogin<string>> FindUserLoginInternalAsync(string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            return FindUserLoginAsync(loginProvider, providerKey, cancellationToken);
        }

        /// <summary>
        /// Get user tokens
        /// </summary>
        /// <param name="user">The token owner.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>User tokens.</returns>
        internal Task<List<IdentityUserToken<string>>> GetUserTokensInternalAsync(TUser user, CancellationToken cancellationToken)
        {
            return GetUserTokensAsync(user, cancellationToken);
        }

        /// <summary>
        /// Save user tokens.
        /// </summary>
        /// <param name="user">The tokens owner.</param>
        /// <param name="tokens">Tokens to save</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns></returns>
        internal Task SaveUserTokensInternalAsync(TUser user, IEnumerable<IdentityUserToken<string>> tokens, CancellationToken cancellationToken)
        {
            return SaveUserTokensAsync(user, tokens, cancellationToken);
        }
        
        /// <summary>
        /// Return a user with the matching userId if it exists.
        /// </summary>
        /// <param name="userId">The user's id.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The user if it exists.</returns>
        protected override Task<TUser> FindUserAsync(string userId, CancellationToken cancellationToken)
        {
            return FindByIdAsync(userId.ToString(), cancellationToken);
        }

        /// <summary>
        /// Return a user login with the matching userId, provider, providerKey if it exists.
        /// </summary>
        /// <param name="userId">The user's id.</param>
        /// <param name="loginProvider">The login provider name.</param>
        /// <param name="providerKey">The key provided by the <paramref name="loginProvider"/> to identify a user.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The user login if it exists.</returns>
        protected override async Task<IdentityUserLogin<string>> FindUserLoginAsync(string userId, string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            var response = await _loginStore.GetAsync(new PageRequest
            {
                Filter = $"{nameof(UserLogin.UserId)} eq '{userId}' and {nameof(UserLogin.LoginProvider)} eq '{loginProvider}' and {nameof(UserLogin.ProviderKey)} eq '{providerKey}'"
            }).ConfigureAwait(false);

            if (response.Count == 1)
            {
                return CreateIdentityUserLogin(response.Items.First());
            }
            return null;
        }

        /// <summary>
        /// Return a user login with  provider, providerKey if it exists.
        /// </summary>
        /// <param name="loginProvider">The login provider name.</param>
        /// <param name="providerKey">The key provided by the <paramref name="loginProvider"/> to identify a user.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The user login if it exists.</returns>
        protected override async Task<IdentityUserLogin<string>> FindUserLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken)
        {

            var response = await _loginStore.GetAsync(new PageRequest
            {
                Filter = $"{nameof(UserLogin.LoginProvider)} eq '{loginProvider}' and {nameof(UserLogin.ProviderKey)} eq '{providerKey}'"
            }).ConfigureAwait(false);

            if (response.Count == 1)
            {
                return CreateIdentityUserLogin(response.Items.First());
            }
            return null;
        }

        /// <summary>
        /// Get user tokens
        /// </summary>
        /// <param name="user">The token owner.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>User tokens.</returns>
        protected override async Task<List<IdentityUserToken<string>>> GetUserTokensAsync(TUser user, CancellationToken cancellationToken)
        {
            var response = await _tokenStore.GetAsync(new PageRequest
            {
                Filter = $"{nameof(UserToken.UserId)} eq '{user.Id}'"
            }, cancellationToken).ConfigureAwait(false);
            return response.Items.Select(IdentityUserToken).ToList();
        }

        /// <summary>
        /// Save user tokens.
        /// </summary>
        /// <param name="user">The tokens owner.</param>
        /// <param name="tokens">Tokens to save</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns></returns>
        protected override async Task SaveUserTokensAsync(TUser user, IEnumerable<IdentityUserToken<string>> tokens, CancellationToken cancellationToken)
        {
            var taskList = new List<Task>(tokens.Count());
            foreach(var token in tokens)
            {
                taskList.Add(_tokenStore.CreateAsync(token.ToEntity(), cancellationToken));
            }

            await Task.WhenAll(taskList).ConfigureAwait(false);
        }

        protected virtual async Task<List<IdentityUserClaim<string>>> GetUserClaimsAsync(TUser user)
        {
            var response = await _claimStore.GetAsync(new PageRequest
            {
                Filter = $"{nameof(UserClaim.UserId)} eq '{user.Id}'"
            }).ConfigureAwait(false);
            return response.Items.Select(CreateIdentityUserClaim).ToList();
        }

        protected virtual async Task<List<IdentityUserLogin<string>>> GetUserLoginsAsync(string userId)
        {
            var response = await _loginStore.GetAsync(new PageRequest
            {
                Filter = $"{nameof(UserLogin.UserId)} eq '{userId}'"
            }).ConfigureAwait(false);
            return response.Items.Select(CreateIdentityUserLogin).ToList();
        }

        private IdentityUserToken<string> IdentityUserToken(UserToken entity)
        {
            return new IdentityUserToken<string>
            {
                UserId = entity.UserId,
                LoginProvider = entity.LoginProvider,
                Name = entity.Name,
                Value = entity.Value
            };
        }
        private IdentityUserClaim<string> CreateIdentityUserClaim(UserClaim entity)
        {
            return new IdentityUserClaim<string>
            {
                UserId = entity.UserId,
                ClaimType = entity.ClaimType,
                ClaimValue = entity.ClaimValue,
                Id = int.Parse(entity.Id)
            };
        }
        private static IdentityUserLogin<string> CreateIdentityUserLogin(UserLogin entity)
        {
            return new IdentityUserLogin<string>
            {
                LoginProvider = entity.LoginProvider,
                ProviderDisplayName = entity.ProviderDisplayName,
                ProviderKey = entity.ProviderKey,
                UserId = entity.UserId
            };
        }

        private Claim CreateClaim(UserClaim claim)
        {
            return new Claim(claim.ClaimType, claim.ClaimValue);
        }

    }
}
