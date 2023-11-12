# Password Hasher to rehash password to a new algorithm for ASP.NET Core Identity.

An implementation of `IPasswordHasher<TUser>` and fallback to the configured default `IPasswordHasher<TUser>`.

## Installation

```csharp
services.AddIdentity<TUser, TRole>();
services.AddScoped<IPasswordHasher<TUser>, My.MyPasswordHasher<TUser>>();
services.AddUpgradePasswordHasher<TUser>(options => 
{
    options.HashPrefixMaps = new Dictionary<byte, string>
    {
        [0x00] = "Microsoft.AspNetCore.Identity.PasswordHasher",
        [0x01] = "Microsoft.AspNetCore.Identity.PasswordHasher",
        [0x03] = "My.MyPasswordHasher"
    }
    options.UsePasswordHasherTypeName = "My.MyPasswordHasher";
    options.DeadLine = new DateTime(2024, 1, 1); // after this date each `SuccessRehashNeeded` result will be considered as `Failed`. It forces the user to update its password.
}); // it must be added at last, after adding all needed IPasswordHasher<TUser>.
```

### Options

Default values:

``` json
"ScryptPasswordHasherOptions": {
    "HashPrefixMaps": [
        { 0x00: "Microsoft.AspNetCore.Identity.PasswordHasher" },
        { 0x01: "Microsoft.AspNetCore.Identity.PasswordHasher" },
        { 0xA2: "Aguacongas.TheIdServer.Identity.Argon2PasswordHasher.Argon2PasswordHasher" },
        { 0x0C: "Aguacongas.TheIdServer.Identity.ScryptPasswordHasher.ScryptPasswordHasher" },
        { 0xBC: "Aguacongas.TheIdServer.Identity.BcryptPasswordHasher.BcryptPasswordHasher" }
    ],
    "UsePasswordHasherTypeName": "Aguacongas.TheIdServer.Identity.Argon2PasswordHasher.Argon2PasswordHasher",
    "DeadLine": null
}
```

- **HashPrefixMaps** defines de map between prefix and password hasher implementation.
- **UsePasswordHasherTypeName** the password hasher implementation to use.
- **DeadLine** (optional) after this date hash using old algorithm or old configuration will be considered invalid to prevent password shucking. It forces the user to update its password.

### How it works

The hashed password must be a base 64 string containing a hash prefixed by a byte defining the hash format.  
`UpgradePasswordHasher` gets the hasher defined by this hash prefix and call it to verify the password.  
If the hasher is not the same as the hasher to use, defined by the option `UsePasswordHasherTypeName`, `Success` results are transformed to `SuccessRehashNeeded`.  
If a dead line is defined and the dead line expired, `SuccessRehashNeeded` results are transformed to `Failed`.  
`UpgradePasswordHasher` uses the hasher defined by the option `UsePasswordHasherTypeName` to hash the password.
