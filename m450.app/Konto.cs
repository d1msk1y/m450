namespace m450.app;

/// <summary>
/// Abstrakte Basisklasse aller Kontotypen. Enthält die Grundfunktionalität
/// (Einzahlen, Beziehen, tägliche Zinsberechnung, Kontoabschluss).
/// </summary>
public abstract class Konto : IKonto
{
    private decimal _kontostand;
    private decimal _aufgelaufeneZinsen;
    private DateOnly _letzterZinsStichtag;

    protected Konto(decimal aktivZinssatz, decimal passivZinssatz, DateOnly? kontoErstelltAm = null)
    {
        AktivZinssatz = aktivZinssatz;
        PassivZinssatz = passivZinssatz;
        EroeffnetAm = kontoErstelltAm ?? DateOnly.FromDateTime(DateTime.Today);
        _letzterZinsStichtag = EroeffnetAm;
    }

    public decimal AktivZinssatz { get; }

    public decimal PassivZinssatz { get; }

    public DateOnly EroeffnetAm { get; }

    public decimal Kontostand => _kontostand;

    public decimal AufgelaufeneZinsen => _aufgelaufeneZinsen;

    /// <summary>Maximaler Betrag, um den das Konto überzogen werden darf (R2, R3, R7).</summary>
    public virtual decimal Ueberzugslimite => 0m;

    /// <summary>Maximaler Betrag pro einzelnem Bezug (R13).</summary>
    public virtual decimal BezugslimiteProBezug => decimal.MaxValue;

    /// <summary>Fixe Jahresgebühr, die beim Kontoabschluss verrechnet wird (R10, R11).</summary>
    public virtual decimal Jahresgebuehr => 0m;

    public void ZahleEin(decimal betrag, DateOnly? datum = null)
    {
        PruefeBetrag(betrag);
        BerechneZinsenBis(datum);
        _kontostand += betrag;
    }

    public void Beziehe(decimal betrag, DateOnly? datum = null)
    {
        PruefeBetrag(betrag);

        var bezugsDatum = datum ?? DateOnly.FromDateTime(DateTime.Today);
        PruefeBezug(betrag, bezugsDatum);

        BerechneZinsenBis(datum);
        _kontostand -= betrag;
    }

    public void Transferiere(IKonto zielKonto, decimal betrag, DateOnly? datum = null)
    {
        ArgumentNullException.ThrowIfNull(zielKonto);

        Beziehe(betrag, datum);
        zielKonto.ZahleEin(betrag, datum);
    }

    /// <summary>
    /// Zinsen werden täglich berechnet, aber separat aufsummiert und erst beim
    /// Jahresabschluss dem Konto gutgeschrieben (R15).
    /// </summary>
    public void SchreibeZinsenFuerTage(int tage)
    {
        if (tage <= 0)
        {
            return;
        }

        if (_kontostand > 0m)
        {
            _aufgelaufeneZinsen += _kontostand * AktivZinssatz / 100m / 360m * tage;
            return;
        }

        if (_kontostand < 0m)
        {
            _aufgelaufeneZinsen -= Math.Abs(_kontostand) * PassivZinssatz / 100m / 360m * tage;
        }
    }

    public void SchreibeZinsenFuerZeitraum(DateOnly von, DateOnly bis)
    {
        if (bis <= von)
        {
            return;
        }

        SchreibeZinsenFuerTage(bis.DayNumber - von.DayNumber);
        _letzterZinsStichtag = bis;
    }

    public void SchliesseKontoAb(DateOnly? datum = null)
    {
        BerechneZinsenBis(datum);

        _kontostand += _aufgelaufeneZinsen;
        _aufgelaufeneZinsen = 0m;
        _kontostand -= Jahresgebuehr;

        NachKontoabschluss();
    }

    /// <summary>Belastet dem Konto eine Gebühr (z.B. für Zahlungsaufträge, R12).</summary>
    protected void BelasteGebuehr(decimal gebuehr)
    {
        if (gebuehr > 0m)
        {
            _kontostand -= gebuehr;
        }
    }

    /// <summary>Wird beim Erteilen eines Zahlungsauftrags aufgerufen (R12).</summary>
    public virtual void VerbucheAuftragsgebuehr()
    {
    }

    /// <summary>Erweiterungspunkt für kontospezifische Bezugsregeln.</summary>
    protected virtual void PruefeBezug(decimal betrag, DateOnly datum)
    {
        if (betrag > BezugslimiteProBezug)
        {
            throw new InvalidOperationException(
                $"Der Bezug von {betrag} überschreitet die Bezugslimite von {BezugslimiteProBezug}.");
        }

        if (_kontostand - betrag < -Ueberzugslimite)
        {
            throw new InvalidOperationException(
                $"Der Bezug von {betrag} überschreitet die Überzugslimite von {Ueberzugslimite}.");
        }
    }

    /// <summary>Erweiterungspunkt für Aufräumarbeiten nach dem Jahresabschluss.</summary>
    protected virtual void NachKontoabschluss()
    {
    }

    private void BerechneZinsenBis(DateOnly? datum)
    {
        if (!datum.HasValue)
        {
            return;
        }

        SchreibeZinsenFuerZeitraum(_letzterZinsStichtag, datum.Value);
        _letzterZinsStichtag = datum.Value;
    }

    private static void PruefeBetrag(decimal betrag)
    {
        if (betrag <= 0m)
        {
            throw new ArgumentOutOfRangeException(nameof(betrag), "Ein Betrag muss positiv sein.");
        }
    }
}
