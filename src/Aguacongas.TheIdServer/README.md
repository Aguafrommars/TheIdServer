# TheIdServer Web Server project

The configuration can be read from *appsettings.json*, *appsettings.{Environment}.json*, command line arguments or environement variables.

Read [Configuration in ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-3.1) for more information.

## Instalation

### From Docker

A [server's Linux image](https://hub.docker.com/r/aguacongas/aguacongastheidserver) is available on Doker hub.

[*sample/MultiTiers/Aguacongas.TheIdServer.Private/Dockerfile-private*](../../sample/MultiTiers/Aguacongas.TheIdServer.Private/Dockerfile-private) is a sample demonstrate how to create a image from the [server image](https://hub.docker.com/r/aguacongas/aguacongastheidserver) to run a private Linux server container.

[*sample/MultiTiers/Aguacongas.TheIdServer.Public/Dockerfile-public*](../../sample/MultiTiers/Aguacongas.TheIdServer.Public/Dockerfile-public) is a sample demonstrate how to create a image from the [server image](https://hub.docker.com/r/aguacongas/aguacongastheidserver) to run a public Linux server container.

Read [Hosting ASP.NET Core images with Docker over HTTPS](https://docs.microsoft.com/en-us/aspnet/core/security/docker-https?view=aspnetcore-3.1) to setup the HTTPS certificate.

#### Kubernetes sample

[/sample/Kubernetes/README.md](/sample/Kubernetes/README.md) contains a sample to setup a solution with Kubernetes.

### From Github Release

Choose your release in the [list of releases](https://github.com/Aguafrommars/TheIdServer/releases) and download the server zip.   
Unzip in the destination of your choice, as any ASP.Net Core web site it can run in IIS or as a stand alone server using the plateform of your choice.

Read [Host and deploy ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/?view=aspnetcore-3.1) for more information.

### From Nuget Packages

If you need more customization, you can use published Nuget packages.
[sample/MultiTiers](sample/MultiTiers) contains a sample to build server and API from Nuget packages.

## Configure IdentityServer4

The section **IdentityServerOptions** is binded to the class [`IdentityServer4.Configuration.IdentityServerOptions`](http://docs.identityserver.io/en/latest/reference/options.html).  
So you can set any IdentityServer4 options you want from configuration

```json
"IdentityServerOptions": {
  "Events": {
    "RaiseErrorEvents": true,
    "RaiseInformationEvents": true,
    "RaiseFailureEvents": true,
    "RaiseSuccessEvents": true
  }
}
```

## Configure stores

### Using EF core

The server supports *SqlServer*, *Sqlite*, *MySql*, *PostgreSQL*, *Oracle* and *InMemory* databases.  
Use **DbType** to the define the dabase engine.

```json
"DbType": "SqlServer"
```

And **ConnectionStrings:DefaultConnection** to define the connection string.

```json
"ConnectionStrings": {
  "DefaultConnection": "Data Source=(LocalDb)\\MSSQLLocalDB;database=TheIdServer;trusted_connection=yes;"
}
```

> For Oracle database, you need a [devart dotConnect for Oracle](https://www.devart.com/dotconnect/oracle/) license.

### Using the API

If you don't want to expose a database with your server, you can setup a second server on a private network accessing the db and use this private server api to access data.

```json
"Proxy": true,
"PrivateServerAuthentication": {
  "Authority": "https://theidserverprivate",
  "ApiUrl": "https://theidserverprivate/api",
  "ClientId": "public-server",
  "ClientSecret": "84137599-13d6-469c-9376-9e372dd2c1bd",
  "Scope": "theidserveradminapi",
  "HttpClientName": "is4"
},
"SignalR": {
  "HubUrl": "https://theidserverprivate/providerhub"
  "HubOptions": {
    "EnableDetailedErrors": true
  },
  "UseMessagePack": true
}
```

#### Proxy

Start the server with in proxy mode.

#### PrivateServerAuthentication

Defines how to authenticate the public server on private server API.

#### SignalR

Defines the [SignalR client](https://docs.microsoft.com/en-us/aspnet/core/signalr/dotnet-client?view=aspnetcore-3.1&tabs=visual-studio) configuration.  
This client is used to update the external provider configuration of a running instance. When an external provider configuration changes, the API send a SignalR notification to inform other running instances.  

For more informations read [Load balancing scenario](https://github.com/Aguafrommars/DymamicAuthProviders/wiki/Load-balancing-scenario).

The SignalR hub accept request at */providerhub* and support [MessagePack](https://msgpack.org/index.html) protocol.

For more informations read [Use MessagePack Hub Protocol in SignalR for ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/signalr/messagepackhubprotocol?view=aspnetcore-3.1).

### Database migration and data seeding

You can start the server with **/seed** command line argument to create the initial DB with initial data. Or configure the server with :

```json
"Migrate": true,
"Seed": true
```

This will create the database with initial users, protected resources, identity resources and clients.

#### Roles

* **Is4-Writer** authorize users in this role to write data
* **Is4-Reader** authorize users in this role to read data

#### Users

* **alice** (pwd *Pass123$*) with roles **Is4-Writer** and **Is4-Reader**
* **bob** (pwd *Pass123$*) with role **Is4-Reader**

#### Protected resouces (API)

* **theidserveradminapi** the server API asking for claims **name** and **role**
* **api1** a sample api 

#### Identity resources

* **profile** default profile resource with **role** claim.
* **openid** default openid resource
* **address** default address resource
* **email** default email resource
* **phone** default phone resource

#### Clients

* **theidserveradmin** the admin app client
* **public-server** the client to use a server as proxy
* **theidserver-swagger** the client for the API documentation
* **client** a client credential flow sample client
* **mvc** a hybrid and client credential flow sample client
* **spa** an authorization code flow sample client
* **device** a device flow sample client

## Configure Credentials

### From file

```json
"IdentityServer": {
  "Key": {
    "Type": "File",
    "FilePath": "{path to the .pfx}",
    "Password":  "{.pfx password}"
  }
}
```

### From store

Read [Example: Deploy to Azure Websites](https://docs.microsoft.com/en-us/aspnet/core/security/authentication/identity-api-authorization?view=aspnetcore-3.1#example-deploy-to-azure-websites)

## Configure the Email service

By default the server use [SendGrid](https://sendgrid.com/) to send Emails by calling the api at */api/email*

```json
"SendGridUser": "your user",
"SendGridKey": "your SendGrid key"
```

### Use your API

If you prefer to use your Email sender, implement a Web API receiving a POST request with the json:

```json
{
  "subject": "Email subject",
  "message": "Email message",
  "addresses": [
    "an-address@aguacongas.con"
  ]
}
```

And update the *EmailApiAuthentication* configuration section:

```json
"EmailApiAuthentication": {
  "Authority": "https://localhost:5443",
  "ApiUrl": "https://localhost:5443/api/email",
  "ClientId": "public-server",
  "ClientSecret": "84137599-13d6-469c-9376-9e372dd2c1bd",
  "Scope": "theidserveradminapi",
  "HttpClientName": "email"
}
```

> If you want to use the same authentication configuration and token for both *EmailApi* and *PrivateServer* you can simplify the configuration by sharing the same **HttpClientName**

```json
"EmailApiAuthentication": {
  "ApiUrl": "https://localhost:5443/api/email",
  "HttpClientName": "is4"
}
```

## Configure the 2fa authenticator issuer

By default the issuer for 2fa authenticator is **Aguacongas.TheIdServer**.  
To update this value set **AuthenticatorIssuer** with your issuer

```json
"AuthenticatorIssuer": "TheIdServer"
```

## Configure the API

### Authentication

The section *ApiAuthentication* define the authentication configuration for the API.

```json
"ApiAuthentication": {
  "Authority": "https://localhost",
  "RequireHttpsMetadata": false,
  "SupportedTokens": "Both",
  "ApiName": "theidserveradminapi",
  "ApiSecret": "5b556f7c-b3bc-4b5b-85ab-45eed0cb962d",
  "EnableCaching": true,
  "CacheDuration": "0:10:0",
  "LegacyAudienceValidation": true
}
```

### Documentation endpoint

To enable the API documentation, set **EnableOpenApiDoc** to `true`

```json
"EnableOpenApiDoc": true
```

Use the section *SwaggerUiSettings* to configure the swagger client authentication

```json
"SwaggerUiSettings": {
  "OAuth2Client": {
    "ClientId": "theidserver-swagger",
    "AppName": "TheIdServer Swagger UI",
    "UsePkceWithAuthorizationCodeGrant": true
  },
  "WithCredentials": true
}
```

### CORS

The section *CorsAllowedOrigin* define allowed CORS origins

```json
"CorsAllowedOrigin": [
  "http://localhost:5001"
]
```

## Configure HTTPS

If you want to diseable HTTPS set **DisableHttps** to `false`

```json
"DisableHttps": true
```

If you use a self signed certificat you can disable strict SSL by settings **DisableStrictSsl** to `true`.

```json
"DisableStrictSsl": true
```

### Configure Forwarded Headers

The section **ForwardedHeadersOptions** is binded to the class [`Microsoft.AspNetCore.Builder.ForwardedHeadersOptions`](https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.builder.forwardedheadersoptions?view=aspnetcore-3.1).  

```json
"ForwardedHeadersOptions": {
  "ForwardedHeaders": "All"
}
```

## Configure the provider hub

External providers are dynamicaly configured with [Aguacongas.AspNetCore.Authentication library](https://github.com/Aguafrommars/DymamicAuthProviders).  
In a [load balanced](https://github.com/Aguafrommars/DymamicAuthProviders/wiki/Load-balancing-scenario) configuration, the provider hub is used to inform other running instances that an external provider configuration changes.  
The **SignalR** section defines the configuration for both SignalR hub and client.

```json
"SignalR": {
  "HubUrl": "https://theidserverprivate/providerhub",
  "HubOptions": {
    "EnableDetailedErrors": true
  },
  "UseMessagePack": true,
  "RedisConnectionString": "redis:6379",
  "RedisOptions": {
    "Configuration": {
      "ChannelPrefix": "TheIdServer"
    }
  }
}
```

If needed, the hub can use [Redis backplane](https://docs.microsoft.com/en-us/aspnet/core/signalr/redis-backplane?view=aspnetcore-3.1) can be used. **SignalR:RedisConnectionString** and **SignalR:RedisOptions** configure the backplane.  
**SignalR:RedisOptions** is binded to an instance of [`Microsoft.AspNetCore.SignalR.StackExchangeRedis.RedisOptions`](https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.signalr.stackexchangeredis.redisoptions?view=aspnetcore-3.0) at startup.

## Configre logs

The section **Serilog** define the [Serilog](https://serilog.net/) configuration.

```json
"Serilog": {
  "LevelSwitches": {
    "$controlSwitch": "Information"
  },
  "MinimumLevel": {
    "ControlledBy": "$controlSwitch"
  },
  "WriteTo": [
    {
      "Name": "Seq",
      "Args": {
        "serverUrl": "http://localhost:5341",
        "controlLevelSwitch": "$controlSwitch",
        "apiKey": "DVYuookX2vOq078fuOyJ"
      }
    },
    {
      "Name": "Console",
      "Args": {
        "outputTemplate": "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}",
        "theme": "Serilog.Sinks.SystemConsole.Themes.AnsiConsoleTheme::Literate, Serilog.Sinks.Console"
      }
    },
    {
      "Name": "Debug",
      "Args": {
        "outputTemplate": "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}"
      }
    }
  ],
  "Enrich": [
    "FromLogContext",
    "WithMachineName",
    "WithThreadId"
  ]
}
```
For more informations read [Serilog.Settings.Configuration](https://github.com/serilog/serilog-settings-configuration/blob/dev/README.md).

## Configure claims providers

The claims providers configuration is described in [Claims provider](../../doc/CLAIMS_PROVIDER.md)

## Additional resources

* [Host and deploy ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/?view=aspnetcore-3.1)
* [DymamicAuthProviders](https://github.com/Aguafrommars/DymamicAuthProviders)
* [Set up a Redis backplane for ASP.NET Core SignalR scale-out](https://docs.microsoft.com/en-us/aspnet/core/signalr/redis-backplane?view=aspnetcore-3.1)
* [Microsoft.AspNetCore.SignalR.StackExchangeRedis.RedisOptions](https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.signalr.stackexchangeredis.redisoptions?view=aspnetcore-3.0)
* [Serilog.Settings.Configuration](https://github.com/serilog/serilog-settings-configuration/blob/dev/README.md)
* [Hosting ASP.NET Core images with Docker over HTTPS](https://docs.microsoft.com/en-us/aspnet/core/security/docker-https?view=aspnetcore-3.1)

