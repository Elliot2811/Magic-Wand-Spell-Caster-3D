using System;
using System.Collections;
using System.Collections.Concurrent;
using UnityEngine;

public class JoyConTracker : MonoBehaviour
{
    public static JoyConTracker Instance { get; private set; }

    public GameObject gyroRepresentation;

    private int count = 0;
    private int[] handles = new int[16];

    private Quaternion currentRotation = Quaternion.identity;
    public Quaternion CurrentRotation => currentRotation;

    public Vector2 ScreenPos
    {
        get { return CaculateRotationToCameraPos(currentRotation); }
    }

    public bool Connected => count > 0;

    private struct ImuData
    {
        public JSL.IMU_STATE currImu;
        public JSL.IMU_STATE prevImu;
        public JSL.JOY_SHOCK_STATE currState;
        public JSL.JOY_SHOCK_STATE prevState;
        public float deltaTime;
    }

    private readonly ConcurrentQueue<ImuData> messageQueue = new ConcurrentQueue<ImuData>();

    private JSL.CallBackDelegate callbackDelegate;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
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

        if (gyroRepresentation != null)
            gyroRepresentation.transform.rotation = currentRotation;
    }

    private void ProcessData(ImuData data)
    {
        bool lPressed = (data.currState.buttons & JSL.JSMASK_L) != 0;
        bool lWasPressed = (data.prevState.buttons & JSL.JSMASK_L) != 0;

        if (lPressed && !lWasPressed)
            ResetAll();

        currentRotation = GyroRotation(data.currImu, data.deltaTime);
        currentRotation = ApplyAccelCorrection(currentRotation, data.currImu, data.deltaTime);

        bool zlPressed = (data.currState.buttons & JSL.JSMASK_ZL) != 0;
        bool zlWasPressed = (data.prevState.buttons & JSL.JSMASK_ZL) != 0;

        if (zlPressed && !zlWasPressed)
        {
            drawingButtonPressed?.Invoke();
        }
        else if (!zlPressed && zlWasPressed)
        {
            drawingButtonReleased?.Invoke();
        }
    }

    private void OnApplicationQuit() => Cleanup();
    private void OnDisable() => Cleanup();

    #region Controller rotation functions

    private Quaternion GyroRotation(JSL.IMU_STATE imu, float deltaTime)
    {
        Vector3 omega = new Vector3(
            -imu.gyroX * Mathf.Deg2Rad,
            -imu.gyroY * Mathf.Deg2Rad,
            imu.gyroZ * Mathf.Deg2Rad
        );

        float magnitude = omega.magnitude;
        float halfAngle = magnitude * 0.5f * deltaTime;

        if (halfAngle < 1e-6f)
            return currentRotation;

        float sinHalf = Mathf.Sin(halfAngle);
        Quaternion delta = new Quaternion(
            (omega.x / magnitude) * sinHalf,
            (omega.y / magnitude) * sinHalf,
            (omega.z / magnitude) * sinHalf,
            Mathf.Cos(halfAngle)
        );

        return Normalize(currentRotation * delta);
    }

    private Quaternion ApplyAccelCorrection(Quaternion rotation, JSL.IMU_STATE imu, float deltaTime)
    {
        Vector3 accel = new Vector3(imu.accelX, imu.accelY, imu.accelZ);
        float accelMag = accel.magnitude;
        bool isStable = accelMag > 0.85f && accelMag < 1.15f;

        if (!isStable)
            return rotation;

        Vector3 gyroUp = rotation * Vector3.up;
        Vector3 accelUp = accel.normalized;
        float tiltError = Vector3.Angle(gyroUp, accelUp);

        if (tiltError <= 0.5f)
            return rotation;

        Vector3 euler = rotation.eulerAngles;
        Quaternion tiltCorr = Quaternion.FromToRotation(gyroUp, accelUp);
        float correctionRate = Mathf.Lerp(1f, 5f, Mathf.InverseLerp(0.5f, 10f, tiltError));
        float correctionStep = correctionRate * deltaTime * Mathf.Deg2Rad;

        Quaternion corrected = Quaternion.Slerp(rotation, tiltCorr * rotation, correctionStep);

        // Reapply original yaw — accel has no yaw information
        Vector3 correctedEuler = corrected.eulerAngles;
        return Quaternion.Euler(correctedEuler.x, euler.y, correctedEuler.z);
    }

    private Quaternion Normalize(Quaternion q)
    {
        float mag = Mathf.Sqrt(q.x * q.x + q.y * q.y + q.z * q.z + q.w * q.w);
        if (mag < 1e-6f)
            return Quaternion.identity;

        return new Quaternion(q.x / mag, q.y / mag, q.z / mag, q.w / mag);
    }

    private void ResetAll()
    {
        currentRotation = Quaternion.identity;
        Debug.Log("Rotation reset");
    }
    #endregion

    #region Switch Connection and Cleanup
    private void Cleanup()
    {
        if (count <= 0)
            return;

        JSL.JslPauseContinuousCalibration(handles[0]);
        JSL.JslSetCallback(null);
        JSL.JslDisconnectAndDisposeAll();
        count = 0;
    }

    private void OnJSLCallback(
        int handle,
        JSL.JOY_SHOCK_STATE currState,
        JSL.JOY_SHOCK_STATE prevState,
        JSL.IMU_STATE currImu,
        JSL.IMU_STATE prevImu,
        float deltaTime)
    {
        if (handle != handles[0])
            return;

        messageQueue.Enqueue(new ImuData
        {
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

        JSL.JslSetGyroSpace(handles[0], 0);

        JSL.JslResetContinuousCalibration(handles[0]);
        JSL.JslStartContinuousCalibration(handles[0]);

        yield return new WaitForSeconds(2f);

        JSL.JslPauseContinuousCalibration(handles[0]);
        Debug.Log("Continuous calibration complete");

        callbackDelegate = OnJSLCallback;
        JSL.JslSetCallback(callbackDelegate);
    }
    #endregion

    #region Convert rotation to screen position
    private Vector2 CaculateRotationToCameraPos(Quaternion rotation)
    {
        Vector3 forward = rotation * Vector3.forward;

        float yawDegrees = Mathf.Atan2(forward.x, forward.z) * Mathf.Rad2Deg;

        float pitchDegrees = Mathf.Asin(Mathf.Clamp(forward.y, -1f, 1f)) * Mathf.Rad2Deg;

        Vector2 posInCamera = new Vector2(
            yawDegrees / GameConstants.HorizontalFovDeg,
            pitchDegrees / GameConstants.VerticalFovDeg
            );

        posInCamera.x = Mathf.Clamp(posInCamera.x, -0.4f, 0.4f);
        posInCamera.y = Mathf.Clamp(posInCamera.y, -0.4f, 0.4f);

        Vector2 viewport = posInCamera + new Vector2(0.5f, 0.5f);

        viewport.x = Mathf.Clamp01(viewport.x);
        viewport.y = Mathf.Clamp01(viewport.y);

        return new Vector2(
            viewport.x * Screen.width,
            viewport.y * Screen.height
            );
    }
    #endregion

    #region Events to send drawing request
    public event Action drawingButtonPressed;
    public event Action drawingButtonReleased;
    #endregion
}