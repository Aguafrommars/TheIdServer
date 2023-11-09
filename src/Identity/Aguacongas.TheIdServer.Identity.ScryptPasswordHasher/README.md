# scrypt Password Hasher for ASP.NET Core Identity

An implementation of IPasswordHasher<TUser> using [Scrypt.Net](https://github.com/viniciuschiele/scrypt/).

## Installation

```csharp
services.AddIdentity<TUser, TRole>();
services.AddScryptPasswordHasher<TUser>();
```

### Options

Default values:

``` json
"ScryptPasswordHasherOptions": {
    "IterationCount": 131072,
    "BlockSize": 8,
    "ThreadCount": 1
    "HashPrefix": 0x0C
}
```

- **IterationCount** must be a power of two greater than 1
- **BlockSize** must be greater than 0
- **ThreadCount** must be greater than 0

Read [Password Storage Cheat Sheet/scrypt](https://cheatsheetseries.owasp.org/cheatsheets/Password_Storage_Cheat_Sheet.html#scrypt) for more information to configure the hasher.

`AddScryptPasswordHasher` can take an action to configure an `ScryptPasswordHasherOptions` instance:

```cs
services.AddScryptPasswordHasher<TUser>(options => configuration.Bind(options));
```

