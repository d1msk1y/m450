namespace m450.app;

/// <summary>
/// Sparkonto: höherer Aktivzins als das Privatkonto (R1), kein Überzug (R3)
/// und gebührenfrei (R10).
/// </summary>
public class Sparkonto : Konto
{
    public const decimal StandardAktivZinssatz = 1.5m;
    public const decimal StandardPassivZinssatz = 7.2m;

    public Sparkonto(
        DateOnly? kontoErstelltAm = null,
        decimal aktivZinssatz = StandardAktivZinssatz,
        decimal passivZinssatz = StandardPassivZinssatz,
        KontoStatus status = KontoStatus.Standard)
        : base(aktivZinssatz, passivZinssatz, status, kontoErstelltAm)
    {
    }
}
