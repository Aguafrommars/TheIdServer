// Project: Aguafrommars/TheIdServer
// Copyright (c) 2020 @Olivier Lefebvre
using Aguacongas.IdentityServer.Abstractions;
using Aguacongas.IdentityServer.Admin.Models;
using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.Admin.Services
{
    /// <summary>
    /// Import service
    /// </summary>
    public class ImportService : IImportService
    {
        private readonly IServiceProvider _provider;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="provider"></param>
        public ImportService(IServiceProvider provider)
        {
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
        }

        /// <summary>
        /// Imports files
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
        public async Task<ImportResult> ImportAsync(IEnumerable<IFormFile> files)
        {
            var result = new ImportResult();
            foreach (var file in files)
            {
                result.Results.Add(await ImportFileAsync(file).ConfigureAwait(false));
            }
            return result;
        }

        private async Task<ImportFileResult> ImportFileAsync(IFormFile file)
        {
            var reader = new StreamReader(file.OpenReadStream());
            var content = await reader.ReadToEndAsync().ConfigureAwait(false);
            var metadata = JsonConvert.DeserializeObject<EntityMetadata>(content);
            var entityType = Type.GetType(metadata.Metadata.TypeName);
            var importerType = typeof(Importer<>).MakeGenericType(entityType);
            var importer = Activator.CreateInstance(importerType, _provider) as Importer;
            var result = await importer.ImportAsync(content).ConfigureAwait(false);
            result.FileName = file.FileName;
            return result;
        }

        class Importer<T> : Importer where T : class, IEntityId
        {
            private readonly IServiceProvider _provider;

            [SuppressMessage("Major Code Smell", "S1144:Unused private types or members should be removed", Justification = "Called by reflexion")]
            public Importer(IServiceProvider provider)
            {
                _provider = provider;
            }

            public override async Task<ImportFileResult> ImportAsync(string content)
            {
                var result = new ImportFileResult();
                var entityList = JsonConvert.DeserializeObject<PageResponse<T>>(content);
                var store = _provider.GetRequiredService<IAdminStore<T>>();
                foreach (var entity in entityList.Items)
                {
                    await AddOrUpdateEntityAsync(entity, store, result).ConfigureAwait(false);
                }
                return result;
            }

            public override async Task AddOrUpdateSubEntitiesAsync(IEnumerable entities, ImportFileResult result)
            {
                var store = _provider.GetRequiredService<IAdminStore<T>>();
                foreach (var entity in entities)
                {
                    await AddOrUpdateEntityAsync(entity as T, store, result).ConfigureAwait(false);
                }
            }

            public override async Task RemoveEntitiesAsync(IEnumerable entities, ImportFileResult result)
            {                
                var store = _provider.GetRequiredService<IAdminStore<T>>();
                foreach (var entity in entities)
                {
                    await RemoveEntityAsync(entity as T, store, result).ConfigureAwait(false);
                }
            }

            private async Task RemoveEntityAsync(T entity, IAdminStore<T> store, ImportFileResult result)
            {
                await store.DeleteAsync(entity.Id).ConfigureAwait(false);
                result.Deleted.Add(entity.Id);
            }

            private async Task AddOrUpdateEntityAsync(T entity, IAdminStore<T> store, ImportFileResult result)
            {
                var subEntities = GetSubEntities(entity);
                var existing = await store.GetAsync(entity.Id, null).ConfigureAwait(false);
                if (existing != null)
                {
                    entity = await store.UpdateAsync(entity).ConfigureAwait(false);
                    result.Updated.Add(entity.Id);
                }
                else
                {
                    entity = await store.CreateAsync(entity).ConfigureAwait(false);
                    result.Created.Add(entity.Id);
                }

                var subResults = new ImportFileResult();
                result.SubEntitiesResults.Add(entity.Id, subResults);
                await ImportSubEntitiesAsync(entity, subEntities, store, subResults).ConfigureAwait(false);
            }

            private async Task ImportSubEntitiesAsync(T entity, Dictionary<string, IEnumerable> subEntities, IAdminStore<T> store, ImportFileResult result)
            {                
                if (!subEntities.Any())
                {
                    return;
                }

                var expand = string.Join(",", subEntities.Keys);
                entity = await store.GetAsync(entity.Id, new GetRequest { Expand = expand }).ConfigureAwait(false);

                foreach (var key in subEntities.Keys)
                {
                    if (subEntities[key] == null)
                    {
                        continue;
                    }

                    var property = typeof(T).GetProperty(key);
                    var subEntityList = property.GetValue(entity) as IEnumerable;
                    var entityType = property.PropertyType.GetGenericArguments()[0];
                    var importerType = typeof(Importer<>).MakeGenericType(entityType);
                    var importer = Activator.CreateInstance(importerType, _provider) as Importer;
                    await importer.RemoveEntitiesAsync(subEntityList, result).ConfigureAwait(false);
                }

                foreach (var entityList in subEntities)
                {
                    var enumerator = entityList.Value.GetEnumerator();
                    if (!enumerator.MoveNext())
                    {
                        continue;
                    }

                    var entityType = enumerator.Current.GetType();
                    var importerType = typeof(Importer<>).MakeGenericType(entityType);
                    var importer = Activator.CreateInstance(importerType, _provider) as Importer;

                    await importer.AddOrUpdateSubEntitiesAsync(entityList.Value, result).ConfigureAwait(false);
                }
            }

            private Dictionary<string, IEnumerable> GetSubEntities(T entity)
            {
                var collectionPropertyList = typeof(T).GetProperties().Where(p => p.PropertyType.IsGenericType && p.PropertyType.GetInterface(typeof(IEnumerable<>).Name) != null);
                var dictionary = new Dictionary<string, IEnumerable>(collectionPropertyList.Count());
                foreach(var property in collectionPropertyList)
                {
                    if (!(property.GetValue(entity) is IEnumerable values))
                    {
                        continue;
                    }
                    dictionary[property.Name] = values;
                    property.SetValue(entity, null); // remove sub entities from entity object
                }
                return dictionary;
            }
        }

        abstract class Importer
        {
            public abstract Task<ImportFileResult> ImportAsync(string content);

            public abstract Task AddOrUpdateSubEntitiesAsync(IEnumerable entities, ImportFileResult result);

            public abstract Task RemoveEntitiesAsync(IEnumerable entities, ImportFileResult result);
        }
    }
}
