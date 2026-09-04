using m450.app;

namespace m450.app.test;

public class BankAccountTests
{
    [Fact]
    public void ZahleEin_IncreasesBalance()
    {
        // Arrange
        var account = new BankAccount(3.6m, 7.2m);

        // Act
        account.ZahleEin(1000m);

        // Assert
        Assert.Equal(1000m, account.Kontostand);
    }

    [Fact]
    public void Beziehe_ReducesBalance()
    {
        // Arrange
        var account = new BankAccount(3.6m, 7.2m);
        account.ZahleEin(1500m);

        // Act
        account.Beziehe(300m);

        // Assert
        Assert.Equal(1200m, account.Kontostand);
    }

    [Fact]
    public void SchreibeZinsenFuerTage_AddsInterestForPositiveBalance()
    {
        // Arrange
        var account = new BankAccount(3.6m, 7.2m);
        account.ZahleEin(1800m);

        // Act
        account.SchreibeZinsenFuerTage(30);

        // Assert
        Assert.Equal(5.4m, account.AufgelaufeneZinsen);
    }

    [Fact]
    public void SchliesseKontoAb_BooksAccruedInterestToBalance()
    {
        // Arrange
        var account = new BankAccount(3.6m, 7.2m);
        account.ZahleEin(7200m);
        account.SchreibeZinsenFuerTage(60);

        // Act
        account.SchliesseKontoAb();

        // Assert
        Assert.Equal(7200m + 43.2m, account.Kontostand);
        Assert.Equal(0m, account.AufgelaufeneZinsen);
    }

    [Fact]
    public void ValidationScenario_AccruesInterestAcrossDifferentBalancePhases()
    {
        // Arrange
        var openingDate = new DateOnly(2026, 3, 1);
        var account = new BankAccount(3.6m, 7.2m, openingDate);

        var julyDate = new DateOnly(2026, 7, 1);
        var augustDate = new DateOnly(2026, 8, 1);
        var octoberDate = new DateOnly(2026, 10, 1);
        var closingDate = new DateOnly(2026, 12, 31);

        // Act
        account.ZahleEin(1000m, openingDate);
        account.ZahleEin(1000m, julyDate);
        account.Beziehe(3000m, augustDate);
        account.ZahleEin(2000m, octoberDate);
        account.SchliesseKontoAb(closingDate);

        // Assert
        Assert.Equal(1015.3m, account.Kontostand);
    }
}
