using System;

//<summary>
//Common contract for mid-match panel events (tug of war, and the 2 future ones).
//GamePlayState triggers one of these at the halfway mark, pauses the main timer,
//and resumes once OnEventCompleted fires.
//</summary>
public interface IMidGameEvent
{
    bool IsActive { get; }

    bool PausesMainTimer { get; }   //will pause the game timer or not based of event: tug-of-war = true, circle event = false

    //<summary>Short title shown on the pre-event banner (e.g. "TUG OF WAR!").</summary>
    string EventTitle { get; }
    //<summary>Short instructional text shown on the pre-event instruction panel.</summary>
    string EventInstructions { get; }

    //<summary>Begin the event (activate its panel, start listening for input, etc).</summary>
    void StartEvent();

    //<summary>
    //Fired exactly once when the event resolves.
    //Payload: 0 = draw/timeout, 1 = left player won it, 2 = right player won it.
    //</summary>
    event Action<int> OnEventCompleted;
}
