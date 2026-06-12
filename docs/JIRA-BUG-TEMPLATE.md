# Jira Bug Template

> Vorlage für Defect Reports. Felder 1:1 in Jira übernehmen (Issue Type: **Bug**).

| Feld | Inhalt |
|---|---|
| **Summary** | `[Komponente] Kurzbeschreibung des Fehlverhaltens (max. 80 Zeichen)` |
| **Issue Type** | Bug |
| **Severity** | Blocker / Critical / Major / Minor / Trivial |
| **Priority** | Highest / High / Medium / Low |
| **Environment** | App-Version, Browser + Version, OS, Test-Umgebung (local/staging), Testdaten |
| **Affected Component** | UI / API / Datenbank / CI |
| **Found in** | Pipeline-Job-Link oder manueller Test |

## Steps to Reproduce

1. Schritt 1 (präzise, mit konkreten Daten)
2. Schritt 2
3. …

## Expected Result

Was laut Anforderung/Akzeptanzkriterium passieren müsste — mit Quelle (User Story, API-Doku).

## Actual Result

Was tatsächlich passiert — mit exakter Fehlermeldung, Statuscode, Stacktrace.

## Attachments

- [ ] Screenshot (`artifacts/<Szenario>.png`)
- [ ] Playwright-Trace (`artifacts/<Szenario>.trace.zip`, öffnen mit `playwright show-trace`)
- [ ] Request/Response-Log (bei API-Bugs)
- [ ] Link auf fehlgeschlagenen CI-Job

## Root-Cause-Klassifikation (nach Analyse)

- [ ] **Produktfehler** — App verhält sich falsch
- [ ] **Testfehler** — Test/Testdaten/Erwartung falsch
- [ ] **Flaky Test** — instabile Wartebedingung, Timing, Umgebung

## Retest

| Datum | Version/Commit | Ergebnis | Tester |
|---|---|---|---|
| | | | |
