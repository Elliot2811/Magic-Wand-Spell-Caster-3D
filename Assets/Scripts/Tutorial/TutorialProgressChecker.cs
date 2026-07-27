using System;

public static class TutorialProgressTracker
{
    private static bool player1Done = false;
    private static bool player2Done = false;

    public static event Action OnBothPlayersDone;

    public static void ReportDone(int playerIndex)
    {
        if (playerIndex == 0) player1Done = true;
        else if (playerIndex == 1) player2Done = true;

        if (player1Done && player2Done)
            OnBothPlayersDone?.Invoke();
    }

    public static bool BothDone => player1Done && player2Done;

    // Call this when the tutorial restarts/reloads, so state doesn't leak into next playthrough
    public static void Reset()
    {
        player1Done = false;
        player2Done = false;
    }
}