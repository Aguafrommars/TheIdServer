# TheIdServer Web Server project

The configuration can be read from *appsettings.json*, *appsettings.{Environment}.json*, command line arguments or environement variables.

Read [Configuration in ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-3.1) for more information.

## Configure stores

### Using EF core

The server supports *SqlServer*, *Sqlite* and *InMemory* databases.  
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

### Using the API

If you don't want to expose a database with your server, you can setup a second server on a private network accessing the db and use this private server api to access data.

```json
"Proxy": true,
"PrivateServerAuthentication": {
  "Authority": "https://localhost:7443",
  "ApiUrl": "https://localhost:7443/api",
  "ClientId": "public-server",
  "ClientSecret": "84137599-13d6-469c-9376-9e372dd2c1bd",
  "Scope": "theidserveradminapi",
  "HttpClientName": "is4"
}
```

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
  "Authority": "https://localhost:5443",
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

If you want to diseable HTTPS set **UseHttps** to `false`

```json
"UseHttps": false
```