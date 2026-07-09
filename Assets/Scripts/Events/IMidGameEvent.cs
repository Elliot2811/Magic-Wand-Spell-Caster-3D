using System;

/// <summary>
/// Common contract for mid-match panel events (tug of war, and the 2 future ones).
/// GamePlayState triggers one of these at the halfway mark, pauses the main timer,
/// and resumes once OnEventCompleted fires.
/// </summary>
public interface IMidGameEvent
{
    bool IsActive { get; }

    /// <summary>Begin the event (activate its panel, start listening for input, etc).</summary>
    void StartEvent();

    /// <summary>
    /// Fired exactly once when the event resolves.
    /// Payload: 0 = draw/timeout, 1 = left player won it, 2 = right player won it.
    /// </summary>
    event Action<int> OnEventCompleted;
}
