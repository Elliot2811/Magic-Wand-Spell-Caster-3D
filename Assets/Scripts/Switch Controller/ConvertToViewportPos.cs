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

        Vector2 posInCamera = new Vector2(
            yawDegrees / GameConstants.HorizontalFovDeg,
            pitchDegrees / GameConstants.VerticalFovDeg
            );

        Vector2 viewport = posInCamera + new Vector2(0.5f, 0.5f);
        
        float left = 0.5f - GameConstants.DrawingAreaPercentage / 2;
        float right = 0.5f + GameConstants.DrawingAreaPercentage / 2;

        viewport.x = Mathf.Clamp(viewport.x, left, right);
        viewport.y = Mathf.Clamp(viewport.y, left, right);

        return new Vector2(
            viewport.x * Screen.width,
            viewport.y * Screen.height
            );
    }
}