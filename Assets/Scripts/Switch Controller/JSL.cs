using System.Runtime.InteropServices;

public static class JSL
{
    #region Button Masks
    public const int JSMASK_L = 0x000100;
    public const int JSMASK_R = 0x000200;
    public const int JSMASK_ZL = 0x000400;
    public const int JSMASK_ZR = 0x000800;

    #endregion

    #region Struct Wrappers
    [StructLayout(LayoutKind.Sequential)]
    public struct JOY_SHOCK_STATE
    {
        public int buttons;
        public float lTrigger;
        public float rTrigger;
        public float stickLX;
        public float stickLY;
        public float stickRX;
        public float stickRY;
    }

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
    #region Device Connection
    [DllImport("JoyShockLibrary")]
    public static extern int JslConnectDevices();
    [DllImport("JoyShockLibrary")]
    public static extern int JslGetConnectedDeviceHandles(int[] deviceArray, int size);
    [DllImport("JoyShockLibrary")]
    public static extern void JslDisconnectAndDisposeAll();
    #endregion

    #region Data collection
    [DllImport("JoyShockLibrary")]
    public static extern void JslSetCallback(CallBackDelegate callback);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void CallBackDelegate(int handle, JOY_SHOCK_STATE currState, JOY_SHOCK_STATE prevState, IMU_STATE currImu, IMU_STATE prevImu, float deltaTime);

    [DllImport("JoyShockLibrary")]
    public static extern IMU_STATE JslGetIMUState(int deviceId);


    [DllImport("JoyShockLibrary")]
    public static extern int JslGetButtons(int deviceId);
    #endregion

    #region Gyro
    [DllImport("JoyShockLibrary")]
    public static extern void JslSetGyroSpace(int deviceId, int gyroSpace);


    [DllImport("JoyShockLibrary")]
    public static extern float JslGetPollRate(int deviceId);

    #endregion

    #region Calibration
    [DllImport("JoyShockLibrary")]
    public static extern void JslResetContinuousCalibration(int deviceId);
    [DllImport("JoyShockLibrary")]
    public static extern void JslStartContinuousCalibration(int deviceId);
    [DllImport("JoyShockLibrary")]
    public static extern void JslPauseContinuousCalibration(int deviceId);
    [DllImport("JoyShockLibrary")]
    public static extern void JslSetAutomaticCalibration(int deviceId, bool enabled);
    #endregion
    #endregion
}