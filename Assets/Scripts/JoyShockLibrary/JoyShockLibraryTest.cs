using System.Collections;
using UnityEngine;

public class JoyShockLibraryTest : MonoBehaviour
{
    // DO NOT FORGET TO DISCONNECT AND DISPOSE ALL ELSE FUTURE PARRING HAS ISSUES



    public GameObject gyroRepresentation;

    private int count = 0;
    private int[] handles = new int[16];

    Quaternion currentRotation = Quaternion.identity;

    [Range(0.9f, 0.999f)]
    public float filterAlpha = 0.98f;

    private void Start()
    {
        JSL.JslDisconnectAndDisposeAll();

        StartCoroutine(ConnectAfterDelay());
    }

    private void Update()
    {
        if (count <= 0)
            return;

        ApplyGyro();
    }

    private void OnApplicationQuit()
    {
        if (count > 0)
        {
            JSL.JslPauseContinuousCalibration(handles[0]);
            JSL.JslDisconnectAndDisposeAll();
        }
    }

    private void OnDisable()
    {
        if (count > 0)
            JSL.JslDisconnectAndDisposeAll();
    }

    private void ApplyGyro()
    {
        JSL.IMU_STATE imu = JSL.JslGetIMUState(handles[0]);

        int buttons = JSL.JslGetButtons(handles[0]);
        if ((buttons & JSL.JSMASK_ZL) != 0)
            ResetYaw();

        currentRotation = GyroRotation(imu);

        Vector3 accel = new Vector3(imu.accelX, imu.accelY, imu.accelZ);
        float accelMag = accel.magnitude;
        bool isStable = accelMag > 0.85f && accelMag < 1.15f;

        if (isStable)
        {
            Vector3 gyroUp = currentRotation * Vector3.up;
            Vector3 accelUp = accel.normalized;

            float tiltError = Vector3.Angle(gyroUp, accelUp);

            if (tiltError > 0.5f)
            {
                Quaternion tiltCorrection = Quaternion.FromToRotation(gyroUp, accelUp);

                float correctionStep = Mathf.Min(tiltError, 2f) * Time.deltaTime * 0.05f;
                currentRotation = Quaternion.Slerp(
                    currentRotation,
                    tiltCorrection * currentRotation,
                    correctionStep
                );
            }
        }

        gyroRepresentation.transform.rotation = currentRotation;
    }

    #region Quaternion Nonsense
    private Quaternion GyroRotation(JSL.IMU_STATE imu)
    {
        Vector3 omega = new Vector3(
            -imu.gyroX * Mathf.Deg2Rad,
            imu.gyroY * Mathf.Deg2Rad,
            imu.gyroZ * Mathf.Deg2Rad
            );

        float qx = currentRotation.x;
        float qy = currentRotation.y;
        float qz = currentRotation.z;
        float qw = currentRotation.w;

        Quaternion derivative = new Quaternion(
            0.5f * (qw * omega.x + qy * omega.z - qz * omega.y),
            0.5f * (qw * omega.y - qx * omega.z + qz * omega.x),
            0.5f * (qw * omega.z + qx * omega.y - qy * omega.x),
            0.5f * (-qx * omega.x - qy * omega.y - qz * omega.z)
            );

        Quaternion result = new Quaternion(
            qx + derivative.x * Time.deltaTime,
            qy + derivative.y * Time.deltaTime,
            qz + derivative.z * Time.deltaTime,
            qw + derivative.w * Time.deltaTime
            );

        return Normalize(result);
    }

    private Quaternion Normalize(Quaternion q)
    {
        float mag = Mathf.Sqrt(q.x * q.x + q.y * q.y + q.z * q.z + q.w * q.w);
        if (mag < 1e-6f) return Quaternion.identity;
        return new Quaternion(q.x / mag, q.y / mag, q.z / mag, q.w / mag);
    }
    #endregion

    private void ResetYaw()
    {
        Vector3 euler = currentRotation.eulerAngles;
        currentRotation = Quaternion.Euler(euler.x, 0f, euler.z);

        Debug.Log("Yaw reset");
    }

    private IEnumerator ConnectAfterDelay()
    {
        yield return new WaitForSeconds(0.5f);

        int devices = JSL.JslConnectDevices();
        Debug.Log("Connected Devices: " + devices);

        count = JSL.JslGetConnectedDeviceHandles(handles, handles.Length);
        Debug.Log("Handles Found: " + count);

        if (count <= 0)
            yield break;

        JSL.JslSetGyroSpace(handles[0], 2);

        JSL.JslResetContinuousCalibration(handles[0]);
        JSL.JslStartContinuousCalibration(handles[0]);

        yield return new WaitForSeconds(2f);

        JSL.JslPauseContinuousCalibration(handles[0]);
        Debug.Log("Continous callibration complete");
    }
}