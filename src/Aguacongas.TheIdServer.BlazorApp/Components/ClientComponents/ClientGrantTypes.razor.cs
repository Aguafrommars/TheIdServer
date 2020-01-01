using Aguacongas.TheIdServer.BlazorApp.Models;
using Microsoft.AspNetCore.Components;
using System;
using System.Linq;
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.TheIdServer.BlazorApp.Components.ClientComponents
{
    public partial class ClientGrantTypes
    {
        [Parameter]
        public Entity.Client Model { get; set; }

        [Parameter]
        public EventCallback<Entity.ClientGrantType> GrantTypeDeletedClicked { get; set; }

        [Parameter]
        public EventCallback<Entity.ClientGrantType> GrantTypeValueChanged { get; set; }

        private bool Validate(string inputValue)
        {
            Console.WriteLine($"Validate grant type {inputValue}");
            if (inputValue == null)
            {
                throw new ArgumentNullException(nameof(inputValue));
            }

            // spaces are not allowed in grant types
            if (inputValue.Contains(' '))
            {
                _notifier.Notify(new Notification
                {
                    Header = "Invalid grant type",
                    IsError = true,
                    Message = "The grant type cannot contains space."
                });
                return false;
            }

            var grantTypes = Model.AllowedGrantTypes;
            if (grantTypes.Any())
            {
                return true;
            }

            if (grantTypes.Any(g => g.GrantType == inputValue))
            {
                _notifier.Notify(new Notification
                {
                    Header = "Invalid grant type",
                    IsError = true,
                    Message = $"This client already contains the grant type {GetGrantTypeName(inputValue)}."
                });

                return false;
            }

            if (grantTypes.Any(g => g.GrantType == "implicit") &&
                (inputValue == "authorization_code" || inputValue == "hybrid"))
            {
                _notifier.Notify(new Notification
                {
                    Header = "Invalid grant type",
                    IsError = true,
                    Message = $"You cannot add {GetGrantTypeName(inputValue)} to a client with {GetGrantTypeName("implicit")} grant type."
                });

                return false;
            }
            if (grantTypes.Any(g => g.GrantType == "authorization_code") &&
                (inputValue == "implicit" || inputValue == "hybrid"))
            {
                _notifier.Notify(new Notification
                {
                    Header = "Invalid grant type",
                    IsError = true,
                    Message = $"You cannot add {GetGrantTypeName(inputValue)} to a client with {GetGrantTypeName("authorization_code")} grant type."
                });

                return false;

            }
            if (grantTypes.Any(g => g.GrantType == "hybrid") &&
                (inputValue == "implicit" || inputValue == "authorization_code"))
            {
                _notifier.Notify(new Notification
                {
                    Header = "Invalid grant type",
                    IsError = true,
                    Message = $"You cannot add {GetGrantTypeName(inputValue)} to a client with {GetGrantTypeName("hybrid")} grant type."
                });

                return false;
            }
            return true;
        }

        private string GetGrantTypeName(string key)
        {
            return GrantTypes.GetGrantTypeName(key);
        }
    }
}
