using Aguacongas.IdentityServer.Abstractions;
using Aguacongas.IdentityServer.Admin.Models;
using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
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
            var storeType = typeof(IAdminStore<>).MakeGenericType(entityType);
            var store = _provider.GetRequiredService(storeType);
            var importerType = typeof(Importer<>).MakeGenericType(entityType);
            var importer = Activator.CreateInstance(importerType, store) as Importer;
            var result = await importer.ImportAsync(content).ConfigureAwait(false);
            result.FileName = file.FileName;
            return result;
        }

        class Importer<T> : Importer where T : class, IEntityId
        {
            private readonly IAdminStore<T> _store;

            [SuppressMessage("Major Code Smell", "S1144:Unused private types or members should be removed", Justification = "Called by reflexion")]
            public Importer(IAdminStore<T> store)
            {
                _store = store;
            }

            public override async Task<ImportFileResult> ImportAsync(string content)
            {
                var result = new ImportFileResult();
                var entityList = JsonConvert.DeserializeObject<PageResponse<T>>(content);
                foreach (var entity in entityList.Items)
                {
                    var existing = await _store.GetAsync(entity.Id, null).ConfigureAwait(false);
                    if (existing != null)
                    {
                        await _store.UpdateAsync(entity).ConfigureAwait(false);
                        result.Updated.Add(entity.Id);
                        continue;
                    }

                    var created = await _store.CreateAsync(entity).ConfigureAwait(false);
                    result.Created.Add(created.Id);
                }
                return result;
            }
        }

        abstract class Importer
        {
            public abstract Task<ImportFileResult> ImportAsync(string content);
        }
    }
}
