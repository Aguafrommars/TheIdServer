using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.Admin.Services.WindowsAuthentication;

/// <summary>
/// Authenticates requests using Negotiate, Kerberos, or NTLM.
/// </summary>
public class WindowsHandler : AuthenticationHandler<WindowsOptions>, IAuthenticationRequestHandler
{
    private readonly NegotiateHandler _innerHanlder;

    /// <summary>
    /// Creates a new <see cref="WindowsHandler"/>
    /// </summary>
    /// <inheritdoc />
    public WindowsHandler(IOptionsMonitor<WindowsOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
        : base(options, logger, encoder, clock)
    {
        _innerHanlder = new NegotiateHandler(options, logger, encoder, clock);        
    }

    /// <inheritdoc/>
    public Task<bool> HandleRequestAsync()
    => _innerHanlder.HandleRequestAsync();

    /// <inheritdoc/>
    protected override Task InitializeHandlerAsync()
    => _innerHanlder.InitializeAsync(Scheme, Request.HttpContext);

    /// <inheritdoc/>
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    => _innerHanlder.AuthenticateAsync();

    /// <inheritdoc/>
    protected override Task HandleChallengeAsync(AuthenticationProperties properties)
    => _innerHanlder.ChallengeAsync(properties);

    /// <inheritdoc/>
    protected override Task HandleForbiddenAsync(AuthenticationProperties properties)
    => _innerHanlder.ForbidAsync(properties);
}
