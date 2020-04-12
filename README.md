# TheIdServer

[OpenID/Connect](https://openid.net/connect/) server base on [IdentityServer4](https://identityserver4.readthedocs.io/en/latest/)

[![Build status](https://ci.appveyor.com/api/projects/status/hutfs4sy38fy9ca7?svg=true)](https://ci.appveyor.com/project/aguacongas/theidserver)
 [![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=aguacongas_TheIdServer&metric=alert_status)](https://sonarcloud.io/dashboard?id=aguacongas_TheIdServer) [![][Docker Cloud Build Status]][Docker url]

[Docker Cloud Build Status]: https://img.shields.io/docker/cloud/build/aguacongas/aguacongastheidserver
[Docker url]: https://hub.docker.com/repository/docker/aguacongas/aguacongastheidserver

## Management application

[*src/Aguacongas.TheIdServer.BlazorApp*](src/Aguacongas.TheIdServer.BlazorApp) contains the management applicationthe code.

### Main features

![home](/doc/assets/home.png)

* [Users management](/doc/USER.md)
* [Roles management](/doc/ROLE.md)
* [Clients management](/doc/CLIENT.md)
* [Apis management](/doc/API.md)
* [Identities management](/doc/IDENTITY.md)
* [External providers management](/doc/PROVIDER.md)

### Preview 

An in memory db version is deployed on [heroku](https://www.heroku.com/) at [https://theidserver.herokuapp.com/](https://theidserver.herokuapp.com/)

## Setup

Read the [server README](src/Aguacongas.TheIdServer/README.md) for server configuration.  
Read the [application README](src/Aguacongas.TheIdServer.BlazorApp/README.md) for application configuration.  

## Build from source

You can build the solution with Visual Studio or use the `dotnet build` command.