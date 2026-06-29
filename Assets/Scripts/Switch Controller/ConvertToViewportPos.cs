using UnityEngine;

public static class ConvertToViewportPos
{
    public static Vector2 Caculate(int deviceIndex)
    {
        if (JoyConTracker.Instance == null || JoyConTracker.Instance.readyToConnect == false)
            return Vector2.zero;

        Vector3 forward = JoyConTracker.Instance.CurrentRotation(deviceIndex) * Vector3.forward;

        float yawDegrees = Mathf.Atan2(forward.x, forward.z) * Mathf.Rad2Deg;
        float pitchDegrees = Mathf.Asin(forward.y) * Mathf.Rad2Deg;

        Vector2 viewport = new Vector2(
            yawDegrees / GameConstants.HorizontalFovDeg + 0.5f,
            pitchDegrees / GameConstants.VerticalFovDeg + 0.5f
            );


        Rect bounds = deviceIndex == 0 ? GameConstants.DrawingRectLeft : GameConstants.DrawingRectRight;

        Vector2 rectCentre = bounds.center;

        viewport.x = Mathf.Clamp(viewport.x - 0.5f + rectCentre.x, bounds.xMin, bounds.xMax);
        viewport.y = Mathf.Clamp(viewport.y - 0.5f + rectCentre.y, bounds.yMin, bounds.yMax);

        return new Vector2(
            viewport.x * Screen.width,
            viewport.y * Screen.height
            );
    }
}