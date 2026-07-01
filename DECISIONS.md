# DECISIONS.md

## 1. What did AI generate for you, and what did you write or modify yourself?

I used Claude to scaffold the full project under a 2-day deadline: the Clean Architecture
project structure (Domain/Application/Infrastructure/Api), all entities and EF Core
configurations, the FluentValidation validators, the JWT auth setup, the global exception
middleware, the DB seeder, and the React front-end (list/detail views, JWT login flow, API
client).

What I did myself:
- Reviewed and ran every endpoint against Swagger/curl and confirmed the responses
  match the task's required routes and behavior.
- Generated the actual EF Core migration locally (`dotnet ef migrations add`) and
  inspected the generated SQL/schema — the AI could describe the intended schema but couldn't
  run `dotnet ef` in its own sandbox (no .NET SDK there), so this step was fully mine.
- Decided the business rule for what happens on `/distribute` when a track is
  resubmitted to a DSP it was already sent to (skip if Pending/Live, allow resubmission if
  Rejected)
- Reviewed the JWT-protection scope (which endpoints require auth) and confirmed it
  matches what I'd actually want in production for a distribution workflow.


## 2. What security issues did you find (or introduce) in the AI-generated code? How did you handle them?

Issues identified in the AI-generated code, and how they were handled:

1. **Dev signing key and demo credentials committed to source control**
   (`appsettings.Development.json`). This is fine for a take-home demo but is a real anti-pattern
   — in production this must move to environment variables, `dotnet user-secrets`, or a secret
   manager (Azure Key Vault / AWS Secrets Manager), and the JWT signing key must be rotated
   before any real deployment. I left it committed only because reviewers need to run the
   project out of the box; I documented this tradeoff explicitly rather than hiding it.

2. **No rate limiting on `/api/auth/token`**. As shipped, the login endpoint has no throttling,
   which makes it brute-forceable.

3. **No password hashing / real identity system**. The "auth" is a single demo credential pair
   compared with a plain string equality check — acceptable to satisfy "at least one JWT-protected
   endpoint" for this task, but not a real user system. A production version needs ASP.NET Core
   Identity (or similar) with hashed passwords, per-user roles, and refresh tokens.

4. **CORS is currently open to a single configured origin**, which is correct, but double-check
   the deployed value doesn't end up wildcarded (`*`) if you later add environment-specific
   config for grading/demo purposes.

## 3. One thing the AI got wrong that you had to fix. What was it and why was it wrong?


- The join entity was initially going to be named `TrackDistribution`, which collides with the
  root solution/namespace (`TrackDistribution.*`) and would cause ambiguous-reference compiler
  errors — renamed to `TrackDistributionRecord`, mapped back to a `TrackDistributions` table via
  EF configuration so the API/DB naming still matches the task spec.
