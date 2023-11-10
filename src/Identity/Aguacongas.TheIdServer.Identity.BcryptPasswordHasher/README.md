# bcrypt Password Hasher for ASP.NET Core Identity

An implementation of IPasswordHasher<TUser> using [BCrypt.Net-Next](https://github.com/BcryptNet/bcrypt.net).

## Installation

```csharp
services.AddIdentity<TUser, TRole>();
services.AddBcryptPasswordHasher<TUser>();
```

### Options

Default values:

``` json
"ScryptPasswordHasherOptions": {
    "WorkFactor": 11,
    "HashPrefix": 0xBC
}
```

- **WorkFactor** must be greater than 9

Read [Password Storage Cheat Sheet/bcrypt](https://cheatsheetseries.owasp.org/cheatsheets/Password_Storage_Cheat_Sheet.html#bcrypt) for more information to configure the hasher.

`AddBcryptPasswordHasher` can take an action to configure an `BcryptPasswordHasherOptions` instance:

```cs
services.AddBcryptPasswordHasher<TUser>(options => configuration.Bind(options));
```

