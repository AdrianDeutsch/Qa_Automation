Feature: Produktsuche
  Als Kunde möchte ich Produkte über die Suche finden,
  damit ich nicht durch den gesamten Katalog blättern muss.

  Akzeptanzkriterien:
  - AK1: Die Suche nach einem vorhandenen Begriff liefert passende Produkte.
  - AK2: Die Suche ohne Treffer zeigt einen verständlichen Hinweis.

  @smoke @ui
  Scenario: Suche findet vorhandene Produkte
    Given ich bin auf der Startseite des Shops
    When ich nach "Pliers" suche
    Then sehe ich mindestens 1 Suchergebnis
    And enthält ein Suchergebnis den Text "Pliers"

  @ui @regression
  Scenario: Suche ohne Treffer zeigt einen Hinweis
    Given ich bin auf der Startseite des Shops
    When ich nach "xyzgibtesnicht123" suche
    Then sehe ich den Hinweis, dass keine Produkte gefunden wurden
