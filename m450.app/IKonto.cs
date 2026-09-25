namespace m450.app;

public interface IKonto
{
    KontoStatus Status { get; }

    decimal AktivZinssatz { get; }

    decimal PassivZinssatz { get; }

    decimal Kontostand { get; }

    decimal AufgelaufeneZinsen { get; }

    decimal Ueberzugslimite { get; }

    decimal BezugslimiteProBezug { get; }

    decimal Jahresgebuehr { get; }

    void ZahleEin(decimal betrag, DateOnly? datum = null);

    void Beziehe(decimal betrag, DateOnly? datum = null);

    void Transferiere(IKonto zielKonto, decimal betrag, DateOnly? datum = null);

    void SchliesseKontoAb(DateOnly? datum = null);
}
