# User Secrets

Store sensitive configuration values outside `appsettings.json` for development and keep them out of source control!

[Official Documentation](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-8.0&tabs=windows)

## What are User Secrets?

User secrets provide a secure way to store sensitive configuration data during development:

- ✅ Stored outside your project directory (not committed to source control)
- ✅ Specific to your local machine
- ✅ Automatically loaded in Development environment
- ✅ Override values in `appsettings.json`

## Setup Instructions

Run these commands inside the API project folder:

```powershell
cd server/Services/Restore/Restore.API
```

### 1. Initialize User Secrets

```powershell
dotnet user-secrets init
```

This creates a unique ID in your `.csproj` file and establishes a secrets storage location.

### 2. Configure Stripe Settings

**Important**: Use **Sandbox** API keys from Stripe Dashboard for isolated testing environments.

#### Get Your Sandbox Keys

1. Go to [Stripe Dashboard](https://dashboard.stripe.com)
2. Click the account picker → **Sandboxes** → Select or create a sandbox
3. Navigate to **Developers** → **API keys**
4. Copy your **Publishable key** and **Secret key**

#### Set the Keys

```powershell
# Set Stripe Publishable Key (for client-side use)
dotnet user-secrets set "StripeSettings:PublishableKey" "pk_test_YOUR_PUBLISHABLE_KEY"

# Set Stripe Secret Key (for server-side use)
dotnet user-secrets set "StripeSettings:SecretKey" "sk_test_YOUR_SECRET_KEY"

# Set Webhook Secret (get from Stripe CLI or Dashboard)
dotnet user-secrets set "StripeSettings:WhSecret" "whsec_YOUR_WEBHOOK_SECRET"
```

### 3. Get Webhook Secret

For local development, use the Stripe CLI:

```powershell
stripe listen -f http://localhost:5000/api/payments/webhook -e charge.succeeded
```

Copy the `whsec_xxx` value from the output and update your secrets:

```powershell
dotnet user-secrets set "StripeSettings:WhSecret" "whsec_YOUR_WEBHOOK_SECRET"
```

### 4. Verify Configuration

```powershell
dotnet user-secrets list
```

Expected output:

```
StripeSettings:PublishableKey = pk_test_51...
StripeSettings:SecretKey = sk_test_51...
StripeSettings:WhSecret = whsec_...
```

## About Stripe Sandboxes

Stripe Sandboxes provide **isolated test environments** for development and testing:

- **Isolated** test environments for different teams/projects
- **Role-based access control** (requires Sandbox User, Developer, or Admin role)
- **Independent data** - changes don't affect other sandboxes
- **Same API key format**: `pk_test_...` and `sk_test_...`
- **Multiple sandboxes** - create separate environments for different purposes

**Best Practice**: Use dedicated sandboxes for each project or team to maintain clean, isolated test data. See [STRIPE.md](STRIPE.md) for setup instructions.

## Current Configuration (Example)

**Note**: These are example values. Replace with your actual sandbox keys.

```
StripeSettings:PublishableKey = pk_test_51XXXXXXXXXXXXX...
StripeSettings:SecretKey = sk_test_51XXXXXXXXXXXXX...
StripeSettings:WhSecret = whsec_XXXXXXXXXXXXXXXXXX...
```

## Other Secrets

As your application grows, you may need to add other secrets:

### JWT Configuration (Production)

```powershell
dotnet user-secrets set "Jwt:SecretKey" "YOUR_SECURE_SECRET_KEY"
```

### Database Connection Strings (Production)

```powershell
dotnet user-secrets set "ConnectionStrings:Production" "YOUR_PRODUCTION_CONNECTION_STRING"
```

### Email Service (SendGrid, etc.)

```powershell
dotnet user-secrets set "SendGrid:ApiKey" "YOUR_SENDGRID_API_KEY"
```

## Security Best Practices

### ✅ DO

- Use user-secrets for all sensitive development data
- Rotate API keys regularly (Stripe CLI login expires after 90 days)
- Use different keys for development and production
- Document which secrets are required in this file

### ❌ DON'T

- Commit secrets to source control (check `.gitignore`)
- Share secrets via chat/email (use secure channels)
- Use development keys in production
- Hardcode secrets in source files

## Production Deployment

User secrets are **only for local development**. For production:

### Azure App Service

- Use **Application Settings** in the Azure Portal
- Or use **Azure Key Vault** for enhanced security

### Docker/Kubernetes

- Use **environment variables**
- Or use **secrets management** (Docker secrets, Kubernetes secrets)

### Other Platforms

- Use platform-specific secret management
- Set environment variables via deployment configuration

## Troubleshooting

### Secrets Not Loading

1. Verify you're in **Development** environment (`ASPNETCORE_ENVIRONMENT=Development`)
2. Check secrets are initialized: `dotnet user-secrets list`
3. Restart your application

### Wrong Values Being Used

User secrets override `appsettings.json` but are overridden by:

1. Environment variables
2. Command-line arguments

### Can't Find Secrets

Secrets are stored in:

- **Windows**: `%APPDATA%\Microsoft\UserSecrets\<user_secrets_id>\secrets.json`
- **macOS/Linux**: `~/.microsoft/usersecrets/<user_secrets_id>/secrets.json`

## Related Documentation

- [STRIPE.md](STRIPE.md) - Complete Stripe configuration guide
- [Microsoft Docs: Safe storage of app secrets](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets)
- [Stripe Sandboxes Documentation](https://docs.stripe.com/sandboxes)
