Feature: Checkout End-to-End
  Als Kunde möchte ich gefundene Produkte bestellen können,
  damit ich sie geliefert bekomme.

  Akzeptanzkriterien:
  - AK1: Ein angemeldeter Kunde kann den Checkout-Wizard vollständig durchlaufen.
  - AK2: Nach dem Abschluss erhält der Kunde eine Bestellbestätigung mit Rechnungsnummer.

  @ui @regression @e2e
  Scenario: Kompletter Bestellprozess von Login bis Bestellbestätigung
    Given ich besitze ein registriertes Kundenkonto
    And ich bin als Kunde angemeldet
    When ich das Produkt "Combination Pliers" über die Suche in den Warenkorb lege
    And ich zur Kasse gehe
    And ich den Checkout mit Standardadresse und Zahlung per Nachnahme abschließe
    Then erhalte ich eine Bestellbestätigung mit Rechnungsnummer
