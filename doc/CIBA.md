# Dudende CIBA Integration

The Duende version of TheIdServer integrate [Client Initiated Backchannel Authentication (CIBA)](https://docs.duendesoftware.com/identityserver/v6/ui/ciba/).

## User validation

It implements [IBackchannelAuthenticationUserValidator](https://docs.duendesoftware.com/identityserver/v6/reference/validators/ciba_user_validator/) to identified the user from the backchannel authentication request sent by the client with **login_hint**, **id_token_hint** or **login_hint_token**.  

* If the request contains **login_hint**, the user is found by her user name.
* If the request contains **login_hint_token**, the user is found by her id.
* If the request contains **id_token_hint**, the **sub** claim is use to find the user by her id.

[Implementation code](../src/IdentityServer/Duende/Aguacongas.IdentityServer.Duende/Services/BackchannelAuthenticationUserNotificationService.cs)

## Notification service

The default implementation of [IBackchannelAuthenticationUserNotificationService](https://docs.duendesoftware.com/identityserver/v6/reference/services/ciba_user_notification/) send an EMail to the user eMail address containing a link the consent page.  

To send the Email it use the server eMail service used to register users. See [Configure the Email service](../src/Aguacongas.TheIdServer.Duende/README.md#Configure-the-Email-service). 

This service is configured with the **BackchannelAuthenticationUserNotificationServiceOptions** section : 

```json
"BackchannelAuthenticationUserNotificationServiceOptions": {
  "Authority": "https://localhost:5443",
  "ApiUrl": "https://localhost:5443/api/email",
  "ClientId": "public-server",
  "ClientSecret": "84137599-13d6-469c-9376-9e372dd2c1bd",
  "Scope": "theidserveradminapi",
  "HttpClientName": "ciba"
}
```

You must update this section if you want to use your Email service instead of [SendGrid](https://sendgrid.com/).

### Use your implementation

If you prefer to implement [IBackchannelAuthenticationUserNotificationService](https://docs.duendesoftware.com/identityserver/v6/reference/services/ciba_user_notification/) you can provide your assembly and class type in the configuration.

exemple:

```json
"BackchannelAuthenticationUserNotificationServiceOptions": {
  "AssemblyPath": "MyImplementation.dll",
  "ServiceType": "MyBackchannelAuthenticationUserNotificationService, MyImplementation"
}
```

The DI will try to load the assembly and create an object with the first constructor found and inject depencies this construcor required.

```cs
.AddTransient(p =>
{
    var settings = p.GetRequiredService<IOptions<BackchannelAuthenticationUserNotificationServiceOptions>>().Value;

    var serviceType = GetBackchannelAuthenticationUserNotificationServiceType(settings);
    var constructor = serviceType.GetConstructors()[0];
    var parameters = constructor.GetParameters();
    var arguments = new object[parameters.Length];
    for (var i = 0; i < parameters.Length; i++)
    {
        var parameterType = parameters[i].ParameterType;
        if (parameterType == typeof(HttpClient))
        {
            var factory = p.GetRequiredService<IHttpClientFactory>();
            arguments[i] = factory.CreateClient(settings.HttpClientName ?? "ciba");
            continue;
        }
        arguments[i] = p.GetRequiredService(parameterType);
    }

    return constructor.Invoke(arguments) as IBackchannelAuthenticationUserNotificationService;
})
```

## Sample

* Configure an application *ciba* with *Ciba* grant type in your server.  
![ciba client](assets/Ciba-client.jpeg)  
* Update the alice user Email with your Email.
![alice email](assets/alice-email.jpeg)    
* Launch the sample [CIBA application sample](../sample/Aguacongas.TheIdServer.CibaSample).  
```bash
❯ dotnet run
Login Hint                  : alice
Binding Message             : 19d2f89f14
Authentication Request Id   : 2df94fc3-9224-4119-a23b-4a9a94e8a328
Expires In                  : 200
Interval                    : 10
 ```
* Click the link received in the Email  
![ciba email](assets/ciba-email.jpeg)
* Login with alice user
![ciba request](assets/ciba-request.jpeg)
* Click Yes and press enter on the client
```bash
Token response:
{"id_token":"eyJhbGciOiJSUzI1NiIsImtpZCI6IkVGODM3QjU4MjM1RkRBMUQzMEUzODgyRkEwODBBNUYwIiwidHlwIjoiSldUIn0.eyJpc3MiOiJodHRwczovL2xvY2FsaG9zdDo1NDQzIiwibmJmIjoxNjQyMjYzNDgzLCJpYXQiOjE2NDIyNjM0ODMsImV4cCI6MTY0MjI2Mzc4MywiYXVkIjoiY2liYSIsImFtciI6WyJwd2QiXSwiYXRfaGFzaCI6IlBXYmttWFNjVHRabTZlNjNXbEM4NkEiLCJzaWQiOiI0NjdFNTNCMzQ0ODJBMjA5Q0JCQkE1MEQyRjQwQzNDNiIsInN1YiI6ImZkOWQ1YWIyLTA0ZTMtNDBkMy1hNTk3LTllMDQ4ZmNjODMwMyIsImF1dGhfdGltZSI6MTY0MjE3OTI1MSwiaWRwIjoibG9jYWwifQ.vrOXBRUbIt7MwjHV8avkFSLX6_TBbrGsuhpVz2yLTQb2b5dqudAk7gay6kRTNhGs5c75YNzMCur0KqGcaTyAxv3eUoJfznEEgbISytEI8a8m0EEO1hsam3fEob5yAmF9_0VMaONDvuJRKd_edUyTYF3c3U6YGJw8u_0ulIQhnTD16qPiMz3uMGYOJfuVti-t4pJjkbXBN9p0gBVXFKoP4QrdzqTyLEGKi2W6RBocUzQDlSZkVao8TV3heoT-U0Cv7cI27S52ncYB61b77lgFKm0f9A97_xi07_to5silTyEKzpuJGD-WSp2IMpKD_00qlJbu-XLaY4o3WTxnvKMNYw","access_token":"eyJhbGciOiJSUzI1NiIsImtpZCI6IkVGODM3QjU4MjM1RkRBMUQzMEUzODgyRkEwODBBNUYwIiwidHlwIjoiYXQrand0In0.eyJpc3MiOiJodHRwczovL2xvY2FsaG9zdDo1NDQzIiwibmJmIjoxNjQyMjYzNDgzLCJpYXQiOjE2NDIyNjM0ODMsImV4cCI6MTY0MjI2NDM4Mywic2NvcGUiOlsib3BlbmlkIiwicHJvZmlsZSIsImFwaTEiLCJvZmZsaW5lX2FjY2VzcyJdLCJhbXIiOlsicHdkIl0sImNsaWVudF9pZCI6ImNpYmEiLCJzdWIiOiJmZDlkNWFiMi0wNGUzLTQwZDMtYTU5Ny05ZTA0OGZjYzgzMDMiLCJhdXRoX3RpbWUiOjE2NDIxNzkyNTEsImlkcCI6ImxvY2FsIiwiZW1haWwiOiJvbGl2aWVyLmxlZmVidnJlQGxpdmUuY29tIiwibmFtZSI6IkFsaWNlIFNtaXRoIiwic2lkIjoiNDY3RTUzQjM0NDgyQTIwOUNCQkJBNTBEMkY0MEMzQzYifQ.Dgqt-7_JIeo8G0v93CqEb7eRrMlkQ8hOh-mu4HACdunak6wRYz2cwwnFW1THhAFFNBq_Te1QvNbqunsA5y8gbQMh7SgM8POx05HsXOD5G6E4AoqNJeicCaDakODDTb8pq5iJumqMOYbI1zdUUbTwjIzF8rSZ-peKby3SPYbz87kllO1eBlbBBCxGGr4K6Dgz1o16EZ72is1y0rHBbiXPjG0IsYFnbMvhIK2soZLzF6pDG0vSGQTz4Q6DSi0gliEEMaXBRLjxVt67dOJH3cuFRvzQg_BTaLcbqiSJGdVGuq4sybUVY-ssVXVY4qjAM47ap9xbc525EFmycK4ZTFte3g","expires_in":900,"token_type":"Bearer","refresh_token":"9628a85c-d754-4403-80d8-c1be01cb636a","scope":"openid profile api1 offline_access"}

Access Token (decoded):
{
  "alg": "RS256",
  "kid": "EF837B58235FDA1D30E3882FA080A5F0",
  "typ": "at\u002Bjwt"
}
{
  "iss": "https://localhost:5443",
  "nbf": 1642263483,
  "iat": 1642263483,
  "exp": 1642264383,
  "scope": [
    "openid",
    "profile",
    "api1",
    "offline_access"
  ],
  "amr": [
    "pwd"
  ],
  "client_id": "ciba",
  "sub": "fd9d5ab2-04e3-40d3-a597-9e048fcc8303",
  "auth_time": 1642179251,
  "idp": "local",
  "email": "olivier.lefebvre@live.com",
  "name": "Alice Smith",
  "sid": "467E53B34482A209CBBBA50D2F40C3C6"
}
```
* Launch the api [API sample](../sample/Aguacongas.TheIdServer.ApiSample)
* Press enter again in the ciba client console.

## Additional resources

* [Client Initiated Backchannel Authentication (CIBA)](https://docs.duendesoftware.com/identityserver/v6/ui/ciba/)
