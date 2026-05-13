using System.Runtime.InteropServices;

public static class JSL
{
    #region Button Masks
    public const int JSMASK_ZL = 0x000400;
    #endregion

    #region Struct Wrappers
    [StructLayout(LayoutKind.Sequential)]
    public struct IMU_STATE
    {
        public float accelX;
        public float accelY;
        public float accelZ;
        public float gyroX;
        public float gyroY;
        public float gyroZ;
    }
    #endregion

    #region Function wrappers
    [DllImport("JoyShockLibrary")]
    public static extern int JslConnectDevices();
    [DllImport("JoyShockLibrary")]
    public static extern int JslGetConnectedDeviceHandles(int[] deviceArray, int size);
    [DllImport("JoyShockLibrary")]
    public static extern void JslDisconnectAndDisposeAll();


    [DllImport("JoyShockLibrary")]
    public static extern IMU_STATE JslGetIMUState(int deviceId);


    [DllImport("JoyShockLibrary")]
    public static extern int JslGetButtons(int deviceId);


    [DllImport("JoyShockLibrary")]
    public static extern void JslSetGyroSpace(int deviceId, int gyroSpace);

    [DllImport("JoyShockLibrary")]
    public static extern void JslResetContinuousCalibration(int deviceId);
    [DllImport("JoyShockLibrary")]
    public static extern void JslStartContinuousCalibration(int deviceId);
    [DllImport("JoyShockLibrary")]
    public static extern void JslPauseContinuousCalibration(int deviceId);
    [DllImport("JoyShockLibrary")]
    public static extern void JslSetAutomaticCalibration(int deviceId, bool enabled);
    #endregion
}