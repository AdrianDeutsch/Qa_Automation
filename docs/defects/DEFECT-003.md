# DEFECT-003 — [UI] Checkout-Wizard verlangt Hausnummer, Registrierung persistiert sie nicht

| Feld | Inhalt |
|---|---|
| **Summary** | [UI] "Proceed"-Button im Billing-Schritt bleibt deaktiviert, weil die bei der Registrierung erfasste Hausnummer im Profil fehlt |
| **Issue Type** | Bug |
| **Severity** | Minor |
| **Priority** | Medium |
| **Environment** | practicesoftwaretesting.com (Toolshop v5.0), Chromium 1223 headless, macOS, 2026-06-12 |
| **Affected Component** | UI / Checkout |
| **Found in** | E2E-Stage `regression`, Szenario "Kompletter Bestellprozess von Login bis Bestellbestätigung" |

## Steps to Reproduce

1. Neues Konto unter `/auth/register` anlegen — Feld **House number** wird ausgefüllt (Pflichtfeld).
2. Einloggen, Produkt in den Warenkorb legen, `/checkout` öffnen.
3. Schritte "Cart" und "Sign in" bestätigen.
4. Im Schritt "Billing Address" das Feld **House number** betrachten.

## Expected Result

Die Rechnungsadresse ist vollständig aus dem Kundenprofil vorbefüllt — inklusive Hausnummer,
da sie bei der Registrierung verpflichtend erfasst wurde. "Proceed to checkout" ist aktiv.

## Actual Result

Alle Adressfelder sind vorbefüllt, **House number ist leer**. Der Button
`proceed-3` bleibt `disabled`, bis die Hausnummer manuell nachgetragen wird.

## Attachments

- [x] Playwright-Trace des fehlgeschlagenen Erstlaufs (`artifacts/Kompletter_Bestellprozess…trace.zip`)
- [x] Feld-Dump des Billing-Schritts: `house_number = ''`, alle übrigen Felder gefüllt

## Root-Cause-Klassifikation

- [x] **Produktfehler** — Registrierungsdaten werden unvollständig ins Profil übernommen.

**Analyse:** Zuerst als Testfehler vermutet (Timeout beim Klick auf `proceed-3`).
Der Trace zeigte den deaktivierten Button; der Feld-Dump die leere Hausnummer.
Workaround im Test: `CheckoutPage.CompleteBillingAddressAsync` trägt die Hausnummer nach,
falls leer — mit Kommentar auf diesen Defect.

## Retest

| Datum | Version/Commit | Ergebnis | Tester |
|---|---|---|---|
| 2026-06-12 | Workaround im Page Object | ✅ E2E-Szenario grün | ShopGuard CI |
