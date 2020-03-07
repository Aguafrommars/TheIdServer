using Aguacongas.TheIdServer.BlazorApp.Models;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Aguacongas.TheIdServer.BlazorApp.Shared
{
    public partial class MainLayout
    {
        private string _loginError;
        private bool _notified;
        private bool _pending;
        private bool _login;
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
            _pending = _login = true;
            try
            {
                _provider.Login();
            }
            catch (Exception e)
            {
                _loginError = e.Message;
                await InvokeAsync(StateHasChanged).ConfigureAwait(false);
            }
        }

        private async Task Logoff()
        {
            _login = false;
            _pending = true;
            await _provider.LogoffAsync();
        }
    }
}
