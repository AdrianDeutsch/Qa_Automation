Feature: US-02 Warenkorb
  Als Kunde möchte ich Produkte in den Warenkorb legen,
  damit ich sie später bestellen kann.

  Akzeptanzkriterien:
  - AK1: Ein hinzugefügtes Produkt erscheint im Warenkorb, die Summe entspricht dem Produktpreis.
  - AK2: Bei mehreren Produkten entspricht die Summe der Summe aller Einzelpreise.
  - AK3: Entfernt der Kunde alle Produkte, ist der Warenkorb leer.

  @smoke @ui
  Scenario: Produkt erfolgreich in den Warenkorb legen
    When ich das Produkt "Combination Pliers" über die Suche in den Warenkorb lege
    Then zeigt der Warenkorb 1 Artikel an
    And die Gesamtsumme entspricht der Summe der hinzugefügten Produkte

  @ui @regression
  Scenario: Zwei Produkte ergeben die korrekte Gesamtsumme
    When ich das Produkt "Combination Pliers" über die Suche in den Warenkorb lege
    And ich das Produkt "Claw Hammer with Shock Reduction Grip" über die Suche in den Warenkorb lege
    Then zeigt der Warenkorb 2 Artikel an
    And die Gesamtsumme entspricht der Summe der hinzugefügten Produkte

  @ui @regression
  Scenario: Entfernen des letzten Produkts leert den Warenkorb
    Given ich das Produkt "Combination Pliers" über die Suche in den Warenkorb gelegt habe
    When ich das Produkt aus dem Warenkorb entferne
    Then ist der Warenkorb leer
