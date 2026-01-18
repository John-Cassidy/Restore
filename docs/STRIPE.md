# Stripe Development Notes

## Account Information

- **Account ID**: `acct_1OfiQ1BurVzaOgd8`
- **Display Name**: Restore
- **Dashboard**: [https://dashboard.stripe.com](https://dashboard.stripe.com)

## Stripe CLI

**Version**: 1.34.0  
**Location**: `C:\tools\stripe-cli\stripe.exe` (added to PATH)

### CLI Commands

```powershell
# Check version
stripe --version
# Output: stripe version 1.34.0

# Login using browser
stripe login
# Press Enter to open the browser or visit the provided URL
# Output: Done! The Stripe CLI is configured for Restore with account id xxxxx
# Note: This key will expire after 90 days, requiring re-authentication

# Get Webhook signing secret (basic):
stripe listen
# Output: Ready! Your webhook signing secret is whsec_xxx (^C to quit)

# Forward webhooks to local API:
stripe listen -f http://localhost:5000/api/payments/webhook -e charge.succeeded
```

## Stripe Sandboxes

**Sandboxes** are Stripe's isolated test environments for development and testing:

- **Isolated environments** - each sandbox is independent with its own data
- **Role-based access** - only users with Sandbox User, Developer, Admin, or Sandbox Admin roles can access
- **Multiple sandboxes** - create separate sandboxes for different teams or projects
- **API key format** - uses `pk_test_` and `sk_test_` prefixes
- **Independent configuration** - each sandbox has its own settings and test data

### Creating a Sandbox

1. Navigate to [Stripe Dashboard](https://dashboard.stripe.com)
2. Click the account picker in the top-left
3. Select **"Create sandbox"** or choose **"Sandboxes"**
4. Give your sandbox a name (e.g., "Restore Development")
5. The sandbox is created with a copy of your live mode configuration

### Getting Sandbox API Keys

1. Open your sandbox from the account picker
2. Navigate to **Developers** → **API keys**
3. You'll see:
   - **Publishable key** (`pk_test_...`) - For client-side use
   - **Secret key** (`sk_test_...`) - For server-side use
4. Copy these keys to your configuration (see Configuration section below)

## Configuration

### Backend (.NET API)

**Location**: `server/Services/Restore/Restore.API`

#### User Secrets Setup

User secrets keep sensitive API keys out of source control. Run these commands from the API project directory:

```powershell
cd server/Services/Restore/Restore.API

# Initialize user secrets (only needed once)
dotnet user-secrets init

# Set Stripe sandbox keys
dotnet user-secrets set "StripeSettings:PublishableKey" "pk_test_YOUR_PUBLISHABLE_KEY"
dotnet user-secrets set "StripeSettings:SecretKey" "sk_test_YOUR_SECRET_KEY"
dotnet user-secrets set "StripeSettings:WhSecret" "whsec_YOUR_WEBHOOK_SECRET"

# List all configured secrets
dotnet user-secrets list
```

**Example Output** (your actual keys will be different):

```
StripeSettings:PublishableKey = pk_test_51XXXXXXXXXXXXX...
StripeSettings:SecretKey = sk_test_51XXXXXXXXXXXXX...
StripeSettings:WhSecret = whsec_XXXXXXXXXXXXXXXXXX...
```

#### appsettings.json Template

Add this section to `appsettings.json` (values will be overridden by user-secrets in development):

```json
{
  "StripeSettings": {
    "PublishableKey": "",
    "SecretKey": "",
    "WhSecret": ""
  }
}
```

### Frontend (React Client)

**Location**: `client/`

#### Environment Variables

The client uses Vite environment variables to configure the Stripe publishable key.

**File**: `client/.env.development`

```env
VITE_API_URL = http://localhost:5000/api/
VITE_STRIPE_PUBLISHABLE_KEY = pk_test_YOUR_PUBLISHABLE_KEY
```

**File**: `client/.env.production`

```env
VITE_API_URL = /api/
VITE_STRIPE_PUBLISHABLE_KEY = pk_test_YOUR_PUBLISHABLE_KEY
```

**Important**:

- Never commit actual API keys to source control
- Add `.env.local` to `.gitignore` for local overrides
- Use different keys for production (live mode keys: `pk_live_`, `sk_live_`)

## Webhook Configuration

### Local Development with Stripe CLI

For local testing, use the Stripe CLI to forward webhook events to your local API:

```powershell
# Start webhook forwarding
.\.developernotes\stripe_1.19.2_windows_x86_64\stripe.exe listen -f http://localhost:5000/api/payments/webhook -e charge.succeeded

# Copy the webhook signing secret (whsec_xxx) from the output
# Update your user-secrets with this value:
dotnet user-secrets set "StripeSettings:WhSecret" "whsec_YOUR_WEBHOOK_SECRET"
```

### Supported Webhook Events

The API handles these Stripe webhook events:

- `charge.succeeded` - Payment successful, updates order status to `PaymentReceived`

**Endpoint**: `/api/payments/webhook` (allows anonymous access)

### Production Webhook Configuration

For production deployment:

1. Go to [Stripe Dashboard](https://dashboard.stripe.com) → **Developers** → **Webhooks**
2. Click **"Add endpoint"**
3. Enter your production URL: `https://your-domain.com/api/payments/webhook`
4. Select events to listen for: `charge.succeeded`
5. Copy the webhook signing secret
6. Add to your production configuration (environment variables, Azure Key Vault, etc.)

## Testing Payments

### Test Card Numbers

Use these test cards in your sandbox (source: [Stripe Testing Documentation](https://docs.stripe.com/testing#cards)):

| Card Number         | Description                        |
| ------------------- | ---------------------------------- |
| 4242 4242 4242 4242 | Successful payment                 |
| 4000 0000 0000 9995 | Card declined (insufficient funds) |
| 4000 0000 0000 0002 | Card declined (generic decline)    |
| 4000 0025 0000 3155 | Requires 3D Secure authentication  |

**Any future expiration date** (e.g., 12/34)  
**Any 3-digit CVC** (e.g., 123)  
**Any postal code** (e.g., 12345)

### Testing Workflow

1. **Start the backend**:

   ```powershell
   cd server/Services/Restore/Restore.API
   dotnet run
   ```

2. **Start the frontend**:

   ```powershell
   cd client
   npm start
   ```

3. **Start Stripe webhook listener**:

   ```powershell
   .\.developernotes\stripe_1.19.2_windows_x86_64\stripe.exe listen -f http://localhost:5000/api/payments/webhook -e charge.succeeded
   ```

4. **Test the flow**:
   - Add products to basket
   - Navigate to checkout
   - Enter test card: `4242 4242 4242 4242`
   - Complete payment
   - Verify webhook receives `charge.succeeded`
   - Check order status updates to `PaymentReceived`

## Implementation Details

### Backend Components

- **[PaymentService.cs](server/Services/Restore/Restore.Infrastructure/Services/PaymentService.cs)** - Creates and updates Stripe PaymentIntents
- **[PaymentsModule.cs](server/Services/Restore/Restore.API/Endpoints/PaymentsModule.cs)** - API endpoints for payment operations
  - `POST /api/payments` - Create or update PaymentIntent
  - `POST /api/payments/webhook` - Webhook handler
- **[VerifyPaymentHandler.cs](server/Services/Restore/Restore.Application/Handlers/VerifyPaymentHandler.cs)** - Processes webhook events and updates order status
- **NuGet Package**: Stripe.net v43.13.0

### Frontend Components

- **[CheckoutWrapper.tsx](client/src/features/checkout/CheckoutWrapper.tsx)** - Initializes Stripe Elements with publishable key
- **[CheckoutPage.tsx](client/src/features/checkout/CheckoutPage.tsx)** - Handles payment confirmation
- **[PaymentForm.tsx](client/src/features/checkout/PaymentForm.tsx)** - Card input form
- **NPM Packages**:
  - @stripe/react-stripe-js v2.4.0
  - @stripe/stripe-js v2.4.0

## MCP Server Integration

The Restore project uses the official Stripe MCP (Model Context Protocol) server for enhanced Stripe integration capabilities through GitHub Copilot.

**Available via**: `com.stripe.mcp`

This provides AI-powered assistance for:

- Searching Stripe documentation
- Finding code samples
- Managing Stripe resources
- Integration planning and recommendations

## Resources

- [Stripe Dashboard](https://dashboard.stripe.com/acct_1OfiQ1BurVzaOgd8/apikeys)
- [Stripe Testing Documentation](https://docs.stripe.com/testing)
- [Stripe Sandboxes Documentation](https://docs.stripe.com/sandboxes)
- [Stripe API Documentation](https://docs.stripe.com/api)
- [Stripe .NET Library](https://github.com/stripe/stripe-dotnet)
- [Stripe React Documentation](https://docs.stripe.com/stripe-js/react)
