Feature: US-03 Buchungsverwaltung über die REST API
  Als System möchte ich Buchungen per REST API anlegen, abfragen, ändern und löschen,
  damit nachgelagerte Prozesse ohne UI auf Bestelldaten arbeiten können.

  Akzeptanzkriterien:
  - AK1: CRUD-Operationen funktionieren über die dokumentierten Endpunkte.
  - AK2: Schreiboperationen erfordern ein gültiges Auth-Token.
  - AK3: Ungültige Anfragen werden mit passenden Statuscodes abgelehnt.
  - AK4: Antworten entsprechen dem dokumentierten Contract.

  @smoke @api
  Scenario: Healthcheck der API
    When ich den API-Healthcheck aufrufe
    Then ist die API erreichbar

  @smoke @api
  Scenario: Auth-Token mit gültigen Zugangsdaten anfordern
    When ich ein Auth-Token mit gültigen Zugangsdaten anfordere
    Then erhalte ich ein gültiges Token

  @api @regression
  Scenario: Auth-Token mit falschen Zugangsdaten wird abgelehnt
    When ich ein Auth-Token mit dem Benutzer "admin" und dem Passwort "falschesPasswort" anfordere
    Then erhalte ich kein Token

  @smoke @api
  Scenario: Neue Buchung anlegen
    Given ich habe eine gültige Buchung für "Max" vorbereitet
    When ich die Buchung über die API anlege
    Then erhalte ich eine Buchungsnummer
    And entsprechen die zurückgegebenen Buchungsdaten der Anfrage

  @api @regression
  Scenario: Angelegte Buchung kann wieder abgerufen werden
    Given ich habe eine gültige Buchung für "Erika" vorbereitet
    And ich die Buchung über die API angelegt habe
    When ich die Buchung über ihre Id abrufe
    Then entsprechen die abgerufenen Buchungsdaten der Anfrage

  @api @regression
  Scenario Outline: Buchungen mit unterschiedlichen Daten anlegen
    Given ich habe eine Buchung mit Vorname "<vorname>", Preis <preis> und Anzahlung "<anzahlung>" vorbereitet
    When ich die Buchung über die API anlege
    Then erhalte ich eine Buchungsnummer
    And hat die angelegte Buchung den Preis <preis> und Anzahlung "<anzahlung>"

    Examples:
      | vorname | preis | anzahlung |
      | Alice   | 111   | true      |
      | Bob     | 0     | false     |
      | Cigdem  | 9999  | true      |

  @api @regression
  Scenario: Buchung aktualisieren
    Given ich habe eine gültige Buchung für "Update" vorbereitet
    And ich die Buchung über die API angelegt habe
    And ich als API-Administrator authentifiziert bin
    When ich den Nachnamen der Buchung auf "Umbenannt" ändere
    Then hat die Buchung beim erneuten Abruf den Nachnamen "Umbenannt"

  @api @regression
  Scenario: Buchung löschen
    Given ich habe eine gültige Buchung für "Delete" vorbereitet
    And ich die Buchung über die API angelegt habe
    And ich als API-Administrator authentifiziert bin
    When ich die Buchung über die API lösche
    Then ist die Buchung nicht mehr abrufbar

  @api @regression
  Scenario: Abruf einer nicht existierenden Buchung liefert keinen Treffer
    When ich die Buchung mit der Id 99999999 abrufe
    Then ist die Buchung nicht vorhanden

  @api @regression
  Scenario: Aktualisierung ohne Auth-Token wird abgelehnt
    Given ich habe eine gültige Buchung für "NoAuth" vorbereitet
    And ich die Buchung über die API angelegt habe
    When ich die Buchung ohne Token zu aktualisieren versuche
    Then wird die Anfrage mit Status 403 abgelehnt

  @api @regression
  Scenario: Anlegen mit unvollständiger Payload wird abgelehnt
    When ich eine Buchung mit unvollständiger Payload anlege
    Then wird die Anfrage mit Status 500 abgelehnt

  @api @regression
  Scenario: Contract-Validierung der Buchungsantwort
    Given ich habe eine gültige Buchung für "Contract" vorbereitet
    And ich die Buchung über die API angelegt habe
    When ich die Buchung über ihre Id abrufe
    Then erfüllt die Antwort den Buchungs-Contract
