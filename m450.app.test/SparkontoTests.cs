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
}
