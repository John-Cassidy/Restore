# End-to-End Payment Testing Guide

Complete guide for manually testing the Restore e-commerce payment flow from setup to verification.

## Prerequisites

- PostgreSQL installed (or Docker for database)
- .NET 8 SDK installed
- Node.js 18+ installed
- Stripe CLI installed (see [STRIPE.md](../STRIPE.md))
- Stripe account configured (see [STRIPE.md](../STRIPE.md))

## Pre-Test Setup

### 0. Configure Stripe API Keys (FIRST TIME ONLY)

**Before your first test run**, you need to configure your Stripe API keys:

#### A. Get Your Stripe Keys

1. Login to Stripe Dashboard: https://dashboard.stripe.com/login
2. Navigate to your sandbox API keys: https://dashboard.stripe.com/acct_1SqyU8BSFHw8VMDK/test/apikeys
3. Copy the following keys:
   - **Publishable key** (starts with `pk_test_...`)
   - **Secret key** (click "Reveal test key" then copy, starts with `sk_test_...`)

#### B. Update Frontend Environment Files

Edit both client environment files with your **Publishable key**:

**File: `client/.env.development`**

```bash
VITE_API_URL = http://localhost:5000/api/
VITE_STRIPE_PUBLISHABLE_KEY = pk_test_YOUR_PUBLISHABLE_KEY_HERE
```

**File: `client/.env.production`**

```bash
VITE_API_URL = /api/
VITE_STRIPE_PUBLISHABLE_KEY = pk_test_YOUR_PUBLISHABLE_KEY_HERE
```

#### C. Update Backend User Secrets

Configure the **Secret key** and **Webhook secret** (see [UserSecrets.md](../UserSecrets.md)):

```powershell
cd server/Services/Restore/Restore.API

# Set Stripe publishable key (optional - frontend uses env file)
dotnet user-secrets set "StripeSettings:PublishableKey" "pk_test_YOUR_PUBLISHABLE_KEY"

# Set Stripe secret key (REQUIRED for backend)
dotnet user-secrets set "StripeSettings:SecretKey" "sk_test_YOUR_SECRET_KEY"

# Webhook secret will be set after starting Stripe CLI (Step 4 below)
```

**⚠️ Important:** Never commit actual API keys to git. The `.env` files are tracked but should contain placeholders in the repository.

---

### 1. Start Database

**Option A: Docker (Recommended)**

```powershell
# Start all services including database
docker-compose up -d

# Or start only database
docker-compose up -d restoredb
```

**Option B: Local PostgreSQL**

```powershell
# Ensure PostgreSQL service is running
# Connection: localhost:5432, Database: store
```

### 2. Start Backend API

```powershell
cd server/Services/Restore/Restore.API
dotnet run
```

**Verify:** API should be running at http://localhost:5000

### 3. Start Frontend Client

```powershell
cd client
npm start
```

**Verify:** Client should be running at http://localhost:3000

### 4. Start Stripe Webhook Listener

```powershell
# From repository root
stripe listen -f http://localhost:5000/api/payments/webhook -e charge.succeeded
```

**Verify:** You should see "Ready! Your webhook signing secret is whsec\_..."

### Pre-Test Checklist

- [ ] Database accessible (PostgreSQL on port 5432)
- [ ] API running (http://localhost:5000)
- [ ] Client running (http://localhost:3000)
- [ ] Stripe webhook listener active
- [ ] Browser open to http://localhost:3000

---

## End-to-End Test Workflow

### Step 1: Browse Catalog

1. Navigate to http://localhost:3000
2. Click **CATALOG** in navigation
3. Browse available products
4. Verify product images and prices load correctly

**Expected:** Product catalog displays with images, names, prices

---

### Step 2: Add Items to Basket

1. Select a product (e.g., "Angular Speedster Board 2000")
2. Click on product card to view details
3. Click **ADD TO BASKET** button
4. Increase quantity if desired (use + button)
5. Add more products if testing multiple items

**Expected:**

- Basket icon shows item count
- Success notification appears

---

### Step 3: View Basket

1. Click basket icon (🛒) in header
2. Review items in basket
3. Verify quantities and prices are correct
4. Test quantity adjustments (+/- buttons)
5. Test remove item if needed

**Expected:**

- Basket displays all items
- Subtotal calculates correctly
- Delivery fee shown ($0.00 for orders under threshold)

---

### Step 4: Initiate Checkout

1. Click **CHECKOUT** button from basket page
2. If not logged in, you'll be prompted to login or register

**Expected:** Redirected to login page or checkout flow

---

### Step 5: Login/Register

**Option A: Use Existing Test User**

- Email: `testuser2026@test.com`
- Password: `Test@1234`

**Option B: Create New User**

1. Click **SIGN UP HERE** on login page
2. Enter username, email, password
3. Confirm password
4. Click **SUBMIT**

**Option C: Use Pre-seeded User**

- Username: `bob` or `admin`
- Email: `bob@test.com` or `admin@test.com`
- Password: `Admin_1234`

**Expected:** Logged in and redirected to checkout

---

### Step 6: Enter Shipping Address

1. Fill in shipping details:
   - **Full Name:** Test User
   - **Address Line 1:** 123 Test Street
   - **City:** Test City
   - **State:** NY
   - **Zip Code:** 12345
   - **Country:** USA

2. Click **NEXT** button

**Expected:** Proceed to order review step

---

### Step 7: Review Order

1. Verify order summary:
   - Items and quantities
   - Subtotal
   - Delivery fee
   - Total amount

2. Review shipping address
3. Click **NEXT** to proceed to payment

**Expected:** Proceed to payment step

---

### Step 8: Complete Payment

#### 8.1: Handle Stripe Link (if appears)

If Stripe Link has saved card ending in 4242:

1. **Click on the masked card number** (• • • • 4242) in Card Number field
2. Click **"Stop using Link and enter card details manually"**
3. Proceed to manual entry below

#### 8.2: Enter Card Details Manually

Enter the following test card:

- **Card Number:** `4242 4242 4242 4242`
- **Expiry Date:** `12/28` (any future date)
- **CVV:** `123` (any 3 digits)
- **Name on card:** Should already be filled as your username

#### 8.3: Submit Payment

1. Verify **PLACE ORDER** button is enabled (blue, not grayed out)
2. Click **PLACE ORDER**

**Expected:**

- Loading indicator appears
- Redirected to confirmation page
- Success message: "Thank you - we have received your payment"
- Order number displayed (e.g., "Your order number is #1")

---

### Step 9: Verify Order in Application

1. Note your order number from confirmation page
2. Click on your username dropdown in header
3. Select **My Orders**
4. Verify your order appears in the list:
   - Order number matches
   - Total amount correct
   - Status shows "Pending" (expected due to webhook signature issue)
   - Date is current

5. Click **VIEW** button to see order details
6. Verify:
   - Correct items and quantities
   - Subtotal matches
   - Delivery fee shown
   - Total is correct

**Expected:** Order details match what was purchased

---

### Step 10: Verify Payment in Stripe Dashboard

#### 10.1: Login to Stripe

1. Open https://dashboard.stripe.com/login
2. Enter your Stripe account credentials
3. Login

#### 10.2: Navigate to Navy Rings Sandbox

1. After login, click account switcher (top-left)
2. Select **Navy Rings** sandbox
3. Or navigate directly to: https://dashboard.stripe.com/acct_1SqyU8BSFHw8VMDK/test/dashboard

#### 10.3: View Payment

1. Click **Transactions** in left sidebar
2. Verify recent payment appears:
   - Amount matches order total
   - Status: **Succeeded**
   - Payment method: Visa •••• 4242
   - Customer name matches
   - Date/time is recent

3. Click on the payment to view details
4. Verify:
   - Payment Intent ID starts with `pi_`
   - Charge ID starts with `ch_`
   - CVC check: **Passed**
   - Risk evaluation: **Normal**
   - Payment breakdown shows Stripe fees
   - Net amount = Total - Fees

**Expected:** Payment record shows successful transaction

---

### Step 11: Verify Webhook (Optional)

1. Check the Stripe CLI terminal window
2. Look for webhook event output:
   ```
   --> charge.succeeded [evt_xxx...]
   <-- [500] POST http://localhost:5000/api/payments/webhook
   ```

**Note:** 500 error is expected if webhook secrets don't match. Order is still created successfully.

---

## Test Results Documentation

After completing the test, document your results in `TEST_CREDENTIALS.local.md`:

```markdown
### Test Execution: [Date]

**Results:**

- ✅ Order Number: #X
- ✅ Amount: $XXX.XX
- ✅ Payment Intent: pi_xxx...
- ✅ Charge ID: ch_xxx...
- ✅ Status: Succeeded
- ⚠️ Webhook: [Status]

**Issues Encountered:**

- [None or list any issues]

**Screenshots:**

- [Location of screenshots if taken]
```

---

## Common Issues & Solutions

### Issue: Place Order button stays disabled

**Solution:**

- Ensure all three Stripe Elements are filled (card, expiry, CVV)
- If using Link, complete the full authentication flow
- Or bypass Link by clicking card number and selecting manual entry

### Issue: Stripe Link prevents CVV entry

**Solution:**

1. Click on masked card number (• • • • 4242)
2. Click "Stop using Link and enter card details manually"
3. Enter all card details fresh

### Issue: Webhook returns 500 error

**Status:** Expected behavior (order still created)

**Cause:** Webhook secret in database differs from CLI-generated secret

**Fix:** Update database webhook secret or regenerate CLI secret to match

### Issue: Database connection refused

**Solution:**

```powershell
# Ensure database is running
docker-compose up -d restoredb

# Or check PostgreSQL service status
```

### Issue: Port already in use

**Solution:**

```powershell
# Find and kill process using port 5000 (API)
netstat -ano | findstr :5000
taskkill /PID [process_id] /F

# Or port 3000 (Client)
netstat -ano | findstr :3000
taskkill /PID [process_id] /F
```

---

## Quick Reference

### Test Credentials

- **User:** testuser2026@test.com / Test@1234
- **Card:** 4242 4242 4242 4242, 12/28, 123
- **Address:** Test User, 123 Test Street, Test City, NY 12345, USA

### URLs

- **Client:** http://localhost:3000
- **API:** http://localhost:5000
- **Stripe Dashboard:** https://dashboard.stripe.com/acct_1SqyU8BSFHw8VMDK/test/dashboard

### Stripe Test Cards

- **Success:** 4242 4242 4242 4242
- **Declined:** 4000 0000 0000 0002
- **Requires Auth:** 4000 0025 0000 3155

---

## Related Documentation

- [TEST_CREDENTIALS.local.md](TEST_CREDENTIALS.local.md) - Your test credentials
- [STRIPE.md](../STRIPE.md) - Stripe configuration
- [STRIPE_TEST_SUMMARY.md](../STRIPE_TEST_SUMMARY.md) - Example test results
- [Testing README](README.md) - Testing overview
