// Project: Aguafrommars/TheIdServer
// Copyright (c) 2020 @Olivier Lefebvre
using Aguacongas.AspNetCore.Authentication;
using Aguacongas.IdentityServer.Abstractions;
using Aguacongas.IdentityServer.KeysRotation;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.Admin.Services
{
    /// <summary>
    /// Subscribe to provider configuration change
    /// </summary>
    public class SchemeChangeSubscriber<TSchemeDefinition> : ISchemeChangeSubscriber where TSchemeDefinition : SchemeDefinitionBase, new()
    {
        private readonly HubConnectionFactory _factory;
        private readonly NoPersistentDynamicManager<TSchemeDefinition> _manager;
        private readonly IDynamicProviderStore<TSchemeDefinition> _store;
        private readonly KeyManagerWrapper<IAuthenticatedEncryptorDescriptor> _dataProtectionKeyManagerWrapper;
        private readonly KeyManagerWrapper<RsaEncryptorDescriptor> _signingKeyManagerWrapper;
        private readonly ILogger<SchemeChangeSubscriber<TSchemeDefinition>> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="SchemeChangeSubscriber{TSchemeDefinition}" /> class.
        /// </summary>
        /// <param name="factory">The factory.</param>
        /// <param name="manager">The manager.</param>
        /// <param name="store">The store.</param>
        /// <param name="dataProtectionKeyManagerWrapper">The data protection key manager wrapper.</param>
        /// <param name="signingKeyManagerWrapper">The signing key manager wrapper.</param>
        /// <param name="logger">The logger.</param>
        /// <exception cref="ArgumentNullException">factory
        /// or
        /// manager
        /// or
        /// manager</exception>
        public SchemeChangeSubscriber(HubConnectionFactory factory,
            NoPersistentDynamicManager<TSchemeDefinition> manager,
            IDynamicProviderStore<TSchemeDefinition> store,
            KeyManagerWrapper<IAuthenticatedEncryptorDescriptor> dataProtectionKeyManagerWrapper,
            KeyManagerWrapper<RsaEncryptorDescriptor> signingKeyManagerWrapper,
            ILogger<SchemeChangeSubscriber<TSchemeDefinition>> logger)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
            _manager = manager ?? throw new ArgumentNullException(nameof(manager));
            _store = store ?? throw new ArgumentNullException(nameof(manager));
            _dataProtectionKeyManagerWrapper = dataProtectionKeyManagerWrapper ?? throw new ArgumentNullException(nameof(dataProtectionKeyManagerWrapper));
            _signingKeyManagerWrapper = signingKeyManagerWrapper ?? throw new ArgumentNullException(nameof(signingKeyManagerWrapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Subscribes this instance.
        /// </summary>
        public Task SubscribeAsync(CancellationToken cancellationToken)
        {
            Task.Delay(15, cancellationToken).ContinueWith(t =>
            {
                var connection = _factory.GetConnection(cancellationToken);
                if (connection == null)
                {
                    return;
                }

                connection.On<string>(nameof(IProviderHub.ProviderAdded), async scheme =>
                {
                    _logger.LogInformation($"SignalR notification received: {nameof(IProviderHub.ProviderAdded)}({scheme})");
                    var definition = await _store.FindBySchemeAsync(scheme).ConfigureAwait(false);
                    await _manager.AddAsync(definition).ConfigureAwait(false);
                });

                connection.On<string>(nameof(IProviderHub.ProviderRemoved), async scheme =>
                {
                    _logger.LogInformation($"SignalR notification received: {nameof(IProviderHub.ProviderRemoved)}({scheme})");
                    await _manager.RemoveAsync(scheme).ConfigureAwait(false);
                });

                connection.On<string>(nameof(IProviderHub.ProviderUpdated), async scheme =>
                {
                    _logger.LogInformation($"SignalR notification received: {nameof(IProviderHub.ProviderUpdated)}({scheme})");
                    var definition = await _store.FindBySchemeAsync(scheme).ConfigureAwait(false);
                    await _manager.UpdateAsync(definition).ConfigureAwait(false);
                });

                connection.On<string, string>(nameof(IProviderHub.KeyRevoked), (kind, id) =>
                {
                    _logger.LogInformation($"SignalR notification received: {nameof(IProviderHub.KeyRevoked)}({kind}, {id})");
                    var keyId = Guid.Parse(id);
                    if (kind == nameof(IAuthenticatedEncryptorDescriptor))
                    {
                        _dataProtectionKeyManagerWrapper.Manager.RevokeKey(keyId, "Revoked by another instance.");
                        return;
                    }

                    _signingKeyManagerWrapper.Manager.RevokeKey(keyId, "Revoked by another instance.");
                });
            
                _factory.StartConnectionAsync(cancellationToken).ContinueWith(t => { });

            });

            return Task.CompletedTask;
        }

        /// <summary>
        /// Uns the subscribe asynchronous.
        /// </summary>
        /// <returns></returns>
        public Task UnSubscribeAsync(CancellationToken cancellationToken)
        {
            return _factory.StopConnectionAsync(cancellationToken);
        }
    }
}
