# TheIdServer Admin Application

This project contains the code of the [Blazor wasm](https//blazor.net) application to manage an [IdentityServer4](https://identityserver4.readthedocs.io/en/latest/)

## Configuration

The application reads its configuration from *appsettings.json* and environment specific configuration data from *appsettings.{environment}.json*

**appsettings.json**

```json
{
  "administratorEmail": "aguacongas@gmail.com",
  "apiBaseUrl": "https://localhost:5443/api",
  "authenticationPaths": {
    "remoteRegisterPath": "/identity/account/register",
    "remoteProfilePath": "/identity/account/manage"
  },
  "userOptions": {
    "roleClaim": "role"
  },
  "providerOptions": {
    "authority": "https://localhost:5443/",
    "clientId": "theidserveradmin",
    "defaultScopes": [
      "openid",
      "profile",
      "theidserveradminapi"
    ],
    "postLogoutRedirectUri": "https://localhost:5443/authentication/logout-callback",
    "redirectUri": "https://localhost:5443/authentication/login-callback",
    "responseType": "code"
  },
  "welcomeContenUrl": "/welcome-fragment.html"
}
```

For more informations read [ASP.NET Core Blazor hosting model configuration / Blazor WebAssembly / Configuration](https://docs.microsoft.com/en-us/aspnet/core/blazor/hosting-model-configuration?view=aspnetcore-3.1#configuration).

### apiBaseUrl

Defines the URL to the API.

### administratorEmail

Defines the adminitrator eMail address.

### authenticationPaths

The section **authenticationPaths** is binded to the class `Microsoft.AspNetCore.Components.WebAssembly.Authentication.RemoteAuthenticationApplicationPathsOptions`.  
The application doesn't contain pages to register a new user or manage the current user, so we set the **authenticationPaths:remoteRegisterPath** and **authenticationPaths:remoteProfilePath** with their corresponding url on the identity server.

 For more informations read [ASP.NET Core Blazor WebAssembly additional security scenarios / Customize app routes](https://docs.microsoft.com/en-us/aspnet/core/security/blazor/webassembly/additional-scenarios?view=aspnetcore-3.1#customize-app-routes).

### userOptions

The section **userOptions** is binded to the class `Microsoft.AspNetCore.Components.WebAssembly.Authentication.RemoteAuthenticationUserOptions`.  
This configuration defines how users are authorized. The application and the API share the same authorization policy : 

* **Is4-Writer** authorize users in this role to write data
* **Is4-Reader** authorize users in this role to read data

The role claims type is defined by **userOptions:roleClaim**.

### providerOptions

The section **providerOptions** is binded to the class `Microsoft.AspNetCore.Components.WebAssembly.Authentication.OidcProviderOptions`.  
This configuration section defines how the application is authentified.  

For more informations read [Secure an ASP.NET Core Blazor WebAssembly standalone app with the Authentication library / Authentication service support](https://docs.microsoft.com/en-us/aspnet/core/security/blazor/webassembly/standalone-with-authentication-library?view=aspnetcore-3.1#authentication-service-support).

### welcomeContenUrl

Defines the URL to the welcome page content.

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

## Additional resources

* [ASP.NET Core Blazor hosting model configuration](https://docs.microsoft.com/en-us/aspnet/core/blazor/hosting-model-configuration?view=aspnetcore-3.1#configuration)
* [ASP.NET Core Blazor WebAssembly additional security scenarios](https://docs.microsoft.com/en-us/aspnet/core/security/blazor/webassembly/additional-scenarios?view=aspnetcore-3.1#customize-app-routes)
* [Secure an ASP.NET Core Blazor WebAssembly standalone app with the Authentication library](https://docs.microsoft.com/en-us/aspnet/core/security/blazor/webassembly/standalone-with-authentication-library?view=aspnetcore-3.1#authentication-service-support)