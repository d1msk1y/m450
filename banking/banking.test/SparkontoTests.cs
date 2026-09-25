using m450.app;

namespace m450.app.test;

public class SparkontoTests
{
    [Fact]
    public void Sparkonto_HasHigherInterestRateThanPrivatkonto()
    {
        // Arrange (R1)
        var sparkonto = new Sparkonto();
        var privatkonto = new Privatkonto();

        // Act
        var sparZins = sparkonto.AktivZinssatz;

        // Assert
        Assert.True(sparZins > privatkonto.AktivZinssatz);
    }

    [Fact]
    public void Beziehe_MoreThanBalance_Throws()
    {
        // Arrange (R3)
        var konto = new Sparkonto();
        konto.ZahleEin(500m);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => konto.Beziehe(501m));
    }

    [Fact]
    public void Beziehe_UpToBalance_IsAllowed()
    {
        // Arrange (R3)
        var konto = new Sparkonto();
        konto.ZahleEin(500m);

        // Act
        konto.Beziehe(500m);

        // Assert
        Assert.Equal(0m, konto.Kontostand);
    }

    [Fact]
    public void SchliesseKontoAb_ChargesNoFee()
    {
        // Arrange (R10)
        var konto = new Sparkonto();
        konto.ZahleEin(1000m);

        // Act
        konto.SchliesseKontoAb();

        // Assert
        Assert.Equal(0m, konto.Jahresgebuehr);
        Assert.Equal(1000m, konto.Kontostand);
    }

    [Fact]
    public void Zinssatz_IsIndependentOfCustomer()
    {
        // Arrange (R5)
        var kontoA = new Sparkonto();
        var kontoB = new Sparkonto();

        // Act & Assert
        Assert.Equal(kontoA.AktivZinssatz, kontoB.AktivZinssatz);
        Assert.Equal(kontoA.PassivZinssatz, kontoB.PassivZinssatz);
    }

    [Fact]
    public void Aktivzins_UsesBalanceBands_AndIgnoresStatusBelow50000()
    {
        var standard = new Sparkonto();
        var vip = new Sparkonto(status: KontoStatus.VIP);

        standard.ZahleEin(9_999m);
        vip.ZahleEin(9_999m);

        standard.SchreibeZinsenFuerTage(1);
        vip.SchreibeZinsenFuerTage(1);

        Assert.Equal(0.42m, standard.AufgelaufeneZinsen);
        Assert.Equal(0.42m, vip.AufgelaufeneZinsen);
    }

    [Fact]
    public void Aktivzins_UsesStatusAt50000OrAbove()
    {
        var standard = new Sparkonto(status: KontoStatus.Standard);
        var vip = new Sparkonto(status: KontoStatus.VIP);

        standard.ZahleEin(50_000m);
        vip.ZahleEin(50_000m);

        standard.SchreibeZinsenFuerTage(1);
        vip.SchreibeZinsenFuerTage(1);

        Assert.Equal(3.13m, standard.AufgelaufeneZinsen);
        Assert.Equal(4.17m, vip.AufgelaufeneZinsen);
    }
}
