# Test Doubles – Antworten

Ich habe xUnit verwendet. Tests ausführen:

```bash
dotnet test TestDoublesExercise/TestDoublesExercise.test/TestDoublesExercise.test.csproj
```

Meine Doubles liegen im Ordner `TestDoubles/`, die Tests in `ReservationServiceTests.cs`.

## Aufgabe 1

Der Service bekommt `ISeatAvailability` im Konstruktor übergeben. Deshalb kann ich im Test einfach meinen eigenen Stub übergeben. Die echte Implementierung brauche ich dafür nicht.

Die Schnittstelle sagt nur, dass `GetAvailableSeats` einen Veranstaltungscode bekommt und eine Zahl zurückgibt. Ich nehme an, dass diese Zahl die freien Plätze sind, nie negativ ist und bei einem unbekannten Code 0 ist.

## Aufgabe 3

Der Stub gibt dem Service Daten (wie viele Plätze frei sind), der Spy merkt sich, was der Service aufgerufen hat (welche Bestätigungen verschickt wurden).

## Aufgabe 5

- **Verfügbarkeit: eigener Stub.** Den hatte ich schon, und er merkt sich die Aufrufe.
- **Repository: Moq.** Mit `Times.Never()` kann ich direkt prüfen, dass nichts gespeichert wurde.
- **Sender: eigener Spy.** Den benutze ich auch in den anderen Tests, und `Assert.Empty` ist gut lesbar.

Eigene Doubles sind praktisch, wenn man sie oft wiederverwendet. Moq ist praktisch, wenn man in einem Test nur schnell einen Aufruf prüfen will.
