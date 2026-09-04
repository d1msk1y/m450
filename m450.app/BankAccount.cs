namespace m450.app;

public class BankAccount
{
    private decimal _kontostand;
    private decimal _aufgelaufeneZinsen;
    private DateOnly _letzterZinsStichtag;

    public BankAccount(decimal aktivZinssatz, decimal passivZinssatz, DateOnly? kontoErstelltAm = null)
    {
        AktivZinssatz = aktivZinssatz;
        PassivZinssatz = passivZinssatz;
        _letzterZinsStichtag = kontoErstelltAm ?? DateOnly.FromDateTime(DateTime.Today);
    }

    public decimal AktivZinssatz { get; }
    public decimal PassivZinssatz { get; }
    public decimal Kontostand => _kontostand;
    public decimal AufgelaufeneZinsen => _aufgelaufeneZinsen;

    public void ZahleEin(decimal betrag, DateOnly? datum = null)
    {
        if (betrag <= 0m)
        {
            throw new ArgumentOutOfRangeException(nameof(betrag), "Ein Betrag muss positiv sein.");
        }

        if (datum.HasValue)
        {
            SchreibeZinsenFuerZeitraum(_letzterZinsStichtag, datum.Value);
        }

        _kontostand += betrag;

        if (datum.HasValue)
        {
            _letzterZinsStichtag = datum.Value;
        }
    }

    public void Beziehe(decimal betrag, DateOnly? datum = null)
    {
        if (betrag <= 0m)
        {
            throw new ArgumentOutOfRangeException(nameof(betrag), "Ein Betrag muss positiv sein.");
        }

        if (datum.HasValue)
        {
            SchreibeZinsenFuerZeitraum(_letzterZinsStichtag, datum.Value);
        }

        _kontostand -= betrag;

        if (datum.HasValue)
        {
            _letzterZinsStichtag = datum.Value;
        }
    }

    public void Transferiere(BankAccount zielKonto, decimal betrag)
    {
        if (zielKonto is null)
        {
            throw new ArgumentNullException(nameof(zielKonto));
        }

        Beziehe(betrag);
        zielKonto.ZahleEin(betrag);
    }

    public void SchreibeZinsenFuerTage(int tage)
    {
        if (tage <= 0)
        {
            return;
        }

        if (_kontostand > 0m)
        {
            _aufgelaufeneZinsen += (_kontostand * AktivZinssatz / 100m / 360m) * tage;
            return;
        }

        if (_kontostand < 0m)
        {
            _aufgelaufeneZinsen -= (Math.Abs(_kontostand) * PassivZinssatz / 100m / 360m) * tage;
        }
    }

    public void SchreibeZinsAufTage(int tage) => SchreibeZinsenFuerTage(tage);

    public void SchreibeZinsenFuerZeitraum(DateOnly von, DateOnly bis)
    {
        if (bis <= von)
        {
            return;
        }

        var tage = bis.DayNumber - von.DayNumber;
        SchreibeZinsenFuerTage(tage);
        _letzterZinsStichtag = bis;
    }

    public void SchliesseKontoAb(DateOnly? datum = null)
    {
        if (datum.HasValue)
        {
            SchreibeZinsenFuerZeitraum(_letzterZinsStichtag, datum.Value);
        }

        _kontostand += _aufgelaufeneZinsen;
        _aufgelaufeneZinsen = 0m;

        if (datum.HasValue)
        {
            _letzterZinsStichtag = datum.Value;
        }
    }
}
