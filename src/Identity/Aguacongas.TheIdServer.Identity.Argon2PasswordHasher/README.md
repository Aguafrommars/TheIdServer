# Argon2 Password Hasher for ASP.NET Core Identity

An implementation of IPasswordHasher<TUser> using [Geralt](https://www.geralt.xyz/).

## Installation

```csharp
services.AddIdentity<TUser, TRole>();
services.AddArgon2PasswordHasher<TUser>();
```

### Options

Default values:

``` json
"Argon2PasswordHasherOptions": {
    "Interations": 2,
    "Memory": 67108864,
    "HashPrefix": 0xA0
}
```

- **Interations** can not be less than 1
- **Memory** can not be less than 8192

Read [Geralt Password hashing Notes](https://www.geralt.xyz/password-hashing#notes) for more information to configure the hasher.

`AddArgon2PasswordHasher` can take an action to configure an `Argon2PasswordHasherOptions` instance:

```cs
services.AddArgon2PasswordHasher<TUser>(options => configuration.Bind(options));
```

