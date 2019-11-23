using Aguacongas.IdentityServer.Admin.Http.Store;
using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Aguacongas.TheIdServer.BlazorApp.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aguacongas.TheIdServer.BlazorApp.Pages
{
    public abstract class EntityModel<T> : ComponentBase, IComparer<Type> where T : class, IEntityId, ICloneable<T>, new()
    {
        const int HEADER_HEIGHT = 95;

        [Inject]
        protected Notifier Notifier { get; set; }

        [Inject]
        protected NavigationManager NavigationManager { get; set; }

        [Inject]
        protected IAdminStore<T> AdminStore { get; set; }

        [Inject]
        protected IServiceProvider Provider { get; set; }

        [Inject]
        protected IJSRuntime JSRuntime { get; set; }

        [Parameter]
        public string Id { get; set; }

        protected bool IsNew { get; private set; }

        protected T Model { get; private set; }

        protected T State { get; set; }

        protected EditContext EditContext { get; private set; }

        protected abstract string Expand { get; }

        protected abstract bool NonEditable { get; }

#pragma warning disable CA1056 // Uri properties should not be strings. Nope, because it's used as parameter of NavigationManager.NavigateTo
        protected abstract string BackUrl { get; }
#pragma warning restore CA1056 // Uri properties should not be strings

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
                State = Model.Clone();
                return;
            }

            Model = await AdminStore.GetAsync(Id, new GetRequest
            {
                Expand = Expand
            }).ConfigureAwait(false);
            CreateEditContext(Model);
            State = Model.Clone();
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
            Id = Model.Id;
            IsNew = false;
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

        protected async Task DeleteEntity()
        {
            try
            {
                await AdminStore.DeleteAsync(Model.Id)
                    .ConfigureAwait(false);
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

            NavigationManager.NavigateTo(BackUrl);

            Notifier.Notify(new Models.Notification
            {
                Header = Model.Id,
                Message = "Deleted"
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

        protected ValueTask ScrollTo(string id)
        {
            return JSRuntime.InvokeVoidAsync("browserInteropt.scrollTo", id, -HEADER_HEIGHT);
        }
        protected abstract T Create();
        protected abstract void SetNavigationProperty<TEntity>(TEntity entity);

        protected abstract void SanetizeEntityToSaved<TEntity>(TEntity entity);

        protected virtual void SetModelEntityId(Type entityType, IEntityId result)
        {
        }

        protected virtual Type GetEntityType(FieldIdentifier identifier)
        {
            return identifier.Model.GetType();
        }

        protected virtual IEntityId GetEntityModel(FieldIdentifier identifier)
        {
            return identifier.Model as IEntityId;
        }

        private async Task HandleMoficationList(Type entityType, Dictionary<IEntityId, ModificationKind> modificationList)
        {
            Console.WriteLine($"HandleMoficationList for type {entityType.Name}");
            var addList = GetModifiedEntities(modificationList, ModificationKind.Add);
            foreach (var entity in addList)
            {
                SetNavigationProperty(entity);
                SanetizeEntityToSaved(entity);
                var result = await CreateAsync(entityType, entity)
                    .ConfigureAwait(false);
                entity.Id = result.Id;
                SetModelEntityId(entityType, result);
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
                var identifier = e.FieldIdentifier;
                var entityType = GetEntityType(identifier);
                var entityModel = GetEntityModel(identifier);
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
