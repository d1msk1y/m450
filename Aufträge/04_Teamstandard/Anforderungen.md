# Kontext und Anforderungen
Ein Team erstellt SHA-256-Prüfsummen für freigegebene Dateiinhalte. Die freigegebene Referenz-Prüfsumme wird separat vor Manipulation geschützt.
R1: Create erhält ein byte-Array und liefert den SHA-256-Hash als grossgeschriebene Hex-Zeichenfolge ohne Trennzeichen.
R2: Für null wird ArgumentNullException mit ParamName content ausgelöst.
R3: Die Berechnung verändert die Eingabedaten nicht.
R4: Der gemeinsame Teamstandard in .editorconfig und Directory.Build.props gilt für das gesamte Projekt.
R5: Der Build und die separate Formatprüfung müssen erfolgreich sein.

Ein Hash allein authentifiziert keine Datei. Die Referenz muss vertrauenswürdig sein. Für Nachrichten-Authentisierung und Passwörter gelten andere Anforderungen. Dieses Beispiel ist kein Passwortspeicher.
