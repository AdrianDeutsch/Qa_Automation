# FLAKY-TESTS.md — Umgang mit instabilen Tests

## Wie erkenne ich Flaky Tests?

| Signal | Werkzeug |
|---|---|
| Test ist rot, der Wiederholungslauf grün — ohne Codeänderung | CI-Retry-Statistik (`retry: max: 2` markiert solche Jobs) |
| Fehlerbild ist ein Timeout, kein Assertion-Fehler | JUnit-Report: `TimeoutException` vs. `Should().Be(...)` |
| Fehler tritt nur unter Last/parallel auf | Nightly-Regression vs. lokaler Einzellauf |
| Trace zeigt korrektes App-Verhalten, nur später als erwartet | `playwright show-trace artifacts/<szenario>.trace.zip` |

**Regel:** Jeder rote E2E-Test wird zuerst klassifiziert (Produktfehler / Testfehler / flaky),
bevor er erneut gestartet wird — sonst verschleiert der Retry echte Bugs.
Beispiel aus diesem Projekt: der Checkout-Timeout sah flaky aus, war aber DEFECT-003 (Produktfehler).

## Ursachen & Gegenmaßnahmen in diesem Framework

### 1. Warten (häufigste Ursache)

- **Keine fixen Sleeps.** Statt `WaitForTimeout` nutzen alle Page Objects Playwrights Auto-Waiting
  plus explizite Zustands-Waits (`Locator.WaitForAsync`).
- **Auf Ergebnis warten, nicht auf Aktivität:** Die Suche wartet zweistufig — erst auf die
  "Searched for:"-Caption (Suche wurde angewendet), dann auf Produktkarten *oder* die
  No-Results-Meldung ([BasePage.cs](ShopGuard/ShopGuard.E2ETests/Pages/BasePage.cs)).
  So matchen keine veralteten DOM-Knoten der vorherigen Ansicht.
- **UI-Zustandswechsel abwarten:** Der Finish-Button im Checkout wird erst geklickt, wenn er zu
  "Confirm" umbeschriftet wurde — vorher träfe der Klick den alten Handler
  ([CheckoutPage.cs](ShopGuard/ShopGuard.E2ETests/Pages/CheckoutPage.cs)).

### 2. Selektoren

- Nur stabile, semantische Selektoren: `data-test`-Attribute, keine CSS-Klassenketten oder XPath-Indizes.
- Selektoren leben ausschließlich in Page Objects — eine Änderung der App trifft genau eine Datei.

### 3. Testdaten-Isolation

- Jedes Szenario registriert ein **eigenes Konto** (`TestDataGenerator.UniqueEmail`) — keine
  geteilten User, keine Reihenfolge-Abhängigkeiten.
- Jedes UI-Szenario läuft in einem **frischen BrowserContext** (eigene Cookies/LocalStorage, eigener Warenkorb).
- @db-Szenarien räumen ihre Orders-Zeilen im `AfterScenario`-Hook wieder ab.

### 4. Externe Abhängigkeiten

- Die Demo-Systeme (Toolshop, restful-booker) sind öffentlich und gelegentlich langsam.
  Dafür gilt: großzügige, zentral konfigurierte Timeouts (`DefaultTimeoutMs` in
  `appsettings.test.json`) und Job-Retry in der CI — aber **kein** Retry auf Szenario-Ebene,
  damit Instabilität sichtbar bleibt.

## Workflow bei Verdacht auf Flakiness

1. Trace + Screenshot aus den CI-Artefakten laden.
2. Lokal 5× gezielt ausführen: `dotnet test --filter "Name~<Szenario>"`.
3. Klassifizieren: Produktfehler → Defect Report; Testfehler → fixen; flaky → Wait/Selektor/Datenisolation prüfen.
4. Bis zur Behebung: Szenario mit `@wip` taggen (läuft nicht in smoke/feature), Ticket verlinken — niemals stillschweigend löschen.
