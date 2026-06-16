<p align="center">
  <img src="docs/images/banner.svg" alt="ShopGuard – BDD Test Automation Framework" width="700">
</p>

<p align="center">
  <a href="https://github.com/AdrianDeutsch/Qa_Automation/actions/workflows/ci.yml"><img src="https://github.com/AdrianDeutsch/Qa_Automation/actions/workflows/ci.yml/badge.svg?branch=main" alt="CI"></a>
  <img src="https://img.shields.io/badge/coverage-99.6%25-brightgreen" alt="Coverage 99.6%">
  <img src="https://img.shields.io/badge/.NET-10.0_LTS-512BD4?logo=dotnet" alt=".NET 10">
  <img src="https://img.shields.io/badge/Reqnroll-3.3-2aa889" alt="Reqnroll">
  <img src="https://img.shields.io/badge/Playwright-1.60-2EAD33?logo=playwright" alt="Playwright">
  <a href="LICENSE"><img src="https://img.shields.io/badge/License-MIT-yellow.svg" alt="MIT License"></a>
</p>

<p align="center">
  <img src="docs/images/hero-demo.gif" alt="Playwright-Testlauf: Login → Suche → Warenkorb → Checkout" width="700">
  <br><em>E2E-Lauf in Aktion: Login → Produktsuche → Warenkorb → Checkout mit Bestellbestätigung</em>
</p>

**ShopGuard** ist ein BDD-Test-Automation-Framework für einen Demo-Webshop und eine REST API.
Es kombiniert Gherkin-Szenarien (Reqnroll) mit Playwright-UI-Tests, typisierten API-Tests und SQL-Datenbank-Validierung.
Eine GitLab-Pipeline führt Unit-, Smoke-, Feature- und Regression-Suiten tag-basiert und fail-fast aus.

---

## 📑 Inhaltsverzeichnis

- [Features](#-features)
- [Architektur](#-architektur)
- [Screenshots](#-screenshots)
- [Quick Start](#-quick-start)
- [Teststrategie](#-teststrategie)
- [Ordnerstruktur](#-ordnerstruktur)
- [CI/CD-Pipeline](#-cicd-pipeline)
- [Defect-Management](#-defect-management)
- [Bonus: Playwright mit TypeScript](#-bonus-playwright-mit-typescript)

---

## ✨ Features

| Feature | Status | Technologie |
|---|---|---|
| ✅ UI-E2E-Tests (11 Szenarien) | grün (lokal) | Playwright für .NET, Page Object Model |
| ✅ API-Tests (14 Szenarien) | grün (CI) | Typisierter `HttpClient`-Client gegen restful-booker |
| ✅ Unit-Tests (49 Tests, < 1 s) | grün (CI) | NUnit 4 + Moq, gemockter `HttpMessageHandler` |
| ✅ SQL-Validierung (2 Szenarien) | grün (CI) | SQLite + Dapper, ANSI-kompatibles Schema |
| ✅ Code Coverage ≥ 80 % | 99,6 % | Coverlet + ReportGenerator |
| ✅ CI/CD mit Tag-Selektion | live | GitHub Actions (+ `.gitlab-ci.yml`), Fail-Fast-Testpyramide |
| ✅ Screenshots + Traces bei Fehlern | aktiv | Playwright Tracing, CI-Artefakte |

**Testziele:** UI gegen den [Toolshop](https://practicesoftwaretesting.com) (stabile `data-test`-Attribute),
API gegen [restful-booker](https://restful-booker.herokuapp.com).

> **Hinweis zur CI-Ausführung:** Beide Demo-Shops (ursprünglich demo.nopcommerce.com, jetzt der Toolshop)
> sind durch eine **Cloudflare-Bot-Challenge** geschützt, die bei Datacenter-IPs (GitHub-Hosted-Runner)
> auslöst. Die **UI-Tests laufen daher lokal bzw. auf einem Self-hosted-Runner** mit Wohn-IP und sind in
> der Hosted-CI per `TestCategory!=ui` ausgeklammert. Unit-, API- und DB-Tests laufen vollständig in der
> Hosted-CI. Das Umgehen des Bot-Schutzes ist bewusst nicht Teil des Frameworks.

---

## 🏗 Architektur

```mermaid
flowchart LR
  A[Gherkin Features\n.feature, DE] --> B[Step Definitions\nReqnroll + NUnit]
  B --> C[Page Objects]
  B --> D[BookingApiClient\nShopGuard.Core]
  B --> E[OrderRepository\nShopGuard.Core]
  C --> F[Playwright\nChromium]
  D --> G[restful-booker API]
  E --> H[(SQLite\nOrders)]
  I[Hooks\nScreenshot + Trace bei Fehlern] -.-> F
```

Wiederverwendbare Logik (API-Client, Preisberechnung, Testdaten-Generator, Config-Loader, Order-Repository)
liegt in `ShopGuard.Core` und ist vollständig unit-getestet — die E2E-Schicht bleibt dünn.

---

## 📸 Screenshots

<img src="docs/images/coverage-report.png" width="700" alt="Coverage-Report">
<br><em>Coverage-Report (ReportGenerator): 99,6 % Line Coverage für ShopGuard.Core</em>

<img src="docs/images/pipeline-green.png" width="700" alt="GitHub Actions Pipeline">
<br><em>Grüne CI-Pipeline (GitHub Actions): Unit → Smoke, Fail-Fast-Testpyramide</em>

> Weitere Live-Artefakte direkt im <a href="https://github.com/AdrianDeutsch/Qa_Automation/actions">Actions-Tab</a>:
> Coverage-Summary im Job-Step-Summary, sowie Screenshots & Playwright-Traces als Job-Artefakte bei
> fehlgeschlagenen E2E-Läufen (öffnen mit `playwright show-trace <trace.zip>`).
> Defect Reports nach Jira-Template liegen unter [docs/defects/](docs/defects/).

---

## 🚀 Quick Start

1. **Repository klonen und bauen**
   ```bash
   git clone https://github.com/AdrianDeutsch/Qa_Automation.git && cd Qa_Automation
   dotnet build ShopGuard
   ```
2. **Playwright-Browser installieren** (einmalig)
   ```bash
   dotnet tool install --global Microsoft.Playwright.CLI
   playwright install chromium
   ```
3. **Unit-Tests ausführen** (< 1 Sekunde)
   ```bash
   dotnet test ShopGuard/ShopGuard.UnitTests
   ```
4. **Smoke-Suite ausführen** (UI + API, ~1 Minute)
   ```bash
   dotnet test ShopGuard/ShopGuard.E2ETests --filter "TestCategory=smoke"
   ```
5. **Headed zusehen** (Browser sichtbar)
   ```bash
   HEADLESS=false dotnet test ShopGuard/ShopGuard.E2ETests --filter "TestCategory=ui"
   ```

Umgebung wechseln: `SHOPGUARD_ENV=staging` (Overrides in [appsettings.test.json](ShopGuard/ShopGuard.E2ETests/appsettings.test.json)).

---

## 🎯 Teststrategie

```mermaid
flowchart TB
  subgraph Pyramide
    UI["🖥 UI-E2E — 11 Szenarien\nlangsam, höchste Aussagekraft"]
    API["🔌 API + DB — 16 Szenarien\nschnell, Contract & Persistenz"]
    UNIT["⚙️ Unit — 49 Tests\n< 1 s, blockiert alles bei Rot"]
  end
  UNIT --> API --> UI
```

**Tagging-Konzept** (Reqnroll-Tags → NUnit-Kategorien → `--filter "TestCategory=..."`):

| Tag | Bedeutung | Läuft in Stage |
|---|---|---|
| `@smoke` | kritischer Pfad, max. 5 min | `smoke` (jeder Push) |
| `@ui` / `@api` / `@db` | Schicht-Auswahl | `feature` (Merge Requests) |
| `@regression` | kompletter Testsatz | `regression` (nightly Schedule) |
| `@e2e` | kompletter Bestellfluss | `feature` + `regression` |
| `@wip` | in Arbeit / instabil ([FLAKY-TESTS.md](FLAKY-TESTS.md)) | nirgends |

---

## 📁 Ordnerstruktur

```
Qa_Automation/
├── ShopGuard/
│   ├── ShopGuard.Core/          # Wiederverwendbar: ApiClient, Helper, Models, OrderRepository
│   ├── ShopGuard.UnitTests/     # 49 NUnit-Tests, Moq-gemockter HttpMessageHandler
│   └── ShopGuard.E2ETests/
│       ├── Features/
│       │   ├── UI/              # Login, Warenkorb, Suche, Checkout (Gherkin, DE)
│       │   └── API/             # Buchungs-CRUD, Auth, Negativtests, DB-Validierung
│       ├── StepDefinitions/     # Bindings, dünn — Logik liegt in Pages/Core
│       ├── Pages/               # Page Object Model (data-test-Selektoren)
│       ├── Database/            # SQL-Schema + scenario-scoped DB-Kontext
│       ├── Hooks/               # Browser-Lifecycle, Screenshot + Trace bei Fehlern
│       ├── Support/             # Settings, PlaywrightDriver, ScenarioState
│       └── reqnroll.json
├── playwright-ts/               # Bonus: 3 UI-Szenarien in Playwright Test (TypeScript)
├── docs/
│   ├── defects/                 # 3 Defect Reports nach Jira-Template
│   ├── JIRA-BUG-TEMPLATE.md
│   └── images/
├── FLAKY-TESTS.md               # Erkennung & Behandlung instabiler Tests
├── .github/workflows/ci.yml     # Live-Pipeline (GitHub Actions), Fail-Fast-Testpyramide
└── .gitlab-ci.yml               # Gleichwertige GitLab-Variante (Stages, Tags, Pages)
```

---

## 🔁 CI/CD-Pipeline

Die Pipeline läuft live als **GitHub Actions** ([ci.yml](.github/workflows/ci.yml)) — die
**[.gitlab-ci.yml](.gitlab-ci.yml)** ist als gleichwertige GitLab-Variante (Stages, Tags, GitLab Pages)
beigelegt und sofort einsetzbar. Beide bilden dieselbe Fail-Fast-Testpyramide ab:

<details>
<summary><b>Stages & Trigger im Überblick</b> (Klick zum Aufklappen)</summary>

| Job / Stage | Trigger | Inhalt | Besonderheit |
|---|---|---|---|
| `unit` | jeder Push | 49 Unit-Tests + Coverage | **Fail Fast** — rot ⇒ nichts läuft weiter |
| `smoke` | jeder Push | `TestCategory=smoke&TestCategory!=ui` (API) | `needs: unit`, Timeout, Retry bei Flakiness |
| `e2e` / `feature` | Pull / Merge Request | `@api` + `@db` | Screenshots/Traces als Artefakte |
| `ui` (GitLab) | MR / nightly, **Self-hosted** | `@ui`-Szenarien | Wohn-IP umgeht Cloudflare, `allow_failure` |
| `regression` | nightly Schedule (02:00 UTC) | kompletter Satz `TestCategory!=ui` | inkl. Unit + API + DB |
| Coverage | jeder Push | Cobertura → Job-Summary (Actions) / Pages (GitLab) | ReportGenerator |

</details>

Bei fehlgeschlagenen UI-Szenarien landen **Full-Page-Screenshot** und **Playwright-Trace**
automatisch als CI-Artefakte (`artifacts/<Szenario>.png` / `.trace.zip`) — Analyse mit:

```bash
playwright show-trace artifacts/<Szenario>.trace.zip
```

---

## 🐞 Defect-Management

Drei real gefundene und dokumentierte Defects (Template: [JIRA-BUG-TEMPLATE.md](docs/JIRA-BUG-TEMPLATE.md)):

| ID | Komponente | Kurzbeschreibung | Root Cause |
|---|---|---|---|
| [DEFECT-001](docs/defects/DEFECT-001.md) | API | `POST /booking` antwortet **418** statt 4xx ohne Accept-Header | Produktfehler |
| [DEFECT-002](docs/defects/DEFECT-002.md) | API | `POST /auth` liefert **200** bei falschen Credentials | Produktfehler |
| [DEFECT-003](docs/defects/DEFECT-003.md) | UI | Checkout blockiert: Hausnummer aus Registrierung fehlt im Profil | Produktfehler |

Workflow: Fehleranalyse (Trace/Logs) → Klassifikation (Produkt-/Testfehler/flaky) → Report → Workaround mit Verweis → Retest.

---

## 🎁 Bonus: Playwright mit TypeScript

Drei UI-Szenarien sind zusätzlich in **Playwright Test (TypeScript)** portiert — gleiche
Page-Object-Struktur, gleiche `data-test`-Selektoren:

```bash
cd playwright-ts
npm install && npx playwright install chromium
npx playwright test
```
