# DEFECT-002 — [API] POST /auth liefert HTTP 200 bei falschen Zugangsdaten

| Feld | Inhalt |
|---|---|
| **Summary** | [API] POST /auth antwortet mit 200 OK + `{"reason":"Bad credentials"}` statt 401 Unauthorized |
| **Issue Type** | Bug |
| **Severity** | Major |
| **Priority** | Medium |
| **Environment** | restful-booker.herokuapp.com (Prod-Demo), getestet am 2026-06-12 |
| **Affected Component** | API / Auth |
| **Found in** | E2E-Stage `feature`, Szenario "Auth-Token mit falschen Zugangsdaten wird abgelehnt" |

## Steps to Reproduce

1. POST auf `/auth` mit ungültigen Zugangsdaten senden:

```bash
curl -i -X POST https://restful-booker.herokuapp.com/auth \
  -H "Content-Type: application/json" \
  -d '{"username":"admin","password":"falsch"}'
```

## Expected Result

`401 Unauthorized` (Standard-Semantik für fehlgeschlagene Authentifizierung),
damit Clients den Fehlerfall über den Statuscode erkennen können.

## Actual Result

`200 OK` mit Body `{"reason":"Bad credentials"}`. Clients müssen den Body parsen,
um Erfolg von Misserfolg zu unterscheiden; generische HTTP-Fehlerbehandlung greift nicht.

## Attachments

- [x] Request/Response-Log (curl oben)

## Root-Cause-Klassifikation

- [x] **Produktfehler** — API-Contract verletzt HTTP-Semantik.

**Analyse:** Der Quirk ist im Client explizit modelliert (`AuthResponse.Reason`,
`AuthenticateAsync` liefert `null` statt Token) und per Unit-Test
`AuthenticateAsync_OnBadCredentials_ReturnsNull` dokumentiert, damit das Verhalten
bei einem späteren Fix der API kontrolliert auffällt.

## Retest

| Datum | Version/Commit | Ergebnis | Tester |
|---|---|---|---|
| 2026-06-12 | Quirk im Client modelliert | ✅ Szenario grün (Erwartung dokumentiert Ist-Verhalten) | ShopGuard CI |
