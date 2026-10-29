namespace TestDoublesExercise.Tests.TestDoubles;

// Stub: gibt für jeden Veranstaltungscode eine feste Anzahl freier Plätze zurück.
// Merkt sich ausserdem, nach welchen Codes gefragt wurde (für Aufgabe 2).
public sealed class SeatAvailabilityStub : ISeatAvailability
{
    private readonly Dictionary<string, int> _seatsByEventCode = new();
    private readonly int _defaultSeats;
    private readonly List<string> _requestedEventCodes = [];

    public SeatAvailabilityStub(int defaultSeats = 0)
    {
        _defaultSeats = defaultSeats;
    }

    public IReadOnlyList<string> RequestedEventCodes => _requestedEventCodes;

    public SeatAvailabilityStub WithSeats(string eventCode, int availableSeats)
    {
        _seatsByEventCode[eventCode] = availableSeats;
        return this;
    }

    public int GetAvailableSeats(string eventCode)
    {
        _requestedEventCodes.Add(eventCode);
        return _seatsByEventCode.TryGetValue(eventCode, out var seats) ? seats : _defaultSeats;
    }
}
