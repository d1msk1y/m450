# Teamauftrag: gemeinsamer Standard
Öffne Teamstandard.csproj oder diesen Projektordner in deiner C#-Umgebung. .NET 8 SDK erforderlich. Keine externen NuGet-Pakete.
Kopiert den ganzen Ordner inklusive .editorconfig und Directory.Build.props. Die Regeldatei bleibt im Repository, nicht nur auf einem persönlichen Rechner.

## 1. Gemeinsame Ausgangslage
`dotnet restore`
`dotnet build --no-incremental`
Erwartung: Der Build scheitert absichtlich an CA5351 (MD5). CA1822 und IDE0040 sind zusätzliche Hinweise. Wortlaut und weitere Meldungen können mit dem SDK variieren.

## 2. Format und Analyse unterscheiden
`dotnet format whitespace --verify-no-changes --no-restore`
Die absichtlich uneinheitliche Einrückung führt zu einem Fehlerstatus. Das ist eine separate Formatprüfung.
`dotnet format whitespace --no-restore`
Dadurch werden nur Formatierungsprobleme behoben. MD5 bleibt bestehen. Diff gemeinsam ansehen.

## 3. Inhalt prüfen und korrigieren
Erklärt jede Meldung. Korrigiert den Code gemäss Anforderungen.md und dem Teamstandard.
Für den Security-Befund die Begründung verstehen. Die Regel darf nicht zur Umgehung des Problems deaktiviert werden.
Prüft R2 ausdrücklich: Eine Security-Regel erkennt nicht automatisch alle fachlichen Vorgaben.
Wenn FingerprintService statisch wird, müssen die Aufrufe in Program.cs mitgeändert werden.

## 4. Gemeinsame Prüfung wie vor einer Übernahme
`dotnet build --no-incremental -warnaserror`
`dotnet format whitespace --verify-no-changes --no-restore`
Beide Befehle müssen erfolgreich sein. Derselbe Ablauf kann im CI-Job laufen. Auch dort ist das gleiche SDK mit denselben Referenzpaketen erforderlich.

## Teamrollen und Abgabe
Person A korrigiert, Person B prüft den Diff. Nach der Formatierung Rollen tauschen.
Abgeben: korrigierter Code, gemeinsame Konfiguration und kurze Begründung für zwei Regelentscheidungen. Nennt zusätzlich einen Fehler, den dieser Standard nicht zuverlässig erkennt.
Bei Git: Formatierung und funktionale/Security-Korrektur in getrennten Commits. Ohne Git: zwei Kopien oder Diffs vergleichen.

## SDK-Konsistenz
In einem realen Team eine vereinbarte SDK-Version über global.json festhalten und dieselbe Version im CI verwenden. Hier keine harte Versionsbindung, damit ein installiertes .NET-8-SDK funktioniert. Grössere SDK-Versionen können andere Diagnoseergebnisse liefern.
