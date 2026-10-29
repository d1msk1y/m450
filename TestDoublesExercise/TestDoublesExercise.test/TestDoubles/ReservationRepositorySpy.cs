namespace TestDoublesExercise.Tests.TestDoubles;

// Repository ohne Datenbank: TrySave gibt immer das gleiche Ergebnis zurück
// und merkt sich, was gespeichert werden sollte.
public sealed class ReservationRepositorySpy(bool saveSucceeds = true) : IReservationRepository
{
    private readonly List<Reservation> _savedReservations = [];

    public IReadOnlyList<Reservation> SavedReservations => _savedReservations;

    public bool TrySave(Reservation reservation)
    {
        _savedReservations.Add(reservation);
        return saveSucceeds;
    }
}
