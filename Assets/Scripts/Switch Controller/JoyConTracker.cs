using System;
using System.Collections;
using System.Collections.Concurrent;
using UnityEngine;

public class JoyConTracker : MonoBehaviour
{
    private struct ImuData
    {
        public int index;
        public JSL.IMU_STATE currImu;
        public JSL.IMU_STATE prevImu;
        public JSL.JOY_SHOCK_STATE currState;
        public JSL.JOY_SHOCK_STATE prevState;
        public float deltaTime;
    }

    public static JoyConTracker Instance { get; private set; }

    public GameObject gyroRepresentationLeft;
    public GameObject gyroRepresentationRight;

    private int count = 0;
    private int[] handles = new int[16];


    private Quaternion[] currentRotation = new Quaternion[16];
    public Quaternion CurrentRotation(int deviceIndex) => currentRotation[deviceIndex];

    public bool readyToConnect { get; private set; } = false;

    private bool cleaned = false;

    private readonly ConcurrentQueue<ImuData> messageQueue = new ConcurrentQueue<ImuData>();
    private JSL.CallBackDelegate callbackDelegate;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(Instance);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        JSL.JslDisconnectAndDisposeAll();
        StartCoroutine(ConnectAfterDelay());
    }

    private void Update()
    {
        if (count <= 0)
            return;

        // Process all pending data
        while (messageQueue.TryDequeue(out ImuData data))
            ProcessData(data);

        if (gyroRepresentationLeft != null)
            gyroRepresentationLeft.transform.rotation = currentRotation[0];
        if (gyroRepresentationRight != null)
            gyroRepresentationRight.transform.rotation = currentRotation[1];
    }

    private void ProcessData(ImuData data)
    {
        bool ResetButPressed = (data.currState.buttons & (JSL.JSMASK_L | JSL.JSMASK_R)) != 0;
        bool ResetButWasPressed = (data.prevState.buttons & (JSL.JSMASK_L | JSL.JSMASK_R)) != 0;

        if (ResetButPressed && !ResetButWasPressed)
            ResetAllRotation(data.index);

        currentRotation[data.index] = GyroRotation(data.index, data.currImu, data.deltaTime);
        currentRotation[data.index] = ApplyAccelCorrection(currentRotation[data.index], data.currImu, data.deltaTime);

        bool DrawButPressed = (data.currState.buttons & (JSL.JSMASK_ZL | JSL.JSMASK_ZR)) != 0;
        bool DrawButWasPressed = (data.prevState.buttons & (JSL.JSMASK_ZL | JSL.JSMASK_ZR)) != 0;

        if (DrawButPressed && !DrawButWasPressed)
        {
            drawingButtonPressed?.Invoke(data.index);
        }
        else if (!DrawButPressed && DrawButWasPressed)
        {
            drawingButtonReleased?.Invoke(data.index);
        }
    }

    private void OnApplicationQuit() => Cleanup();
    private void OnDisable() => Cleanup();

    #region Controller rotation functions

    private Quaternion GyroRotation(int index, JSL.IMU_STATE imu, float deltaTime)
    {
        Vector3 omega = new Vector3(
            -imu.gyroX * Mathf.Deg2Rad,
            -imu.gyroY * Mathf.Deg2Rad,
            imu.gyroZ * Mathf.Deg2Rad
        );

        float magnitude = omega.magnitude;
        float halfAngle = magnitude * 0.5f * deltaTime;

        if (halfAngle < 1e-6f)
            return currentRotation[index];

        float sinHalf = Mathf.Sin(halfAngle);
        Quaternion delta = new Quaternion(
            (omega.x / magnitude) * sinHalf,
            (omega.y / magnitude) * sinHalf,
            (omega.z / magnitude) * sinHalf,
            Mathf.Cos(halfAngle)
        );

        return Quaternion.Normalize(delta * currentRotation[index]);
    }

    private Quaternion ApplyAccelCorrection(Quaternion rotation, JSL.IMU_STATE imu, float deltaTime)
    {
        Vector3 accel = new Vector3(-imu.accelX, imu.accelY, imu.accelZ);
        float accelMag = accel.magnitude;

        bool isStable = accelMag > 0.85f && accelMag < 1.15f;
        if (!isStable)
            return rotation;

        Vector3 gyroUp = rotation * Vector3.up;
        Vector3 accelUp = accel.normalized;

        float tiltError = Vector3.Angle(gyroUp, accelUp);
        if (tiltError <= 0.5f)
            return rotation;

        //Vector3 error = Vector3.Cross(gyroUp, accelUp);
        //Quaternion tiltCorr = Quaternion.Euler(error * 5f);
        Quaternion tiltCorr = Quaternion.FromToRotation(gyroUp, accelUp);

        float correctionRate = Mathf.Lerp(1f, 5f, Mathf.InverseLerp(0.5f, 10f, tiltError));
        float correctionStep = correctionRate * deltaTime;

        Quaternion corrected = Quaternion.Slerp(rotation, tiltCorr * rotation, correctionStep);

        return Normalize(corrected);
    }

    private Quaternion Normalize(Quaternion q)
    {
        float mag = Mathf.Sqrt(q.x * q.x + q.y * q.y + q.z * q.z + q.w * q.w);
        if (mag < 1e-6f)
            return Quaternion.identity;

        return new Quaternion(q.x / mag, q.y / mag, q.z / mag, q.w / mag);
    }

    private void ResetAllRotation(int index)
    {
        currentRotation[index] = Quaternion.identity;
        Debug.Log("Rotation reset");
    }
    #endregion

    #region Switch Connection and Cleanup
    private void Cleanup()
    {
        if (cleaned || count <= 0)
            return;

        for (int i = 0; i < count; i++)
        {
            JSL.JslPauseContinuousCalibration(handles[i]);
        }

        JSL.JslSetCallback(null);
        JSL.JslDisconnectAndDisposeAll();
        count = 0;

        cleaned = true;
    }

    private void OnJSLCallback(
        int handle,
        JSL.JOY_SHOCK_STATE currState,
        JSL.JOY_SHOCK_STATE prevState,
        JSL.IMU_STATE currImu,
        JSL.IMU_STATE prevImu,
        float deltaTime)
    {
        int index = Array.FindIndex(handles, item => item == handle);

        if (index == -1)
            return;

        messageQueue.Enqueue(new ImuData
        {
            index = index,
            currImu = currImu,
            prevImu = prevImu,
            currState = currState,
            prevState = prevState,
            deltaTime = deltaTime
        });
    }

    private IEnumerator ConnectAfterDelay()
    {
        yield return new WaitForSeconds(0.5f);

        int devices = JSL.JslConnectDevices();
        Debug.Log($"Connected Devices: {devices}");

        count = JSL.JslGetConnectedDeviceHandles(handles, handles.Length);
        Debug.Log($"Handles Found: {count}");

        if (count <= 0)
            yield break;

        Debug.Log("Calibrating");

        for (int i = 0; i < count; i++)
        {
            JSL.JslSetGyroSpace(handles[i], i);

            JSL.JslResetContinuousCalibration(handles[i]);
            JSL.JslStartContinuousCalibration(handles[i]);
        }

        yield return new WaitForSeconds(GameConstants.controllerCallibrationTime);

        for (int i = 0; i < count; i++)
        {
            JSL.JslPauseContinuousCalibration(handles[i]);
        }

        Debug.Log("Continuous calibration complete");

        callbackDelegate = OnJSLCallback;
        JSL.JslSetCallback(callbackDelegate);

        readyToConnect = true;
    }
    #endregion

    #region Events to send drawing request
    public event Action<int> drawingButtonPressed;
    public event Action<int> drawingButtonReleased;
    #endregion
}