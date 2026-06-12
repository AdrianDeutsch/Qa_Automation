Feature: US-01 Login
  Als Kunde möchte ich mich mit meinen Zugangsdaten anmelden können,
  damit ich auf mein Konto und meine Bestellungen zugreifen kann.

  Akzeptanzkriterien:
  - AK1: Mit gültiger E-Mail und gültigem Passwort wird der Kunde angemeldet.
  - AK2: Bei falschem Passwort erscheint eine Fehlermeldung, der Kunde bleibt abgemeldet.
  - AK3: Bei unbekannter E-Mail-Adresse erscheint dieselbe generische Fehlermeldung
         (kein User-Enumeration-Leak).
  - AK4: Syntaktisch ungültige E-Mail-Adressen werden bereits am Feld validiert.

  @smoke @ui
  Scenario: Erfolgreicher Login mit gültigen Zugangsdaten
    Given ich besitze ein registriertes Kundenkonto
    And ich bin auf der Login-Seite
    When ich mich mit gültigen Zugangsdaten anmelde
    Then bin ich als Kunde angemeldet

  @ui @regression
  Scenario: Login mit falschem Passwort wird abgelehnt
    Given ich besitze ein registriertes Kundenkonto
    And ich bin auf der Login-Seite
    When ich mich mit dem falschen Passwort "Falsch!2026x" anmelde
    Then sehe ich die Fehlermeldung "Invalid email or password"
    And bin ich nicht angemeldet

  @ui @regression
  Scenario: Login mit unbekannter E-Mail-Adresse wird abgelehnt
    Given ich bin auf der Login-Seite
    When ich mich mit der E-Mail "ghost.user.shopguard@example.com" und dem Passwort "Egal!2026x" anmelde
    Then sehe ich die Fehlermeldung "Invalid email or password"

  @ui @regression
  Scenario Outline: Login mit ungültiger E-Mail-Eingabe zeigt Feldvalidierung
    Given ich bin auf der Login-Seite
    When ich mich mit der E-Mail "<email>" und dem Passwort "<passwort>" anmelde
    Then sehe ich einen Validierungsfehler am E-Mail-Feld

    Examples:
      | email           | passwort   |
      | kein-at-zeichen | Pass!2026x |
      | unvollstaendig@ | Pass!2026x |
