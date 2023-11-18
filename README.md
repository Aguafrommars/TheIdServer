# TheIdServer

[OpenID/Connect](https://openid.net/connect/), [OAuth2](https://oauth.net/2/), [WS-Federation](https://docs.oasis-open.org/wsfed/federation/v1.2/os/ws-federation-1.2-spec-os.html) and [SAML 2.0](http://docs.oasis-open.org/security/saml/v2.0/sstc-saml-approved-errata-2.0.html) server based on [Duende IdentityServer](https://duendesoftware.com/products/identityserver) and [ITfoxtec Identity SAML 2.0](https://www.itfoxtec.com/IdentitySaml2).

> [OpenID/Connect](https://openid.net/connect/), [OAuth2](https://oauth.net/2/), [WS-Federation](https://docs.oasis-open.org/wsfed/federation/v1.2/os/ws-federation-1.2-spec-os.html) and [SAML 2.0](http://docs.oasis-open.org/security/saml/v2.0/sstc-saml-approved-errata-2.0.html) are protocols that enable secure authentication and authorization of users and applications on the web. They allow users to sign in with their existing credentials from an identity provider (such as Google, Facebook, Microsoft, Twitter ans so-on) and grant access to their data and resources on different platforms and services. These protocols also enable developers to create applications that can interact with various APIs and resources without exposing the user’s credentials or compromising their privacy. Some examples of applications that use these protocols are web browsers, mobile apps, web APIs, and single-page applications.

> [Duende IdentityServer](https://duendesoftware.com/products/identityserver) is a framework that implements OpenID Connect and OAuth 2.0 protocols for ASP.NET Core applications. It allows you to create your own identity and access management solution that can integrate with various identity providers and APIs.

> [ITfoxtec Identity SAML 2.0](https://www.itfoxtec.com/IdentitySaml2) is a framework that implements SAML-P for both Identity Provider (IdP) and Relying Party (RP).

> TheIdServer implements all Duende IdentityServer features, a SAML 2.0 Identity Provider and comes with an admin UI.

[![Quality gate](https://sonarcloud.io/api/project_badges/quality_gate?project=aguacongas_TheIdServer)](https://sonarcloud.io/dashboard?id=aguacongas_TheIdServer)

[![Build status](https://ci.appveyor.com/api/projects/status/hutfs4sy38fy9ca7?svg=true)](https://ci.appveyor.com/project/aguacongas/theidserver) [![Docker](https://github.com/Aguafrommars/TheIdServer/actions/workflows/docker.yml/badge.svg)](https://github.com/Aguafrommars/TheIdServer/actions/workflows/docker.yml) [![Artifact HUB](https://img.shields.io/endpoint?url=https://artifacthub.io/badge/repository/aguafrommars)](https://artifacthub.io/packages/search?repo=aguafrommars)

### Documentation

Thanks [@ldeluigi](https://github.com/ldeluigi) and its [markdown-docs GitHub action](https://github.com/ldeluigi/markdown-docs). All markdown files are deployed in html [here](https://aguafrommars.github.io/TheIdServer/).

### Try it now at [https://theidserver-duende.herokuapp.com/](https://theidserver-duende.herokuapp.com/)

**login**: alice  
**pwd**: Pass123$

An in-memory database version is available on [Heroku](https://www.heroku.com/).

### Give a Star! :star:

If you like or are using this project to learn or start your solution, please give it a star. Thanks!

Or if you're feeling really generous, we support sponsorships.

Choose your favorite:

* [issuehunts](https://issuehunt.io/r/Aguafrommars/TheIdServer/issues/170)
* [github sponsor](https://github.com/sponsors/aguacongas),
* [liberapay](https://liberapay.com/aguacongas)

## Main features

### Admin app
![home](https://raw.githubusercontent.com/Aguafrommars/TheIdServer/master/doc/assets/home.png)

* [Users management](https://github.com/Aguafrommars/TheIdServer/tree/master/doc/USER.md)
* [Roles management](https://github.com/Aguafrommars/TheIdServer/tree/master/doc/ROLE.md)
* [Clients management](https://github.com/Aguafrommars/TheIdServer/tree/master/doc/CLIENT.md)
* [Apis management](https://github.com/Aguafrommars/TheIdServer/tree/master/doc/API.md)
* [Api Scopes management](https://github.com/Aguafrommars/TheIdServer/tree/master/doc/SCOPE.md)
* [Identities management](https://github.com/Aguafrommars/TheIdServer/tree/master/doc/IDENTITY.md)
* [Relying parties management](https://github.com/Aguafrommars/TheIdServer/tree/master/doc/RELYING-PARTY.md)
* [External providers management](https://github.com/Aguafrommars/TheIdServer/tree/master/doc/PROVIDER.md)
* [Localizable](https://github.com/Aguafrommars/TheIdServer/tree/master/doc/LOCALIZATION.md)
* [Export/import configuration](https://github.com/Aguafrommars/TheIdServer/tree/master/doc/EXPORT_IMPORT.md)
* [Keys management](https://github.com/Aguafrommars/TheIdServer/tree/master/doc/KEYS.md)
* [Server settings](https://github.com/Aguafrommars/TheIdServer/tree/master/doc/SETTINGS.md)

### Server

* [OpenID/Connect](https://openid.net/connect/), [OAuth2](https://oauth.net/2/), [WS-Federation](https://docs.oasis-open.org/wsfed/federation/v1.2/os/ws-federation-1.2-spec-os.html) and [Saml2P](http://docs.oasis-open.org/security/saml/v2.0/sstc-saml-approved-errata-2.0.html) server
* [Large choice of database](https://github.com/Aguafrommars/TheIdServer/blob/master/doc/SERVER.md#using-entity-framework-core)
* [Dynamic external provider configuration](https://github.com/Aguafrommars/TheIdServer/tree/master/doc/SERVER.md#configure-the-provider-hub)
* [Public / Private installation](https://github.com/Aguafrommars/TheIdServer/tree/master/doc/SERVER.md#using-the-api)
* [Docker support](https://github.com/Aguafrommars/TheIdServer/tree/master/doc/SERVER.md#from-docker)
* [Claims providers](https://github.com/Aguafrommars/TheIdServer/tree/master/doc/CLAIMS_PROVIDER.md)
* [External claims mapping](https://github.com/Aguafrommars/TheIdServer/tree/master/doc/EXTERNAL_CLAIMS_MAPPING.md)
* [Localizable](https://github.com/Aguafrommars/TheIdServer/tree/master/doc/LOCALIZATION.md)
* [OpenID Connect Dynamic Client Registration](https://openid.net/specs/openid-connect-registration-1_0.html)
* [Auto remove expired tokens](https://github.com/Aguafrommars/TheIdServer/tree/master/doc/SERVER.md#configure-token-cleaner)
* [Keys rotation](https://github.com/Aguafrommars/TheIdServer/tree/master/doc/KEYS_ROTATION.md)
* [Create Personal Access Token](https://github.com/Aguafrommars/TheIdServer/tree/master/doc/PAT.md)
* [Duende CIBA integration](https://github.com/Aguafrommars/TheIdServer/tree/master/doc/CIBA.md)
* [Token exchange](https://datatracker.ietf.org/doc/html/rfc8693)([RFC 8693](https://datatracker.ietf.org/doc/html/rfc8693))
* [Health checks](https://github.com/Aguafrommars/TheIdServer/tree/master/doc/SERVER.md#health-checks)
* [OpenTelemety](https://github.com/Aguafrommars/TheIdServer/tree/master/doc/OPEN_TELEMETRY.md)
* [Server side session](https://github.com/Aguafrommars/TheIdServer/tree/master/doc/SERVER_SIDE_SESSIONS.md)
* [Passwor hashing configuration](https://github.com/Aguafrommars/TheIdServer/tree/master/doc/SERVER.md#configure-password-hashers-options)
  
  
## Setup

* Read the [TheIdServer Duende Web Server](doc/SERVER.md) to configure the Duende IdentityServer.  
**You'll need to [acquire a license](https://duendesoftware.com/products/identityserver#pricing) for a commercial use of this version.**
* Read the [TheIdServer Admin Application](doc/ADMINAPP.md) for application configuration.  

## Build from source

You can build the solution with Visual Studio or use the `dotnet build` command.  
To build docker images launch at solution root: 

```bash
docker build -t aguacongas/theidserver.duende:dev -f "./src/Aguacongas.TheIdServer.Duende/Dockerfile" .
docker build -t aguacongas/theidserverapp:dev -f "./src/Aguacongas.TheIdServer.BlazorApp/Dockerfile" .
```

## Contribute

We warmly welcome contributions. You can contribute by opening an issue, suggest new a feature, or submit a pull request.

Read [How to contribute](https://github.com/Aguafrommars/TheIdServer/tree/master/CONTRIBUTING.md) and [Contributor Covenant Code of Conduct](https://github.com/Aguafrommars/TheIdServer/tree/master/CODE_OF_CONDUCT.md) for more information.

## OIDC Certification test result

The server pass the [oidcc-basic-certification-test-plan](
https://www.certification.openid.net/plan-detail.html?plan=ZKco5LJhicIlT&public=true) with some warnings. It is anticipated that it will pass the certification process, but we need your assistance. Please sponsor this project to help us pay the required [certification fee](https://openid.net/certification/fees/).

Choose your favorite:

* [github sponsor](https://github.com/sponsors/aguacongas/sponsorships?sponsor=aguacongas&tier_id=151490)
* [issuehunts](https://issuehunt.io/r/Aguafrommars/TheIdServer/issues/170)
* [liberapay](https://liberapay.com/aguacongas)

## IdentityServer4 end of support

The old IS4 version has been remove from the solution as [IS4 reach is end of support](https://github.com/IdentityServer/IdentityServer4#important-update).
