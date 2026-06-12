Feature: Bestellvalidierung in der Datenbank
  Als QA möchte ich nach API-Aktionen den Datenbankzustand verifizieren,
  damit ich sicherstelle, dass das Backend Bestellungen korrekt persistiert.

  Akzeptanzkriterien:
  - AK1: Nach dem Anlegen einer Buchung existiert genau ein Orders-Datensatz mit Status "Pending".
  - AK2: Nach einer Stornierung wechselt der Status auf "Cancelled".

  @db @api @regression
  Scenario: Nach dem Anlegen einer Buchung wird die Bestellung mit Status Pending persistiert
    Given ein Kunde mit einer eindeutigen E-Mail-Adresse
    And ich habe eine gültige Buchung für "DbCheck" vorbereitet
    When ich die Buchung über die API anlege
    And das Backend die Bestellung zur Buchung registriert
    Then existiert in der Tabelle Orders genau ein Datensatz für den Kunden
    And hat die Bestellung den Status "Pending"

  @db @regression
  Scenario: Nach der Stornierung hat die Bestellung den Status Cancelled
    Given ein Kunde mit einer eindeutigen E-Mail-Adresse
    And ich für den Kunden eine Bestellung über 59.90 angelegt habe
    When ich die Bestellung storniere
    Then hat die Bestellung den Status "Cancelled"
