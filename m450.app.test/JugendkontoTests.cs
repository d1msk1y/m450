using m450.app;

namespace m450.app.test;

public class JugendkontoTests
{
    private static readonly DateOnly Eroeffnung = new(2026, 1, 1);

    [Fact]
    public void Eroeffnen_ForPersonUnder20_IsAllowed()
    {
        // Arrange (R6)
        var geburtsdatum = new DateOnly(2010, 5, 20);

        // Act
        var konto = new Jugendkonto(geburtsdatum, Eroeffnung);

        // Assert
        Assert.Equal(0m, konto.Kontostand);
    }

    [Fact]
    public void Eroeffnen_ForPersonOf20OrOlder_Throws()
    {
        // Arrange (R6)
        var geburtsdatum = new DateOnly(2006, 1, 1);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => new Jugendkonto(geburtsdatum, Eroeffnung));
    }

    [Fact]
    public void Beziehe_MoreThanBalance_Throws()
    {
        // Arrange (R7)
        var konto = new Jugendkonto(new DateOnly(2010, 5, 20), Eroeffnung);
        konto.ZahleEin(100m, Eroeffnung);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => konto.Beziehe(101m, Eroeffnung));
    }

    [Fact]
    public void Jugendkonto_HasPreferredInterestRate()
    {
        // Arrange (R8)
        var jugendkonto = new Jugendkonto(new DateOnly(2010, 5, 20), Eroeffnung);
        var sparkonto = new Sparkonto(Eroeffnung);
        var privatkonto = new Privatkonto(Eroeffnung);

        // Act
        var jugendZins = jugendkonto.AktivZinssatz;

        // Assert
        Assert.True(jugendZins > sparkonto.AktivZinssatz);
        Assert.True(jugendZins > privatkonto.AktivZinssatz);
    }

    [Fact]
    public void SchliesseKontoAb_ChargesNoFee()
    {
        // Arrange (R10)
        var konto = new Jugendkonto(new DateOnly(2010, 5, 20), Eroeffnung);
        konto.ZahleEin(1000m, Eroeffnung);

        // Act
        konto.SchliesseKontoAb(Eroeffnung);

        // Assert
        Assert.Equal(0m, konto.Jahresgebuehr);
        Assert.Equal(1000m, konto.Kontostand);
    }

    [Fact]
    public void Beziehe_AboveWithdrawalLimit_Throws()
    {
        // Arrange (R13)
        var konto = new Jugendkonto(new DateOnly(2010, 5, 20), Eroeffnung, bezugslimiteProBezug: 500m);
        konto.ZahleEin(2000m, Eroeffnung);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => konto.Beziehe(501m, Eroeffnung));
    }

    [Fact]
    public void Beziehe_UpToWithdrawalLimit_IsAllowed()
    {
        // Arrange (R13)
        var konto = new Jugendkonto(new DateOnly(2010, 5, 20), Eroeffnung, bezugslimiteProBezug: 500m);
        konto.ZahleEin(2000m, Eroeffnung);

        // Act
        konto.Beziehe(500m, Eroeffnung);

        // Assert
        Assert.Equal(1500m, konto.Kontostand);
    }

    [Fact]
    public void Zinssatz_IsIndependentOfCustomer()
    {
        // Arrange (R5)
        var kontoA = new Jugendkonto(new DateOnly(2010, 5, 20), Eroeffnung);
        var kontoB = new Jugendkonto(new DateOnly(2012, 9, 3), Eroeffnung);

        // Act & Assert
        Assert.Equal(kontoA.AktivZinssatz, kontoB.AktivZinssatz);
        Assert.Equal(kontoA.PassivZinssatz, kontoB.PassivZinssatz);
    }
}
