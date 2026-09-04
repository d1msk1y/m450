using m450.app;

namespace m450.app.test;

public class PrivatkontoTests
{
    [Fact]
    public void Privatkonto_HasLowerInterestRateThanSparkonto()
    {
        // Arrange (R1)
        var privatkonto = new Privatkonto();
        var sparkonto = new Sparkonto();

        // Act
        var privatZins = privatkonto.AktivZinssatz;
        var sparZins = sparkonto.AktivZinssatz;

        // Assert
        Assert.True(privatZins < sparZins);
    }

    [Fact]
    public void Beziehe_WithinOverdraftLimit_IsAllowed()
    {
        // Arrange (R2)
        var konto = new Privatkonto(ueberzugslimite: 1000m);
        konto.ZahleEin(500m);

        // Act
        konto.Beziehe(1200m);

        // Assert
        Assert.Equal(-700m, konto.Kontostand);
    }

    [Fact]
    public void Beziehe_BeyondOverdraftLimit_Throws()
    {
        // Arrange (R2)
        var konto = new Privatkonto(ueberzugslimite: 1000m);
        konto.ZahleEin(500m);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => konto.Beziehe(1501m));
    }

    [Fact]
    public void SchliesseKontoAb_ChargesYearlyFee()
    {
        // Arrange (R11)
        var konto = new Privatkonto(jahresgebuehr: 60m);
        konto.ZahleEin(1000m);

        // Act
        konto.SchliesseKontoAb();

        // Assert
        Assert.Equal(940m, konto.Kontostand);
    }

    [Fact]
    public void VerbucheAuftragsgebuehr_FirstTenOrdersPerYearAreFree()
    {
        // Arrange (R12)
        var konto = new Privatkonto();
        konto.ZahleEin(1000m);

        // Act
        for (var i = 0; i < Privatkonto.KostenfreieAuftraegeProJahr; i++)
        {
            konto.VerbucheAuftragsgebuehr();
        }

        // Assert
        Assert.Equal(1000m, konto.Kontostand);
    }

    [Fact]
    public void VerbucheAuftragsgebuehr_EleventhOrderCostsFee()
    {
        // Arrange (R12)
        var konto = new Privatkonto();
        konto.ZahleEin(1000m);

        // Act
        for (var i = 0; i < Privatkonto.KostenfreieAuftraegeProJahr + 2; i++)
        {
            konto.VerbucheAuftragsgebuehr();
        }

        // Assert
        Assert.Equal(1000m - 2 * Privatkonto.GebuehrProZahlungsauftrag, konto.Kontostand);
    }

    [Fact]
    public void SchliesseKontoAb_ResetsFreeOrderCounter()
    {
        // Arrange (R12)
        var konto = new Privatkonto(jahresgebuehr: 0m);
        konto.ZahleEin(1000m);
        for (var i = 0; i < Privatkonto.KostenfreieAuftraegeProJahr; i++)
        {
            konto.VerbucheAuftragsgebuehr();
        }

        // Act
        konto.SchliesseKontoAb();
        konto.VerbucheAuftragsgebuehr();

        // Assert
        Assert.Equal(1, konto.AuftraegeImJahr);
        Assert.Equal(1000m, konto.Kontostand);
    }
}
