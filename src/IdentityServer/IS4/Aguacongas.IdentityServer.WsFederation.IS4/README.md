# Aguacongas.IdentityServer.WsFederation.IS4

Add a WS-Federation controller to your IS4 server.

## Setup

```cs
services.AddIdentityServer()
    .AddKeysRotation(options => configuration.GetSection(nameof(KeyRotationOptions))?.Bind(options));

services.AddControllersWithViews()
    .AddIdentityServerWsFederation();
```

> WS-Fedration depends on a `ISigningCredentialStore`. You can register it using `AddSigningCredential` with a `X509Certificate2` in place of `AddKeysRotation` if you prefer.

## Usage

**`wsfederation/metadata`** returns the WS-Federation metadata document.

You can add a client to you configuration with **wsfed** as protocol type:

```cs
new Client
{
    ClientId = "urn:aspnetcorerp",
    ProtocolType = ProtocolTypes.WsFederation,

    RedirectUris = { "http://localhost:10314/" },
    FrontChannelLogoutUri = "http://localhost:10314/account/signoutcleanup",
    IdentityTokenLifetime = 36000,

    AllowedScopes = { "openid", "profile" }
}
```

And configure the client to use WS-Federation authentication:

```cs
services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = WsFederationDefaults.AuthenticationScheme;
})
    .AddCookie(options =>
    {
        options.Cookie.Name = "aspnetcorewsfed";
    })
    .AddWsFederation(options =>
    {
        options.MetadataAddress = "https://localhost:5443/wsfederation/metadata";
        options.RequireHttpsMetadata = false;

        options.Wtrealm = "urn:aspnetcorerp";

        options.SignOutWreply = "https://localhost:10315";
        options.SkipUnrecognizedRequests = true;
    });
```

## Metadata configuration

`AddIdentityServerWsFederation` extension accept a `IConfiguration` or a `WsFederationOptions` parameter to configure the metadata document génération with claims lists.

``` cs
mvcBuilder.AddIdentityServerWsFederation(configurationManager.GetSection(nameof(WsFederationOptions)));
```

``` json
"WsFederationOptions": {
  "ClaimTypesOffered": [
    {
      "Uri": "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name",
      "DisplayName": "Name",
      "Description": "The unique name of the user"
    },
    {
      "Uri": "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier",
      "DisplayName": "Name ID",
      "Description": "The SAML name identifier of the user"
    },
    {
      "Uri": "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress",
      "DisplayName": "E-Mail Address",
      "Description": "The e-mail address of the user"
    },
    {
      "Uri": "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname",
      "DisplayName": "Given Name",
      "Description": "The given name of the user"
    },
    {
      "Uri": "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname",
      "DisplayName": "Given Name",
      "Description": "The given name of the user"
    },
    {
      "Uri": "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname",
      "DisplayName": "Surname",
      "Description": "The surname of the user"
    },
    {
      "Uri": "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/dateofbirth",
      "DisplayName": "Birth date",
      "Description": "The birth date of the user"
    },
    {
      "Uri": "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/webpage",
      "DisplayName": "Web page",
      "Description": "The wep page of the user"
    }
  ]
}
```

This add the **ClaimTypesOffered** collection to the metadata document:

``` xml
<md:EntityDescriptor xmlns:md="urn:oasis:names:tc:SAML:2.0:metadata" entityID="https://localhost:5443">
	<md:RoleDescriptor xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:fed="http://docs.oasis-open.org/wsfed/federation/200706" xsi:type="fed:SecurityTokenServiceType" protocolSupportEnumeration="http://docs.oasis-open.org/wsfed/federation/200706">
		<md:KeyDescriptor use="signing">
			<KeyInfo xmlns="http://www.w3.org/2000/09/xmldsig#">
				...
			</KeyInfo>
		</md:KeyDescriptor>
		<fed:ClaimTypesOffered>
			<auth:ClaimType xmlns:auth="http://docs.oasis-open.org/wsfed/authorization/200706" Uri="http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name" Optional="true">
				<auth:DisplayName>Name</auth:DisplayName>
				<auth:Description>The unique name of the user</auth:Description>
			</auth:ClaimType>
			<auth:ClaimType xmlns:auth="http://docs.oasis-open.org/wsfed/authorization/200706" Uri="http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier" Optional="true">
				<auth:DisplayName>Name ID</auth:DisplayName>
				<auth:Description>The SAML name identifier of the user</auth:Description>
			</auth:ClaimType>
			<auth:ClaimType xmlns:auth="http://docs.oasis-open.org/wsfed/authorization/200706" Uri="http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress" Optional="true">
				<auth:DisplayName>E-Mail Address</auth:DisplayName>
				<auth:Description>The e-mail address of the user</auth:Description>
			</auth:ClaimType>
			<auth:ClaimType xmlns:auth="http://docs.oasis-open.org/wsfed/authorization/200706" Uri="http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname" Optional="true">
				<auth:DisplayName>Given Name</auth:DisplayName>
				<auth:Description>The given name of the user</auth:Description>
			</auth:ClaimType>
			<auth:ClaimType xmlns:auth="http://docs.oasis-open.org/wsfed/authorization/200706" Uri="http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname" Optional="true">
				<auth:DisplayName>Given Name</auth:DisplayName>
				<auth:Description>The given name of the user</auth:Description>
			</auth:ClaimType>
			<auth:ClaimType xmlns:auth="http://docs.oasis-open.org/wsfed/authorization/200706" Uri="http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname" Optional="true">
				<auth:DisplayName>Surname</auth:DisplayName>
				<auth:Description>The surname of the user</auth:Description>
			</auth:ClaimType>
			<auth:ClaimType xmlns:auth="http://docs.oasis-open.org/wsfed/authorization/200706" Uri="http://schemas.xmlsoap.org/ws/2005/05/identity/claims/dateofbirth" Optional="true">
				<auth:DisplayName>Birth date</auth:DisplayName>
				<auth:Description>The birth date of the user</auth:Description>
			</auth:ClaimType>
			<auth:ClaimType xmlns:auth="http://docs.oasis-open.org/wsfed/authorization/200706" Uri="http://schemas.xmlsoap.org/ws/2005/05/identity/claims/webpage" Optional="true">
				<auth:DisplayName>Web page</auth:DisplayName>
				<auth:Description>The wep page of the user</auth:Description>
			</auth:ClaimType>
		</fed:ClaimTypesOffered>
		<fed:PassiveRequestorEndpoint>
			<wsa:EndpointReference xmlns:wsa="http://www.w3.org/2005/08/addressing">
				<wsa:Address>https://localhost:5443/WsFederation</wsa:Address>
			</wsa:EndpointReference>
		</fed:PassiveRequestorEndpoint>
	</md:RoleDescriptor>
	<Signature xmlns="http://www.w3.org/2000/09/xmldsig#">
		...
	</Signature>
</md:EntityDescriptor>
```

You can also configure the `ClaimTypesRequested` collection.