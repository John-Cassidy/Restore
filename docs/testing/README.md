# Testing Documentation

This folder contains testing guides, test credentials templates, and test result documentation for the Restore e-commerce application.

## Files

### Test Guides

- **E2E_TESTING_GUIDE.md** - Complete end-to-end testing workflow
  - Pre-test setup (start database, API, client, webhooks)
  - Step-by-step test execution
  - Verification in application and Stripe Dashboard
  - Common issues and solutions

### Test Credentials

- **TEST_CREDENTIALS.template.md** - Template for creating your local test credentials file
  - Copy this to `TEST_CREDENTIALS.local.md`
  - Fill in your actual test credentials
  - The `.local.md` file is git-ignored for security

- **TEST_CREDENTIALS.local.md** - Your personal test credentials (git-ignored)
  - Not committed to repository
  - Contains your actual test user accounts, Stripe keys, etc.
  - Safe for storing sensitive test data

## Getting Started with Testing

### 1. Setup Test Credentials

```bash
# Copy the template
cp TEST_CREDENTIALS.template.md TEST_CREDENTIALS.local.md

# Edit with your actual credentials
code TEST_CREDENTIALS.local.md
```

### 2. Configure Stripe Testing

See [Stripe Configuration Guide](../STRIPE.md) for:

- Setting up your Stripe sandbox
- Configuring API keys
- Setting up webhooks
- Using Stripe CLI

### 3. Run E2E Tests

See [Stripe Test Summary](../STRIPE_TEST_SUMMARY.md) for:

- Example test execution
- Expected results
- Known issues and workarounds
- Stripe Link bypass instructions

## Test Data

### Pre-seeded Users

The database initializer creates these test users:

| Username | Email          | Password   | Role          |
| -------- | -------------- | ---------- | ------------- |
| bob      | bob@test.com   | Admin_1234 | Member        |
| admin    | admin@test.com | Admin_1234 | Admin, Member |

### Stripe Test Cards

| Purpose            | Card Number         | Result                  |
| ------------------ | ------------------- | ----------------------- |
| Successful Payment | 4242 4242 4242 4242 | Payment succeeds        |
| Declined Payment   | 4000 0000 0000 0002 | Card declined           |
| Requires Auth      | 4000 0025 0000 3155 | 3D Secure auth required |

See [Stripe Testing Documentation](https://stripe.com/docs/testing) for more test cards.

## Testing Workflow

### Manual Testing

1. Start all services (database, backend, frontend, Stripe webhook listener)
2. Create or use existing test user account
3. Add products to basket
4. Complete checkout process
5. Verify payment in Stripe Dashboard
6. Check order in application

### Automated Testing with Playwright

See the automated test examples in `TEST_CREDENTIALS.local.md` (after setup) for:

- User registration and login
- Basket management
- Checkout flow
- Payment submission
- Order verification

**Note**: Stripe Elements cannot be fully automated due to cross-origin security. Manual card entry is required for payment testing.

## Known Issues

- **Stripe Link Interference**: Stripe Link may auto-fill card details and prevent direct CVV entry
  - **Workaround**: Click on card number field to "Stop using Link and enter card details manually"
- **Webhook Signature Validation**: CLI-generated webhook secret may differ from database value
  - **Impact**: Order status remains "Pending" instead of "PaymentReceived"
  - **Fix**: Synchronize webhook secrets between CLI and database

## Related Documentation

- [Stripe Configuration](../STRIPE.md) - Complete Stripe setup guide
- [Stripe Test Summary](../STRIPE_TEST_SUMMARY.md) - E2E test results
- [User Secrets Management](../UserSecrets.md) - Secure credential storage
- [Client Documentation](../CLIENT.md) - Frontend testing
- [Server Documentation](../SERVER.md) - Backend API testing

## Security Notes

- ✅ `TEST_CREDENTIALS.local.md` is git-ignored
- ✅ Never commit actual credentials
- ✅ Use `.local.md` extension for personal files
- ✅ Keep sensitive data in user secrets or environment variables
- ✅ Use Stripe sandbox for all development testing
