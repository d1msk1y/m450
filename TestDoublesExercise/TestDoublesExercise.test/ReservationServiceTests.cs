using Moq;
using TestDoublesExercise.Tests.TestDoubles;

namespace TestDoublesExercise.Tests;

// Antworten zu den Aufgaben 1, 3 und 5: siehe ANTWORTEN.md
public sealed class ReservationServiceTests
{
    private const string Email = "anna@example.com";
    private const string EventCode = "KONZERT-2026";

    // Aufgabe 1

    [Fact]
    public void Reserve_WithEnoughSeats_ReturnsTrue()
    {
        // Arrange
        var availability = new SeatAvailabilityStub().WithSeats(EventCode, 3);
        var repository = new ReservationRepositorySpy(saveSucceeds: true);
        var sender = new ConfirmationSenderSpy();
        var service = new ReservationService(availability, repository, sender);

        // Act
        var result = service.Reserve(Email, EventCode, 2);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Reserve_WithTooFewSeats_ReturnsFalse()
    {
        // Arrange
        var availability = new SeatAvailabilityStub().WithSeats(EventCode, 1);
        var service = new ReservationService(availability, new ReservationRepositorySpy(), new ConfirmationSenderSpy());

        // Act
        var result = service.Reserve(Email, EventCode, 2);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Reserve_WithExactlyEnoughSeats_Succeeds()
    {
        // Arrange
        var availability = new SeatAvailabilityStub().WithSeats(EventCode, 2);
        var service = new ReservationService(availability, new ReservationRepositorySpy(), new ConfirmationSenderSpy());

        // Act
        var result = service.Reserve(Email, EventCode, 2);

        // Assert
        Assert.True(result);
    }

    // Aufgabe 2: leere Listen = keine Abhängigkeit wurde aufgerufen

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Reserve_WithNonPositiveSeatCount_ThrowsBeforeCallingDependencies(int seatCount)
    {
        // Arrange
        var availability = new SeatAvailabilityStub(defaultSeats: 10);
        var repository = new ReservationRepositorySpy();
        var sender = new ConfirmationSenderSpy();
        var service = new ReservationService(availability, repository, sender);

        // Act + Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => service.Reserve(Email, EventCode, seatCount));

        Assert.Empty(availability.RequestedEventCodes);
        Assert.Empty(repository.SavedReservations);
        Assert.Empty(sender.SentReservations);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Reserve_WithEmptyEventCode_ThrowsBeforeCallingDependencies(string eventCode)
    {
        // Arrange
        var availability = new SeatAvailabilityStub(defaultSeats: 10);
        var repository = new ReservationRepositorySpy();
        var sender = new ConfirmationSenderSpy();
        var service = new ReservationService(availability, repository, sender);

        // Act + Assert
        Assert.Throws<ArgumentException>(() => service.Reserve(Email, eventCode, 2));

        Assert.Empty(availability.RequestedEventCodes);
        Assert.Empty(repository.SavedReservations);
        Assert.Empty(sender.SentReservations);
    }

    // Aufgabe 3

    [Fact]
    public void Reserve_WhenSaved_SendsExactlyOneConfirmationWithReservationData()
    {
        // Arrange
        var availability = new SeatAvailabilityStub().WithSeats(EventCode, 5);
        var sender = new ConfirmationSenderSpy();
        var service = new ReservationService(availability, new ReservationRepositorySpy(true), sender);

        // Act
        var result = service.Reserve(Email, EventCode, 2);

        // Assert
        Assert.True(result);
        var sent = Assert.Single(sender.SentReservations);
        Assert.Equal(Email, sent.CustomerEmail);
        Assert.Equal(EventCode, sent.EventCode);
        Assert.Equal(2, sent.SeatCount);
    }

    [Fact]
    public void Reserve_WithTooFewSeats_DoesNotSaveOrSend()
    {
        // Arrange
        var availability = new SeatAvailabilityStub().WithSeats(EventCode, 1);
        var repository = new ReservationRepositorySpy();
        var sender = new ConfirmationSenderSpy();
        var service = new ReservationService(availability, repository, sender);

        // Act
        var result = service.Reserve(Email, EventCode, 2);

        // Assert
        Assert.False(result);
        Assert.Empty(repository.SavedReservations);
        Assert.Empty(sender.SentReservations);
    }

    [Fact]
    public void Reserve_WhenSavingFails_DoesNotSendConfirmation()
    {
        // Arrange
        var availability = new SeatAvailabilityStub().WithSeats(EventCode, 5);
        var repository = new ReservationRepositorySpy(saveSucceeds: false);
        var sender = new ConfirmationSenderSpy();
        var service = new ReservationService(availability, repository, sender);

        // Act
        var result = service.Reserve(Email, EventCode, 2);

        // Assert
        Assert.False(result);
        Assert.Single(repository.SavedReservations);
        Assert.Empty(sender.SentReservations);
    }

    // Aufgabe 4

    [Fact]
    public void Reserve_WithEnoughSeats_SavesCorrectReservationOnce_Moq()
    {
        // Arrange
        var repository = new Mock<IReservationRepository>();
        repository.Setup(r => r.TrySave(It.IsAny<Reservation>())).Returns(true);
        var sender = new ConfirmationSenderSpy();
        var service = new ReservationService(
            new SeatAvailabilityStub().WithSeats(EventCode, 3), repository.Object, sender);

        // Act
        var result = service.Reserve(Email, EventCode, 2);

        // Assert
        Assert.True(result);
        repository.Verify(r => r.TrySave(It.Is<Reservation>(res =>
                res.CustomerEmail == Email &&
                res.EventCode == EventCode &&
                res.SeatCount == 2)),
            Times.Once());
        Assert.Single(sender.SentReservations);
    }

    [Fact]
    public void Reserve_WhenMockedSaveFails_ReturnsFalseAndSendsNothing_Moq()
    {
        // Arrange
        var repository = new Mock<IReservationRepository>();
        repository.Setup(r => r.TrySave(It.IsAny<Reservation>())).Returns(false);
        var sender = new ConfirmationSenderSpy();
        var service = new ReservationService(
            new SeatAvailabilityStub().WithSeats(EventCode, 3), repository.Object, sender);

        // Act
        var result = service.Reserve(Email, EventCode, 2);

        // Assert
        Assert.False(result);
        repository.Verify(r => r.TrySave(It.Is<Reservation>(res =>
                res.CustomerEmail == Email &&
                res.EventCode == EventCode &&
                res.SeatCount == 2)),
            Times.Once());
        Assert.Empty(sender.SentReservations);
    }

    [Fact]
    public void Reserve_WithTooFewSeats_DoesNotCallRepository()
    {
        // Arrange
        var repository = new Mock<IReservationRepository>();
        var sender = new ConfirmationSenderSpy();
        var service = new ReservationService(
            new SeatAvailabilityStub().WithSeats(EventCode, 1), repository.Object, sender);

        // Act
        var result = service.Reserve(Email, EventCode, 2);

        // Assert
        Assert.False(result);
        repository.Verify(r => r.TrySave(It.IsAny<Reservation>()), Times.Never());
        Assert.Empty(sender.SentReservations);
    }

    // Aufgabe 5: Stub, Moq und Spy gemischt (Begründung in ANTWORTEN.md)

    [Fact]
    public void Reserve_WithWhitespaceEmail_ThrowsBeforeCallingDependencies()
    {
        // Arrange
        var availability = new SeatAvailabilityStub(defaultSeats: 10);
        var repository = new Mock<IReservationRepository>();
        var sender = new ConfirmationSenderSpy();
        var service = new ReservationService(availability, repository.Object, sender);

        // Act + Assert
        Assert.Throws<ArgumentException>(() => service.Reserve("   ", EventCode, 2));

        Assert.Empty(availability.RequestedEventCodes);
        repository.Verify(r => r.TrySave(It.IsAny<Reservation>()), Times.Never());
        Assert.Empty(sender.SentReservations);
    }
}
