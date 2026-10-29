namespace TestDoublesExercise.Tests.TestDoubles;

// Spy: verschickt nichts, sondern speichert jede Reservation in einer Liste.
public sealed class ConfirmationSenderSpy : IConfirmationSender
{
    private readonly List<Reservation> _sentReservations = [];

    public IReadOnlyList<Reservation> SentReservations => _sentReservations;

    public void Send(Reservation reservation) => _sentReservations.Add(reservation);
}
