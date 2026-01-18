# Test User Credentials Template

> **⚠️ TEMPLATE FILE**  
> Copy this file to `TEST_CREDENTIALS.local.md` and fill in your actual test credentials.  
> The `.local.md` file is excluded from git and safe for storing sensitive data.

## Test User Account

Create a test user account with these properties:

- **Username**: `[your-test-username]`
- **Email**: `[your-test-email]@test.com`
- **Password**: `[your-secure-password]`
- **Role**: Member (default)

## Stripe Test Cards

### Successful Payment

- **Card Number**: `4242 4242 4242 4242`
- **Expiry**: Any future date (e.g., `12/28`)
- **CVC**: Any 3 digits (e.g., `123`)
- **ZIP**: Any 5 digits (e.g., `12345`)

### Declined Payment

- **Card Number**: `4000 0000 0000 0002`

### Requires Authentication

- **Card Number**: `4000 0025 0000 3155`

## Test Shipping Address

Use any test shipping address for checkout:

- **Full Name**: [Your Name]
- **Address Line 1**: [Street Address]
- **City**: [City]
- **State**: [State]
- **Zip Code**: [Zip]
- **Country**: USA

## Pre-Seeded Users (from DbInitializer)

These users are automatically created when the database is initialized:

### Regular User

- **Username**: `bob`
- **Email**: `bob@test.com`
- **Password**: `Admin_1234`
- **Role**: Member

### Admin User

- **Username**: `admin`
- **Email**: `admin@test.com`
- **Password**: `Admin_1234`
- **Roles**: Admin, Member

## Environment Configuration

- **Frontend**: http://localhost:3000
- **Backend API**: http://localhost:5000
- **Database**: PostgreSQL on localhost:5432
- **Stripe Sandbox**: [Your Sandbox Name] ([Account ID])
  - **Dashboard URL**: https://dashboard.stripe.com/[your-account-id]/test/dashboard
  - **Login**: Use your Stripe account username and password at https://dashboard.stripe.com/login
  - **Note**: After login, you may need to manually navigate to your sandbox using the account switcher

## Automated Test Results

Document your test execution results here:

### Test Execution: [Date]

**Automated Steps Completed:**

1. ⬜ User Registration
2. ⬜ User Login
3. ⬜ Navigate to Basket
4. ⬜ Checkout Initiated
5. ⬜ Shipping Address
6. ⬜ Order Review
7. ⬜ Payment Page
8. ⬜ Manual Card Entry
9. ⬜ Payment Submitted
10. ⬜ Order Created
11. ⬜ Basket Cleared
12. ⬜ Order Confirmation
13. ⬜ View Order

**Test Results:**

Document your payment testing results, including:

- Payment Intent IDs
- Order numbers
- Webhook status
- Any errors encountered
- Screenshots location

## Setup Instructions

1. Copy this template: `cp TEST_CREDENTIALS.template.md TEST_CREDENTIALS.local.md`
2. Fill in your actual test credentials in the `.local.md` file
3. Configure your Stripe sandbox (see [STRIPE.md](../STRIPE.md))
4. Run automated tests to verify setup
5. Document test results in your local file

## Notes

- The `TEST_CREDENTIALS.local.md` file is git-ignored and safe for storing credentials
- Never commit actual credentials to the repository
- Update this template if you add new test scenarios or credentials
- See [STRIPE_TEST_SUMMARY.md](../STRIPE_TEST_SUMMARY.md) for example test results
