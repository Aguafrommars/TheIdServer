using Aguacongas.TheIdServer.CibaSample;
using IdentityModel;
using IdentityModel.Client;

IDiscoveryCache _cache = new DiscoveryCache(Constants.Authority);

Console.Title = "Console CIBA Client";

var loginResponse = await RequestBackchannelLoginAsync();

var tokenResponse = await RequestTokenAsync(loginResponse);

tokenResponse.Show();

Console.ReadLine();
await CallServiceAsync(tokenResponse.AccessToken);

async Task<TokenResponse> RequestTokenAsync(BackchannelAuthenticationResponse authorizeResponse)
{
    var disco = await _cache.GetAsync();
    if (disco.IsError)
    {
        throw new Exception(disco.Error);
    }

    var client = new HttpClient();

    while (true)
    {
        var response = await client.RequestBackchannelAuthenticationTokenAsync(new BackchannelAuthenticationTokenRequest
        {
            Address = disco.TokenEndpoint,
            ClientId = "ciba",
            ClientSecret = "secret",
            AuthenticationRequestId = authorizeResponse.AuthenticationRequestId
        });

        if (response.IsError)
        {
            if (response.Error == OidcConstants.TokenErrors.AuthorizationPending || response.Error == OidcConstants.TokenErrors.SlowDown)
            {
                Console.WriteLine($"{response.Error}...waiting.");
                Thread.Sleep(authorizeResponse.Interval ?? 1 * 1000);
            }
            else
            {
                throw new Exception(response.Error);
            }
        }
        else
        {
            return response;
        }
    }
}

async Task<BackchannelAuthenticationResponse> RequestBackchannelLoginAsync()
{
    var disco = await _cache.GetAsync();
    if (disco.IsError)
    {
        throw new Exception(disco.Error);
    }

    var cibaEp = disco.BackchannelAuthenticationEndpoint;

    var username = "alice";
    var bindingMessage = Guid.NewGuid().ToString("N").Substring(0, 10);

    var req = new BackchannelAuthenticationRequest()
    {
        Address = cibaEp,
        ClientId = "ciba",
        ClientSecret = "secret",
        Scope = "openid profile api1 offline_access",
        LoginHint = username,
        //IdTokenHint = "eyJhbGciOiJSUzI1NiIsImtpZCI6IkYyNjZCQzA3NTFBNjIyNDkzMzFDMzI4QUQ1RkIwMkJGIiwidHlwIjoiSldUIn0.eyJpc3MiOiJodHRwczovL2xvY2FsaG9zdDo1MDAxIiwibmJmIjoxNjM4NDc3MDE2LCJpYXQiOjE2Mzg0NzcwMTYsImV4cCI6MTYzODQ3NzMxNiwiYXVkIjoiY2liYSIsImFtciI6WyJwd2QiXSwiYXRfaGFzaCI6ImE1angwelVQZ2twczBVS1J5VjBUWmciLCJzaWQiOiIzQTJDQTJDNjdBNTAwQ0I2REY1QzEyRUZDMzlCQTI2MiIsInN1YiI6IjgxODcyNyIsImF1dGhfdGltZSI6MTYzODQ3NzAwOCwiaWRwIjoibG9jYWwifQ.GAIHXYgEtXw5NasR0zPMW3jSKBuWujzwwnXJnfHdulKX-I3r47N0iqHm5v5V0xfLYdrmntjLgmdm0DSvdXswtZ1dh96DqS1zVm6yQ2V0zsA2u8uOt1RG8qtjd5z4Gb_wTvks4rbUiwi008FOZfRuqbMJJDSscy_YdEJqyQahdzkcUnWZwdbY8L2RUTxlAAWQxktpIbaFnxfr8PFQpyTcyQyw0b7xmYd9ogR7JyOff7IJIHPDur0wbRdpI1FDE_VVCgoze8GVAbVxXPtj4CtWHAv07MJxa9SdA_N-lBcrZ3PHTKQ5t1gFXwdQvp3togUJl33mJSru3lqfK36pn8y8ow",
        BindingMessage = bindingMessage,
        RequestedExpiry = 200
    };

    var client = new HttpClient();
    var response = await client.RequestBackchannelAuthenticationAsync(req);

    if (response.IsError)
    {
        throw new Exception(response.Error);
    }

    Console.WriteLine($"Login Hint                  : {username}");
    Console.WriteLine($"Binding Message             : {bindingMessage}");
    Console.WriteLine($"Authentication Request Id   : {response.AuthenticationRequestId}");
    Console.WriteLine($"Expires In                  : {response.ExpiresIn}");
    Console.WriteLine($"Interval                    : {response.Interval}");
    Console.WriteLine();

    Console.WriteLine($"\nPress enter to start polling the token endpoint.");
    Console.ReadLine();

    return response;
}

async Task CallServiceAsync(string token)
{
    var baseAddress = Constants.SampleApi;

    var client = new HttpClient
    {
        BaseAddress = new Uri(baseAddress)
    };

    client.SetBearerToken(token);
    var response = await client.GetStringAsync("identity");

    "\n\nService claims:".ConsoleGreen();
    Console.WriteLine(response.PrettyPrintJson());
}