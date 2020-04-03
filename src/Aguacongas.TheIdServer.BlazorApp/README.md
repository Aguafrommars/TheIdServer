# TheIdServer Admin Application

This project contains the code of the [Blazor wasm](https//blazor.net) application to manage an [IdentityServer4](https://identityserver4.readthedocs.io/en/latest/)

## Configuration

The application reads its configuration from *appsettings.json* and environment specific configuration data from *appsettings.{environment}.json*

**appsettings.json**

```json
{
  "configurationEndpoint": "appsettings.json", // the endpoint uri returning OIDC configuration
  "apiBaseUrl": "https://localhost:5443/api", // the api endpoint uri
  "welcomeContenUrl": "/welcome-fragment.html", // the endpoint uri returning the welcome page html code 
  "administratorEmail": "aguacongas@gmail.com", // the administrator email
}
```

### configurationEndpoint

The configuration endpoint must return a JSON containing OIDC configuration for the application with the same structure than [oidc-client-js configuration](https://github.com/IdentityModel/oidc-client-js/wiki#configuration)

**full sample**

```json
{
  "client_id": "theidserveradmin", // application id
  "response_type": "code", // OAuth response type
  "scope": "openid profile theidserveradminapi", // requested scopes
  "authority": "https://theidserver.herokuapp.com/", // OAuth server address
  "redirect_uri": "https://theidserver.herokuapp.com/authentication/login-callback", // login redirect uri
  "post_logout_redirect_uri": "https://theidserver.herokuapp.com/authentication/logout-callback", // logout redirect uri
  "metadata": { // optional, this to override value return by IdentityServer4 descovery document. 
    "issuer": "http://theidserver.herokuapp.com",
    "jwks_uri": "https://theidserver.herokuapp.com/.well-known/openid-configuration/jwks",
    "authorization_endpoint": "https://theidserver.herokuapp.com/connect/authorize",
    "token_endpoint": "https://theidserver.herokuapp.com/connect/token",
    "userinfo_endpoint": "https://theidserver.herokuapp.com/connect/userinfo",
    "end_session_endpoint": "https://theidserver.herokuapp.com/connect/endsession",
    "check_session_iframe": "https://theidserver.herokuapp.com/connect/checksession"
  }
}
```

## Welcome page customization

Except its title, the home page displays contents read from `welcomeContenUrl` endpoint.

This endpoint should return an html fragment code.

[**sample**](../Aguacongas.TheIdServer/wwwroot/welcome-fragment.html)

```html
<p>
    This application manage your <a href="https://identityserver4.readthedocs.io/en/latest/">IdentityServer4</a>.
</p>
<p>
    Visit the <a href="https://github.com/aguacongas/TheIdServer">github site</a> for doc, source code and issue tracking.
</p>
<p>
    If you have trouble to login disable chromuim cookies-without-same-site-must-be-secure flag.<br />
    <code>
        chrome://flags/#cookies-without-same-site-must-be-secure
    </code><br/>
    This site is running under a <a href="https://devcenter.heroku.com/articles/dyno-types">free heroku dyno</a> without end-to-end https.
</p>
<p>
    You can sign-in with <b>alice</b> to have reader/writer access, or <b>bob</b> for a read only access.<br />
    The passord is <i>Pass123$</i>
</p>
```