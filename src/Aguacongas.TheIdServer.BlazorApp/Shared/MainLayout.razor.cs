using Aguacongas.TheIdServer.BlazorApp.Models;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Aguacongas.TheIdServer.BlazorApp.Shared
{
    public partial class MainLayout
    {
        private readonly object _syncObject = new object();
        private bool _pendingLoging = false;
        private string _loginError;
        private bool _notified;
        private string ShowToast(string userName)
        {
            if (!_notified)
            {
                _notifier.Notify(new Notification
                {
                    Header = userName,
                    Message = "Connected"
                });
                _notified = true;
            }
            return null;
        }

        [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "We want to catch all exception here.")]
        private async Task LoginAsync()
        {
            if (_pendingLoging)
            {
                return;
            }
            lock (_syncObject)
            {
                if (_pendingLoging)
                {
                    return;
                }
                _pendingLoging = true;
            }

            try
            {
                await _provider.LoginAsync()
                    .ConfigureAwait(false);
            }
            catch (Exception e)
            {
                _loginError = e.Message;
                await InvokeAsync(StateHasChanged).ConfigureAwait(false);
            }
        }
    }
}
