using Aguacongas.IdentityServer.Admin.Http.Store;
using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Aguacongas.TheIdServer.BlazorApp.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aguacongas.TheIdServer.BlazorApp.Pages
{
    public abstract class EntityModel<T> : ComponentBase, IComparer<Type> where T : class, IEntityId, ICloneable<T>, new()
    {
        [Inject]
        public Notifier Notifier { get; set; }

        [Inject]
        public IAdminStore<T> AdminStore { get; set; }

        [Inject]
        public IServiceProvider Provider { get; set; }

        [Parameter]
        public string Id { get; set; }

        protected bool IsNew { get; private set; }

        protected T Model { get; private set; }

        protected T State { get; set; }

        protected EditContext EditContext { get; private set; }

        protected abstract string Expand { get; }

        protected abstract bool NonEditable { get; }

        private readonly Dictionary<Type, Dictionary<IEntityId, ModificationKind>> _changes =
            new Dictionary<Type, Dictionary<IEntityId, ModificationKind>>();

        public virtual int Compare(Type x, Type y)
        {
            if (x == typeof(T))
            {
                return -1;
            }
            if (y == typeof(T))
            {
                return 1;
            }
            return 0;
        }

        protected override async Task OnInitializedAsync()
        {
            if (Id == null)
            {
                IsNew = true;
                Model = Create();
                CreateEditContext(Model);
                EntityCreated(Model);
                return;
            }

            Model = await AdminStore.GetAsync(Id, new GetRequest
            {
                Expand = Expand
            }).ConfigureAwait(false);
            CreateEditContext(Model);
        }

        protected async Task HandleValidSubmit()
        {
            if (!_changes.Any())
            {
                Notifier.Notify(new Models.Notification
                {
                    Header = Model.Id,
                    IsError = false,
                    Message = "No changes"
                });
                return;
            }

            State = Model.Clone();
            Model = State.Clone();
            StateHasChanged();

            try
            {
                var keys = _changes.Keys
                    .OrderBy(k => k, this);
                
                foreach (var key in keys)
                {
                    await HandleMoficationList(key, _changes[key])
                        .ConfigureAwait(false);
                }
            }
            catch (ProblemException pe)
            {
                Notifier.Notify(new Models.Notification
                {
                    Header = "Error",
                    IsError = true,
                    Message = pe.Details.Detail
                });
                throw;
            }
#pragma warning disable CA1031 // Do not catch general exception types. We want to notify all error
            catch (Exception e)
#pragma warning restore CA1031 // Do not catch general exception types
            {
                Notifier.Notify(new Models.Notification
                {
                    Header = "Error",
                    IsError = true,
                    Message = e.Message
                });
                throw;
            }
            finally
            {
                _changes.Clear();
            }

            Notifier.Notify(new Models.Notification
            {
                Header = Model.Id,
                Message = "Saved"
            });
        }

        protected void EntityCreated<TEntity>(TEntity entity) where TEntity : class, IEntityId
        {
            entity = entity ?? throw new ArgumentNullException(nameof(entity));
            var entityType = typeof(TEntity);
            var modifications = GetModifications(entityType);
            Console.WriteLine($"Add created change for entity {entityType.Name} {entity.Id}");
            modifications.Add(entity, ModificationKind.Add);
        }

        protected void EntityDeleted<TEntity>(TEntity entity) where TEntity : class, IEntityId
        {
            entity = entity ?? throw new ArgumentNullException(nameof(entity));
            var entityType = typeof(TEntity);
            var modifications = GetModifications(entityType);
            if (entity.Id == null)
            {
                Console.WriteLine($"Remove change for entity {entityType.Name}");
                modifications.Remove(entity);
                return;
            }
            Console.WriteLine($"Add delete change for entity {entityType.Name} {entity.Id}");
            modifications.Add(entity, ModificationKind.Delete);
        }

        protected abstract T Create();
        protected abstract void SetNavigationProperty<TEntity>(TEntity entity);

        protected abstract void SanetizeEntityToSaved<TEntity>(TEntity entity);

        private async Task HandleMoficationList(Type entityType, Dictionary<IEntityId, ModificationKind> modificationList)
        {
            Console.WriteLine($"HandleMoficationList for type {entityType.Name}");
            var addList = GetModifiedEntities(modificationList, ModificationKind.Add);
            foreach (var entity in addList)
            {
                SetNavigationProperty(entity);
                SanetizeEntityToSaved(entity);
                await CreateAsync(entityType, entity)
                    .ConfigureAwait(false);
            }
            var updateList = GetModifiedEntities(modificationList, ModificationKind.Update);
            foreach (var entity in updateList)
            {
                SanetizeEntityToSaved(entity);
                await UpdateAsync(entityType, entity)
                    .ConfigureAwait(false);
            }
            var deleteList = GetModifiedEntities(modificationList, ModificationKind.Delete);
            foreach (var entity in deleteList)
            {
                await DeleteAsync(entityType, entity)
                    .ConfigureAwait(false);
            }
        }

        private static IEnumerable<IEntityId> GetModifiedEntities(Dictionary<IEntityId, ModificationKind> modificationList, ModificationKind kind)
        {
            return modificationList
                            .Where(m => m.Value == kind)
                            .Select(m => m.Key);
        }

        private Task<IEntityId> UpdateAsync(Type entityType, IEntityId entity)
        {
            return StoreAsync(entityType, entity, (store, e) =>
           {
               return store.UpdateAsync(e);
           });
        }

        private Task<IEntityId> DeleteAsync(Type entityType, IEntityId entity)
        {
            return StoreAsync(entityType, entity, async (store, e) =>
            {
                await store.DeleteAsync(e.Id)
                .ConfigureAwait(false);
                return e;
            });
        }

        private Task<IEntityId> CreateAsync(Type entityType, IEntityId entity)
        {
            return StoreAsync(entityType, entity, (store, e) =>
            {
                return store.CreateAsync(e);
            });
        }

        private void CreateEditContext(T model)
        {
            EditContext = new EditContext(model);
            EditContext.OnFieldChanged += (s, e) =>
            {
                var identitfier = e.FieldIdentifier;
                var entityType = identitfier.Model.GetType();
                var entityModel = identitfier.Model as IEntityId;
                var modifications = GetModifications(entityType);

                if (!modifications.ContainsKey(entityModel))
                {
                    Console.WriteLine($"Add update modification for entity {entityType} {entityModel.Id}");
                    modifications.Add(entityModel, ModificationKind.Update);
                }
            };
        }

        private Dictionary<IEntityId, ModificationKind> GetModifications(Type entityType)
        {
            if (!_changes.TryGetValue(entityType, out Dictionary<IEntityId, ModificationKind> modifications))
            {
                modifications = new Dictionary<IEntityId, ModificationKind>();
                _changes.Add(entityType, modifications);
            }

            return modifications;
        }

        private async Task<IEntityId> StoreAsync(Type entityType, IEntityId entity, Func<IAdminStore, IEntityId, Task<IEntityId>> action)
        {
            entity = entity ?? throw new ArgumentNullException(nameof(entity));

            if (NonEditable)
            {
                throw new InvalidOperationException("The entity is non editable");
            }

            var store = Provider.GetRequiredService(typeof(IAdminStore<>).MakeGenericType(entityType)) as IAdminStore;
            entity = await action.Invoke(store, entity)
                .ConfigureAwait(false);

            return entity;
        }

        private enum ModificationKind
        {
            Add,
            Update,
            Delete
        }
    }
}
