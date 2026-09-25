namespace m450.app;

/// <summary>
/// Privatkonto: tiefer Aktivzins (R1), Überzug bis zu einem Maximalbetrag (R2),
/// fixe Jahresgebühr (R11) und 10 kostenlose Zahlungsaufträge pro Jahr (R12).
/// </summary>
public class Privatkonto : Konto
{
    public const decimal StandardAktivZinssatz = 0.5m;
    public const decimal StandardPassivZinssatz = 7.2m;
    public const decimal StandardUeberzugslimite = 1000m;
    public const decimal StandardJahresgebuehr = 60m;
    public const decimal GebuehrProZahlungsauftrag = 2m;
    public const int KostenfreieAuftraegeProJahr = 10;

    private readonly decimal _ueberzugslimite;
    private readonly decimal _jahresgebuehr;
    private int _auftraegeImJahr;

    public Privatkonto(
        DateOnly? kontoErstelltAm = null,
        decimal aktivZinssatz = StandardAktivZinssatz,
        decimal passivZinssatz = StandardPassivZinssatz,
        decimal ueberzugslimite = StandardUeberzugslimite,
        decimal jahresgebuehr = StandardJahresgebuehr,
        KontoStatus status = KontoStatus.Standard)
        : base(aktivZinssatz, passivZinssatz, status, kontoErstelltAm)
    {
        _ueberzugslimite = ueberzugslimite;
        _jahresgebuehr = jahresgebuehr;
    }

    public override decimal Ueberzugslimite => _ueberzugslimite;

    public override decimal Jahresgebuehr => _jahresgebuehr;

    public int AuftraegeImJahr => _auftraegeImJahr;

    public override void VerbucheAuftragsgebuehr()
    {
        _auftraegeImJahr++;

        if (_auftraegeImJahr > KostenfreieAuftraegeProJahr)
        {
            BelasteGebuehr(GebuehrProZahlungsauftrag);
        }
    }

    protected override void NachKontoabschluss() => _auftraegeImJahr = 0;
}
