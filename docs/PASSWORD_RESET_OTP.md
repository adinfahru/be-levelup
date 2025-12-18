Password reset (OTP) — Frontend & API guide

Overview

This document describes the password-change flow using a one-time PIN (OTP) sent by email. It covers API endpoints, request/response examples, UX recommendations, validation rules, and error handling for the frontend.

API Endpoints

- POST /api/v1/auth/password/request
  - Body: { "email": "user@example.com" }
  - Behaviour: Always returns success 200 (to avoid leaking account existence). If the email exists the server:
    - generates a 6-digit numeric OTP
    - hashes OTP (bcrypt) and stores it in account.otp (OtpHash)
    - sets otp_expires_at (15 minutes) and otp_attempts = 0
    - sends OTP email using existing `IEmailHandler`
  - Success response:
    - 200 OK { "success": true, "message": "If the email exists, an OTP has been sent." }

- POST /api/v1/auth/password/confirm
  - Body: { "email": "user@example.com", "otp": "123456", "newPassword": "NewPass123!" }
  - Behaviour:
    - find account by email
    - if not found or OTP not set/expired → return 400/422 with generic error
    - if otp_attempts >= 3 → return 400 (max attempts)
    - validate OTP using bcrypt verify
    - if invalid → increment otp_attempts, return 400 ("Invalid OTP")
    - if valid → hash new password, save PasswordHash, clear OTP fields, send confirmation email
  - Success response:
    - 200 OK { "success": true, "message": "Password changed successfully" }

Optional endpoint (server-side):
- POST /api/v1/auth/password/resend
  - Same semantics as `request` but should be rate-limited (server-side) and enforce cooldown (e.g., 60s) and max resend per hour/day.

Frontend (FE) Flow

1. Request OTP screen
   - UI: input `email`, button `Send OTP`
   - On submit: call `POST /api/v1/auth/password/request`.
   - Always show neutral message: "If the email exists, an OTP has been sent." and show OTP entry UI.
   - Start a countdown timer on the FE (15:00) visually indicating expiry.
   - Disable resend for a short cooldown (60s) and show attempts left if server returns that info (server may choose to return attempts left; current implementation does not).

2. OTP + New Password screen
   - Inputs: `otp` (6 digits), `newPassword`, `confirmPassword`.
   - Validation (FE):
     - OTP: numeric, 6 digits
     - Password: enforce same rules as create (min length, uppercase, lowercase, number, special char)
     - confirmPassword must match
   - UX: provide "Resend OTP" button (disabled during cooldown), show remaining attempts and expiry countdown.
   - On submit: call `POST /api/v1/auth/password/confirm` with { email, otp, newPassword }.
   - Error handling:
     - If OTP invalid: show "Invalid OTP" with remaining attempts (if server provides), allow retry until attempts exhausted.
     - If OTP expired: show message and offer `Resend OTP`.
     - If max attempts exceeded: prompt user to request a new OTP (call `request` again).
   - On success: show confirmation message and redirect to login.

Security & UX recommendations

- Do not reveal whether an email exists on the `request` endpoint. Always return 200.
- Show generic messages for failures (avoid confirming user existence).
- Use HTTPS for all calls.
- Rate limit OTP requests and implement server-side cooldown (e.g., 1 request per 60s, 5 requests per hour).
- Limit OTP attempts to 3 (server enforces). On exhaustion, require new OTP and reset attempts.
- Store hashed OTP (bcrypt) on the server (done). OTP is numeric but store hashed.
- OTP lifetime: 10–15 minutes (server uses 15 minutes).
- Optionally log events (request, success, fail) for security audits.

Email content guidelines

- Subject: "[LevelUp] Password change OTP"
- Body (HTML): short, includes OTP in bold, expiry info, and a note to ignore if not requested.
- Do not include password or any link that automatically resets password without OTP.

FE Example (JS fetch)

Request OTP:

```js
fetch('/api/v1/auth/password/request', {
  method: 'POST',
  headers: { 'Content-Type': 'application/json' },
  body: JSON.stringify({ email })
});
```

Confirm OTP and change password:

```js
fetch('/api/v1/auth/password/confirm', {
  method: 'POST',
  headers: { 'Content-Type': 'application/json' },
  body: JSON.stringify({ email, otp, newPassword })
});
```

Notes for integration

- Backend already uses `IEmailHandler` and `IHashHandler` (bcrypt) — no extra deps.
- Database migration required to add `otp_expires_at` and `otp_attempts` columns. The code reuses the `otp` column name to store the hashed OTP; migrations should ensure column length is sufficient (varchar(512)).
- The API intentionally returns minimal/safe messages to avoid user enumeration.

Docs location

- This file: [docs/PASSWORD_RESET_OTP.md](docs/PASSWORD_RESET_OTP.md)

If you want, I can also:
- Add REST Client examples into `LevelUp.API/api-tests.http` for quick manual testing.
- Generate the EF migration files (you previously ran migration command locally). Want me to create the migration files in source control instead? 

What should I do next?