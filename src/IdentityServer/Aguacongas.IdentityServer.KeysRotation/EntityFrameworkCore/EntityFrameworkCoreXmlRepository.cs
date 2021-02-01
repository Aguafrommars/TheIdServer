// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

// Modifications copyright (c) 2021 @Olivier Lefebvre

// This file is a copy of https://github.com/dotnet/aspnetcore/blob/v3.1.8/src/DataProtection/EntityFrameworkCore/src/EntityFrameworkCoreXmlRepository.cs
// with :
// namespace change from original Microsoft.AspNetCore.DataProtection.EntityFrameworkCore
// generic context interface change from original IDataProtectionKeyContext
using Microsoft.AspNetCore.DataProtection.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

// namespace change from original Microsoft.AspNetCore.DataProtection.EntityFrameworkCore
namespace Aguacongas.IdentityServer.KeysRotation.EntityFrameworkCore
{
    /// <summary>
    /// An <see cref="IXmlRepository"/> backed by an EntityFrameworkCore datastore.
    /// </summary>
    public class EntityFrameworkCoreXmlRepository<TContext> : IXmlRepository
        where TContext : DbContext, IKeyRotationContext // generic context interface change from original IDataProtectionKeyContext
    {
        private readonly IServiceProvider _services;
        private readonly ILogger _logger;

        /// <summary>
        /// Creates a new instance of the <see cref="EntityFrameworkCoreXmlRepository{TContext}"/>.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="loggerFactory">The <see cref="ILoggerFactory"/>.</param>
        public EntityFrameworkCoreXmlRepository(IServiceProvider services, ILoggerFactory loggerFactory)
        {
            if (loggerFactory == null)
            {
                throw new ArgumentNullException(nameof(loggerFactory));
            }

            _logger = loggerFactory.CreateLogger<EntityFrameworkCoreXmlRepository<TContext>>();
            _services = services ?? throw new ArgumentNullException(nameof(services));
        }

        /// <inheritdoc />
        public virtual IReadOnlyCollection<XElement> GetAllElements()
        {
            using var scope = _services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<TContext>();

            // Put logger in a local such that `this` isn't captured.
            var logger = _logger;
            return context.KeyRotationKeys.AsNoTracking().Select(key => TryParseKeyXml(key.Xml, logger)).ToList().AsReadOnly(); // generic context interface change from original IDataProtectionKeyContext
        }

        /// <inheritdoc />
        public void StoreElement(XElement element, string friendlyName)
        {
            using var scope = _services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<TContext>();
            var newKey = new KeyRotationKey
            {
                FriendlyName = friendlyName,
                Xml = element.ToString(SaveOptions.DisableFormatting)
            };

            context.KeyRotationKeys.Add(newKey); // generic context interface change from original IDataProtectionKeyContext
            _logger.LogDebug("Saving key '{FriendlyName}' to '{DbContext}'.", typeof(TContext).Name);
            context.SaveChanges();
        }

        private static XElement TryParseKeyXml(string xml, ILogger logger)
        {
            try
            {
                return XElement.Parse(xml);
            }
            catch (Exception e)
            {
                logger?.LogWarning(e, "An exception occurred while parsing the key xml '{Xml}'.", xml);
                return null;
            }
        }
    }
}
