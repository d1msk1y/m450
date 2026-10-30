# Anforderungen: Rabattberechnung
R1: Der Warenwert ist ein decimal-Betrag in CHF und darf nicht negativ sein. Bei negativen Werten muss ArgumentOutOfRangeException entstehen.
R2: Ab einschliesslich CHF 100.00 beträgt der Rabatt 10 Prozent.
R3: Unter CHF 100.00 gibt es keinen Rabatt.
R4: Der Rückgabewert ist der zu zahlende Endbetrag nach Abzug des Rabatts.
R5: Der Endbetrag wird auf zwei Dezimalstellen gerundet. Bei einer exakten Mitte gilt MidpointRounding.AwayFromZero.
R6: Der Kundenname darf null sein. Die Begrüssung lautet dann "Hallo Gast". Andere Namen werden unverändert mit "Hallo " begrüsst.
