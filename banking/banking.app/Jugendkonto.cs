namespace m450.app;

/// <summary>
/// Jugendkonto: nur für Jugendliche unter 20 (R6), kein Überzug (R7),
/// Vorzugszins (R8), gebührenfrei (R10) und mit Bezugslimite pro Bezug (R13).
/// </summary>
public class Jugendkonto : Konto
{
    public const decimal StandardAktivZinssatz = 2.0m;
    public const decimal StandardPassivZinssatz = 7.2m;
    public const decimal StandardBezugslimiteProBezug = 500m;
    public const int MaximalAlter = 20;

    private readonly decimal _bezugslimiteProBezug;

    public Jugendkonto(
        DateOnly geburtsdatum,
        DateOnly? kontoErstelltAm = null,
        decimal aktivZinssatz = StandardAktivZinssatz,
        decimal passivZinssatz = StandardPassivZinssatz,
        decimal bezugslimiteProBezug = StandardBezugslimiteProBezug,
        KontoStatus status = KontoStatus.Standard)
        : base(aktivZinssatz, passivZinssatz, status, kontoErstelltAm)
    {
        Geburtsdatum = geburtsdatum;
        _bezugslimiteProBezug = bezugslimiteProBezug;

        if (BerechneAlter(geburtsdatum, EroeffnetAm) >= MaximalAlter)
        {
            throw new InvalidOperationException(
                $"Ein Jugendkonto kann nur von Jugendlichen unter {MaximalAlter} Jahren eröffnet werden.");
        }
    }

    public DateOnly Geburtsdatum { get; }

    public override decimal BezugslimiteProBezug => _bezugslimiteProBezug;

    public static int BerechneAlter(DateOnly geburtsdatum, DateOnly stichtag)
    {
        var alter = stichtag.Year - geburtsdatum.Year;

        if (geburtsdatum.AddYears(alter) > stichtag)
        {
            alter--;
        }

        return alter;
    }

}
