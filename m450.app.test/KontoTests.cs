using System.Runtime.InteropServices.JavaScript;
using m450.app;

namespace m450.app.test;

/// <summary>Minimaler konkreter Kontotyp, um die Basisklasse isoliert zu testen.</summary>
internal class Testkonto(decimal aktivZinssatz, decimal passivZinssatz, DateOnly? eroeffnetAm = null)
    : Konto(aktivZinssatz, passivZinssatz, KontoStatus.Standard, eroeffnetAm)
{
    public override decimal Ueberzugslimite => decimal.MaxValue;
}

public class KontoTests
{
    [Fact]
    public void ZahleEin_IncreasesBalance()
    {
        // Arrange
        var konto = new Testkonto(3.6m, 7.2m);

        // Act
        konto.ZahleEin(1000m);

        // Assert
        Assert.Equal(1000m, konto.Kontostand);
    }

    [Fact]
    public void Beziehe_ReducesBalance()
    {
        // Arrange
        var konto = new Testkonto(3.6m, 7.2m);
        konto.ZahleEin(1500m);

        // Act
        konto.Beziehe(300m);

        // Assert
        Assert.Equal(1200m, konto.Kontostand);
    }

    [Fact]
    public void ZahleEin_WithNegativeAmount_Throws()
    {
        // Arrange
        var konto = new Testkonto(3.6m, 7.2m);

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => konto.ZahleEin(-1m));
    }

    [Fact]
    public void SchreibeZinsenFuerTage_AddsInterestForPositiveBalance()
    {
        // Arrange
        var konto = new Testkonto(3.6m, 7.2m);
        konto.ZahleEin(1800m);

        // Act
        konto.SchreibeZinsenFuerTage(30);

        // Assert
        Assert.Equal(5.4m, konto.AufgelaufeneZinsen);
    }

    [Fact]
    public void SchreibeZinsenFuerTage_KeepsInterestSeparateFromBalance()
    {
        // Arrange (R15)
        var konto = new Testkonto(3.6m, 7.2m);
        konto.ZahleEin(1800m);

        // Act
        konto.SchreibeZinsenFuerTage(30);

        // Assert
        Assert.Equal(1800m, konto.Kontostand);
        Assert.Equal(5.4m, konto.AufgelaufeneZinsen);
    }

    [Fact]
    public void SchliesseKontoAb_BooksAccruedInterestToBalance()
    {
        // Arrange
        var konto = new Testkonto(3.6m, 7.2m);
        konto.ZahleEin(7200m);
        konto.SchreibeZinsenFuerTage(60);

        // Act
        konto.SchliesseKontoAb();

        // Assert
        Assert.Equal(7200m + 43.2m, konto.Kontostand);
        Assert.Equal(0m, konto.AufgelaufeneZinsen);
    }

    [Fact]
    public void Transferiere_MovesAmountToTargetAccount()
    {
        // Arrange
        var quelle = new Testkonto(3.6m, 7.2m);
        var ziel = new Testkonto(3.6m, 7.2m);
        quelle.ZahleEin(500m);

        // Act
        quelle.Transferiere(ziel, 200m);

        // Assert
        Assert.Equal(300m, quelle.Kontostand);
        Assert.Equal(200m, ziel.Kontostand);
    }

    [Fact]
    public void ValidationScenario_AccruesInterestAcrossDifferentBalancePhases()
    {
        // Arrange
        var eroeffnung = new DateOnly(2026, 3, 1);
        var konto = new Testkonto(3.6m, 7.2m, eroeffnung);

        var juli = new DateOnly(2026, 7, 1);
        var august = new DateOnly(2026, 8, 1);
        var oktober = new DateOnly(2026, 10, 1);
        var abschluss = new DateOnly(2026, 12, 31);

        // Act
        konto.ZahleEin(1000m, eroeffnung);
        konto.ZahleEin(1000m, juli);
        konto.Beziehe(3000m, august);
        konto.ZahleEin(2000m, oktober);
        konto.SchliesseKontoAb(abschluss);

        // Assert
        Assert.Equal(1015.3m, konto.Kontostand);
    }

    [Fact]
    public void Beziehe_FullBalance_ReducesBalanceToZero()
    {
        // Arrange
        var konto = new Testkonto(3.3m, 4.5m);
        konto.ZahleEin(7200m);

        //Act
        konto.Beziehe(7200m);

        //Assert
        Assert.Equal(0m, konto.Kontostand);
    }

    [Fact]
    public void Constructor_WithCustomCreationDate_SetsEroeffnetAm()
    {
        var fourDaysAgo = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-4));
        
        var konto = new Testkonto(3.4m, 4.5m, fourDaysAgo);
        
        Assert.Equal(fourDaysAgo, konto.EroeffnetAm);
    }
}
