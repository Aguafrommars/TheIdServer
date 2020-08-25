# TheIdServer Admin Application

This project is the [Blazor Web Assembly](https//blazor.net) application to manage an [IdentityServer4](https://identityserver4.readthedocs.io/en/latest/) instance.

## Installation
### From Docker

The application is embedded in the [server's Linux image](../Aguacongas.TheIdServer/README.md#from-docker).  
If you prefer, you can install the [standalone application'sLinux image](https://hub.docker.com/r/aguacongas/aguacongastheidserverblazorapp).  
This image uses an [nginx](http://nginx.org/) server to host the application.

### From Github Release

The application is embedded in the [server's Github release](../Aguacongas.TheIdServer/README.md#from-github-release).  
You can choose to install the standalone application by selecting *Aguacongas.TheIdServer.BlazorApp{version}.zip* in the [list of releases](https://github.com/Aguafrommars/TheIdServer/releases).   
Unzip in the destination of your choice, and use the server of your choice.

Read [Host and deploy ASP.NET Core Blazor WebAssembly](https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/blazor/webassembly?view=aspnetcore-3.1) for more information.

### From NuGet packages

NuGet packages composing the application are available on [nuget.org](https://www.nuget.org/):

* **Aguacongas.TheIdServer.BlazorApp.Infrastructure** contains application models, services, validators and extensions
* **Aguacongas.TheIdServer.BlazorApp.Components** contains application components
* **Aguacongas.TheIdServer.BlazorApp.Pages** contains application pages

## Configuration

The application obtains its configuration from appsettings.json and the environment-specific settings from appsettings.{environment}.json.

**appsettings.json**

```json
{
  "administratorEmail": "aguacongas@gmail.com",
  "apiBaseUrl": "https://localhost:5443/api",
  "authenticationPaths": {
    "remoteRegisterPath": "/identity/account/register",
    "remoteProfilePath": "/identity/account/manage"
  },
  "loggingOptions": {
    "minimum": "Debug",
    "filters": [
      {       
        "category": "System",
        "level": "Warning"
      },
      {
        "category": "Microsoft",
        "level": "Information"
      }
    ]
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

For more details, read [ASP.NET Core Blazor hosting model configuration / Blazor WebAssembly / Configuration](https://docs.microsoft.com/en-us/aspnet/core/blazor/hosting-model-configuration?view=aspnetcore-3.1#configuration).

### apiBaseUrl

Defines the URL to the API.

### administratorEmail

Defines the administrator eMail address.

### authenticationPaths

The section **authenticationPaths** is binded to the class `Microsoft.AspNetCore.Components.WebAssembly.Authentication.RemoteAuthenticationApplicationPathsOptions`.  
The application doesn't contain pages to register a new user or manage the current user, so we set the **authenticationPaths:remoteRegisterPath** and **authenticationPaths:remoteProfilePath** with their corresponding URL on the identity server.

 For more information, read [ASP.NET Core Blazor WebAssembly additional security scenarios / Customize app routes](https://docs.microsoft.com/en-us/aspnet/core/security/blazor/webassembly/additional-scenarios?view=aspnetcore-3.1#customize-app-routes).

### loggingOptions

Defines logging options.

#### minimum

Defines the [log minimum level](https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.logging.loglevel?view=dotnet-plat-ext-3.1).

#### filters

Each item in this array adds a log filter by category and [LogLevel](https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.logging.loglevel?view=dotnet-plat-ext-3.1).

### userOptions

The section **userOptions** is bound to the class `Microsoft.AspNetCore.Components.WebAssembly.Authentication.RemoteAuthenticationUserOptions`.  
This configuration defines how users are authorized. The application and the API share the same authorization policy.

* **Is4-Writer** authorizes users in this role to write data.
* **Is4-Reader** permits users in this role to read data.

**userOptions:roleClaim** define the role claims type.

### providerOptions

The section **providerOptions** is binded to the class `Microsoft.AspNetCore.Components.WebAssembly.Authentication.OidcProviderOptions`.  
This configuration section defines the application authentication.  

For more details, read [Secure an ASP.NET Core Blazor WebAssembly standalone app with the Authentication library / Authentication service support](https://docs.microsoft.com/en-us/aspnet/core/security/blazor/webassembly/standalone-with-authentication-library?view=aspnetcore-3.1#authentication-service-support).

### welcomeContenUrl

Defines the URL to the welcome page content.

## Welcome page customization

Except for its title, the home page displays contents read from `welcomeContenUrl` endpoint.

This endpoint should return an HTML fragment.

[**sample**](../Aguacongas.TheIdServer/wwwroot/welcome-fragment.html)

```html
<p>
    This application manage your <a href="https://identityserver4.readthedocs.io/en/latest/">IdentityServer4</a>.
</p>
<p>
    Visit the <a href="https://github.com/aguacongas/TheIdServer">github site</a> for doc, source code and issue tracking.
</p>
<p>
    If you have trouble with login, disable Chromium cookies-without-same-site-must-be-secure flag.<br />
    <code>
        chrome://flags/#cookies-without-same-site-must-be-secure
    </code><br/>
    This site is running under a <a href="https://devcenter.heroku.com/articles/dyno-types">free heroku dyno</a> without end-to-end https.
</p>
<p>
    You can sign-in with <b>alice</b> to have reader/writer access, or <b>bob</b> for a read-only access.<br />
    The password is <i>Pass123$</i>.
</p>
```

## Client and API secrets

The application doesn't generate secrets. Shared Secrets or x509 certificates require conversion before storing. 

* Convert a SharedSecret to SHA256 value.
* Convert an X509 certificate to Base64 string.

## Additional resources

* [ASP.NET Core Blazor hosting model configuration](https://docs.microsoft.com/en-us/aspnet/core/blazor/hosting-model-configuration?view=aspnetcore-3.1#configuration)
* [ASP.NET Core Blazor WebAssembly additional security scenarios](https://docs.microsoft.com/en-us/aspnet/core/security/blazor/webassembly/additional-scenarios?view=aspnetcore-3.1#customize-app-routes)
* [Secure an ASP.NET Core Blazor WebAssembly standalone app with the Authentication library](https://docs.microsoft.com/en-us/aspnet/core/security/blazor/webassembly/standalone-with-authentication-library?view=aspnetcore-3.1#authentication-service-support)
* [LogLevel Enum](https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.logging.loglevel?view=dotnet-plat-ext-3.1)
