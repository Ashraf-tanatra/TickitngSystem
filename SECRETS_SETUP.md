# Secret configuration

Never commit the JWT signing key, email API key, or production connection string.
The tracked `appsettings.example.json` file contains placeholders only.

## Local development

Generate a cryptographically random JWT key and save it in .NET User Secrets:

```powershell
$key = [Convert]::ToBase64String(
    [Security.Cryptography.RandomNumberGenerator]::GetBytes(48))
dotnet user-secrets set "Jwt:Key" $key --project TickitngSystem
Remove-Variable key
```

User Secrets are stored outside the repository and are loaded only in the
Development environment.

## Production IIS

Generate a different key on the server and configure it as an application pool
environment variable:

```text
Jwt__Key=<base64 value generated from at least 32 random bytes>
```

Do not reuse the development key. Restart the application pool after changing the
value. Changing the production key immediately invalidates every existing access
token, so users must sign in again.

Keep these values outside source control as well:

```text
ConnectionStrings__constr
Resend__ApiKey
```
