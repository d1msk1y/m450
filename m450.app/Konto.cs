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

    protected Konto(
        decimal aktivZinssatz,
        decimal passivZinssatz,
        KontoStatus status = KontoStatus.Standard,
        DateOnly? kontoErstelltAm = null)
    {
        AktivZinssatz = aktivZinssatz;
        PassivZinssatz = passivZinssatz;
        Status = status;
        EroeffnetAm = kontoErstelltAm ?? DateOnly.FromDateTime(DateTime.Today);
        _letzterZinsStichtag = EroeffnetAm;
    }

    public KontoStatus Status { get; }

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
            var gutschrift = _kontostand * SchreibeZinsGut(_kontostand) / 100m / 360m * tage;
            _aufgelaufeneZinsen += Math.Round(gutschrift, 2, MidpointRounding.AwayFromZero);
            return;
        }

        if (_kontostand < 0m)
        {
            var belastung = Math.Abs(_kontostand) * PassivZinssatz / 100m / 360m * tage;
            _aufgelaufeneZinsen -= Math.Round(belastung, 2, MidpointRounding.AwayFromZero);
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
        var bezugslimite = HoleBezugslimiteProBezug(datum);

        if (betrag > bezugslimite)
        {
            throw new InvalidOperationException(
                $"Der Bezug von {betrag} überschreitet die Bezugslimite von {bezugslimite}.");
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

    /// <summary>
    /// Bestimmt den Zinssatz für ein Guthaben gemäss Status und Guthabensklasse.
    /// </summary>
    protected virtual decimal SchreibeZinsGut(decimal guthaben)
    {
        if (guthaben < 10_000m)
        {
            return AktivZinssatz;
        }

        if (guthaben < 50_000m)
        {
            return AktivZinssatz + 0.5m;
        }

        var zuschlag = Status == KontoStatus.VIP ? 1.5m : 0.75m;
        return AktivZinssatz + zuschlag;
    }

    /// <summary>
    /// Liefert die Bezugslimite für ein bestimmtes Datum; Standardkonten nutzen die fixe Limite.
    /// </summary>
    protected virtual decimal HoleBezugslimiteProBezug(DateOnly datum) => BezugslimiteProBezug;

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
