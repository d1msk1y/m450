# Einrichtung und Werkzeuge für C#
Stand: 29.10.2026. SDK-Übung getestet mit .NET SDK 8.0.408. Neuere SDKs können weitere oder veränderte Meldungen erzeugen.

## Bereits enthalten
Visual Studio bringt Compiler- und IDE-Analyse mit. Die .NET-Analyzer sind im .NET-SDK enthalten. Für SDK-Projekte ab .NET 5 ist Codequalitätsanalyse grundsätzlich aktiviert, jedoch sind nicht alle Regeln aktiv. Code-Stil ist im Editor verfügbar. Unterstützte IDE-Regeln laufen beim Build erst mit EnforceCodeStyleInBuild und passendem Schweregrad. dotnet format gehört seit .NET 6 zum SDK.

## So startet die Klasse
1. Visual Studio Installer öffnen, falls nötig Workload für .NET-Entwicklung und das .NET-8-SDK installieren. Vor der Lektion prüfen: dotnet --list-sdks.
2. Den vollständigen Ordner 04_Teamstandard kopieren, inklusive .editorconfig und Directory.Build.props. Teamstandard.csproj öffnen.
3. dotnet restore und anschliessend dotnet build --no-incremental ausführen. In Visual Studio alternativ Projekt neu erstellen. Fehlerliste auf Build und gesamtes Projekt einstellen.
4. Meldung öffnen, Regel-ID und Fundstelle lesen. Mit Strg+. nach einer passenden Korrektur suchen. Jede automatische Korrektur fachlich prüfen.
5. Format separat prüfen: dotnet format whitespace --verify-no-changes --no-restore. Format anwenden: dotnet format whitespace --no-restore.
6. Nach den Korrekturen: dotnet build --no-incremental -warnaserror. Beide Prüfungen müssen erfolgreich sein.
Menünamen und Umfang der Hintergrundanalyse unterscheiden sich zwischen VS-Versionen. Der Terminal-Build liefert die gemeinsame Referenz.

## Gemeinsame Konfiguration
.editorconfig legt Formatierung und ausgewählte Regeln fest. root=true beendet die Suche nach übergeordneten .editorconfig-Dateien. Untergeordnete Dateien können Einstellungen überschreiben.
Directory.Build.props bündelt MSBuild-Einstellungen für Projekte darunter. Das Beispiel setzt Nullable, EnableNETAnalyzers, EnforceCodeStyleInBuild sowie AnalysisLevel 8.0. AnalysisMode None plus explizite Regeln hält die Übung übersichtlich. Das ist ein Unterrichtsausschnitt, kein vollständiger Produktionsstandard. Für reale Projekte einen geeigneten breiteren Regelsatz prüfen.
global.json hält eine SDK-Version und zulässige Aktualisierungen fest. Im Unterricht gilt die lokal installierte .NET-8-Version. Für ein Team dieselbe konkrete Version vereinbaren und in CI verwenden. Die Datei global.json.example ist eine Vorlage und bindet das Übungsprojekt noch nicht.
Regeländerungen gemeinsam im Pull Request prüfen. Neue Mitglieder erhalten die Konfiguration automatisch beim Klonen. Gleiches Tool und SDK sind zusätzlich nötig, denn verschiedene IDEs unterstützen unterschiedliche Regeln.
CA5351 ist hier error: der Build stoppt. IDE0040 und CA1822 sind warning. Der Abschlussbefehl -warnaserror macht auch verbleibende Warnungen zum Hindernis. Formatter-Prüfungen laufen separat. none deaktiviert eine Regel. Ausnahmen gezielt begründen und überprüfen, statt ganze Kategorien unbesehen abzuschalten.

## Zusätzliche Werkzeuge
- StyleCop.Analyzers: Stilkonventionen. In Visual Studio NuGet-Pakete verwalten, das Paket suchen, eine kompatible Version auswählen und im Projekt festhalten.
- Roslynator.Analyzers: zusätzliche C#-Diagnosen. Ebenfalls als NuGet-Paket referenzieren. Aktuelle Roslynator-IDE-Erweiterungen allein enthalten keine Analyzer.
- SonarQube for IDE, früher SonarLint: über Erweiterungen / Erweiterungen verwalten installieren. SonarQube Server/Cloud und CI können gemeinsame Qualitätsprofile und Auswertungen ergänzen. Umfang der Security-Analyse hängt von Produkt und Konfiguration ab. Eine IDE-Erweiterung allein richtet keinen CI-Check ein.
- ReSharper / Rider: zusätzliche Inspektionen und Refactorings, ReSharper als Visual-Studio-Erweiterung, Rider als eigene IDE. Eigene Prüfungen und EditorConfig-Optionen separat für CI einrichten. dotnet build führt nicht automatisch alle JetBrains-Inspektionen aus.
NuGet-Analyzer im Repository mit fester Version und PrivateAssets=all referenzieren. Anschliessend restore, die gewünschten Regeln festlegen und den Build prüfen. Im Unterricht sind diese Pakete optional, keine Voraussetzung.

## CI als gemeinsamer Prüfpunkt
Nach dem Checkout dasselbe vereinbarte SDK bereitstellen. Im Projektordner nacheinander:
```
dotnet restore
dotnet build --no-restore --no-incremental -warnaserror
dotnet format whitespace --verify-no-changes --no-restore
```
Fehlschlag blockiert die Übernahme nur dann verbindlich, wenn der CI-Status in den Repository-Regeln als erforderlich eingerichtet ist. Ein bestandener Check ersetzt das menschliche Review nicht.

## Security-Einordnung
Statische Sicherheitsanalyse (SAST) kann gefährliche Muster früh erkennen, etwa veraltete Kryptografie oder bestimmte SQL-Injection-Risiken. Nicht jede SDK-Security-Regel ist standardmässig aktiv. Unser Beispiel aktiviert CA5351 ausdrücklich. Ein SHA-256-Hash hilft beim Integritätsvergleich mit einer vertrauenswürdigen Referenz. Ein Angreifer darf die Referenz nicht mitändern können. Für Passwörter sind dafür vorgesehene Passwort-Hashverfahren nötig.
Analyse garantiert weder Wartbarkeit noch Sicherheit. Fachliche Fehler, Architekturprobleme und manche Datenflüsse benötigen weitergehende Prüfung. Security-Reviews, dynamische Tests sowie Abhängigkeits- und Geheimnisprüfungen ergänzen sie.

## Quellen
https://learn.microsoft.com/en-us/dotnet/fundamentals/code-analysis/overview
https://learn.microsoft.com/en-us/dotnet/fundamentals/code-analysis/configuration-files
https://learn.microsoft.com/en-us/visualstudio/code-quality/install-net-analyzers
https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-format
https://learn.microsoft.com/en-us/dotnet/fundamentals/code-analysis/quality-rules/ca5351
https://github.com/DotNetAnalyzers/StyleCopAnalyzers
https://github.com/dotnet/roslynator/blob/main/README.md
https://docs.sonarsource.com/sonarqube-for-visual-studio/getting-started/installation
https://www.jetbrains.com/help/resharper/Using_EditorConfig.html
