# TheIdServer Web Server project

## Configure Credentials

### From file

```json
{
  "IdentityServer": {
    "Key": {
      "Type": "File",
      "FilePath": "{path to the .pfx}",
      "Password":  "{.pfx password}"
    }
  }
}
```

### From store

Read [Example: Deploy to Azure Websites](https://docs.microsoft.com/en-us/aspnet/core/security/authentication/identity-api-authorization?view=aspnetcore-3.1#example-deploy-to-azure-websites)