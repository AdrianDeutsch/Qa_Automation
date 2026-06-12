# DEFECT-001 — [API] POST /booking antwortet mit 418 statt 4xx bei fehlendem Accept-Header

| Feld | Inhalt |
|---|---|
| **Summary** | [API] POST /booking liefert 418 "I'm a Teapot" statt sinnvollem 4xx, wenn der Accept-Header fehlt |
| **Issue Type** | Bug |
| **Severity** | Major |
| **Priority** | High |
| **Environment** | restful-booker.herokuapp.com (Prod-Demo), .NET 10 HttpClient, getestet am 2026-06-12 |
| **Affected Component** | API |
| **Found in** | E2E-Stage `smoke`, Szenario "Neue Buchung anlegen" |

## Steps to Reproduce

1. POST auf `https://restful-booker.herokuapp.com/booking` mit gültiger JSON-Payload senden.
2. Dabei **keinen** `Accept`-Header mitschicken (Standardverhalten von .NET `HttpClient`).
3. Antwort betrachten.

```bash
curl -i -X POST https://restful-booker.herokuapp.com/booking \
  -H "Accept:" -H "Content-Type: application/json" \
  -d '{"firstname":"Max","lastname":"Tester","totalprice":100,"depositpaid":true,"bookingdates":{"checkin":"2026-07-01","checkout":"2026-07-04"}}'
```

## Expected Result

`200 OK` mit Buchungs-Echo — oder, falls der Server einen Accept-Header erzwingen will,
`406 Not Acceptable` mit aussagekräftiger Fehlermeldung.

## Actual Result

`418 I'm a Teapot` mit Body `I'm a Teapot`. Der Statuscode ist laut RFC 2324 ein Scherz-Code
und für Clients nicht interpretierbar.

## Attachments

- [x] Request/Response-Log (siehe curl oben; mit `Accept: application/json` antwortet derselbe Request mit 200)
- [x] CI-Job: E2E-Lauf vom 2026-06-12, 10 von 16 API-Szenarien rot

## Root-Cause-Klassifikation

- [x] **Produktfehler** — Server beantwortet fehlenden Accept-Header mit Scherz-Statuscode.

**Analyse:** Der Test selbst war korrekt; auslösend war, dass .NET `HttpClient` ohne expliziten
`Accept`-Header sendet. Als Workaround setzt `BookingApiClient` jetzt immer `Accept: application/json`
(abgesichert durch Unit-Test `EveryRequest_SendsAcceptApplicationJson`).

## Retest

| Datum | Version/Commit | Ergebnis | Tester |
|---|---|---|---|
| 2026-06-12 | Workaround im Client | ✅ Alle 16 API-/DB-Szenarien grün | ShopGuard CI |
