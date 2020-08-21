# TheIdServer

[OpenID/Connect](https://openid.net/connect/) server based on [IdentityServer4](https://identityserver4.readthedocs.io/en/latest/)

[![Build status](https://ci.appveyor.com/api/projects/status/hutfs4sy38fy9ca7?svg=true)](https://ci.appveyor.com/project/aguacongas/theidserver)
 [![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=aguacongas_TheIdServer&metric=alert_status)](https://sonarcloud.io/dashboard?id=aguacongas_TheIdServer) [![][Docker Cloud Build Status]][Docker url]

[Docker Cloud Build Status]: https://img.shields.io/docker/cloud/build/aguacongas/aguacongastheidserver
[Docker url]: https://hub.docker.com/repository/docker/aguacongas/aguacongastheidserver

## Management application

[*src/Aguacongas.TheIdServer.BlazorApp*](https://github.com/Aguafrommars/TheIdServer/tree/master/src/Aguacongas.TheIdServer.BlazorApp) contains the source code of the management application.

### Main features

#### Application
![home](https://raw.githubusercontent.com/Aguafrommars/TheIdServer/master/doc/assets/home.png)

* [Users management](https://github.com/Aguafrommars/TheIdServer/tree/master/doc/USER.md)
* [Roles management](https://github.com/Aguafrommars/TheIdServer/tree/master/doc/ROLE.md)
* [Clients management](https://github.com/Aguafrommars/TheIdServer/tree/master/doc/CLIENT.md)
* [Apis management](https://github.com/Aguafrommars/TheIdServer/tree/master/doc/API.md)
* [Scopes management](https://github.com/Aguafrommars/TheIdServer/tree/master/doc/SCOPE.md)
* [Identities management](https://github.com/Aguafrommars/TheIdServer/tree/master/doc/IDENTITY.md)
* [External providers management](https://github.com/Aguafrommars/TheIdServer/tree/master/doc/PROVIDER.md)
* [Localizable](https://github.com/Aguafrommars/TheIdServer/tree/master/doc/LOCALIZATION.md)
* [Export/import configuration](https://github.com/Aguafrommars/TheIdServer/tree/master/doc/EXPORT_IMPORT.md)

#### Server

* [Dynamic external provider configuration](https://github.com/Aguafrommars/TheIdServer/tree/master/src/Aguacongas.TheIdServer/README.md#configure-the-provider-hub)
* [Public / Private installation](https://github.com/Aguafrommars/TheIdServer/tree/master/src/Aguacongas.TheIdServer/README.md#using-the-api)
* [Docker support](https://github.com/Aguafrommars/TheIdServer/tree/master/src/Aguacongas.TheIdServer/README.md#from-docker)
* [Claims providers](https://github.com/Aguafrommars/TheIdServer/tree/master/doc/CLAIMS_PROVIDER.md)
* [External claims mapping](https://github.com/Aguafrommars/TheIdServer/tree/master/doc/EXTERNAL_CLAIMS_MAPPING.md)
* [Localizable](https://github.com/Aguafrommars/TheIdServer/tree/master/doc/LOCALIZATION.md)
* [OpenID Connect Dynamic Client Registration](https://openid.net/specs/openid-connect-registration-1_0.html)
* [Auto remove expired tokens](https://github.com/Aguafrommars/TheIdServer/tree/master/src/Aguacongas.TheIdServer/README.md#configure-token-cleaner)

### Preview 

An in-memory database version is available on [Heroku](https://www.heroku.com/) at [https://theidserver.herokuapp.com/](https://theidserver.herokuapp.com/).

## Setup

Read the [server README](https://github.com/Aguafrommars/TheIdServer/tree/master/src/Aguacongas.TheIdServer/README.md) for server configuration.  
Read the [application README](https://github.com/Aguafrommars/TheIdServer/tree/master/src/Aguacongas.TheIdServer.BlazorApp/README.md) for application configuration.  

## Build from source

You can build the solution with Visual Studio or use the `dotnet build` command.

## Contribute

We warmly welcome contributions. You can contribute by opening an issue, suggest new a feature, or submit a pull request.

Read [How to contribute](https://github.com/Aguafrommars/TheIdServer/tree/master/CONTRIBUTING.md) and [Contributor Covenant Code of Conduct](https://github.com/Aguafrommars/TheIdServer/tree/master/CODE_OF_CONDUCT.md) for more information.

## OIDC Certification test result

https://www.certification.openid.net/plan-detail.html?plan=ZKco5LJhicIlT