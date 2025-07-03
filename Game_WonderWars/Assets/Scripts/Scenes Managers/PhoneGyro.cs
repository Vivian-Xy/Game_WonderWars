using UnityEngine;

/// <summary>
/// Attach this script to the GameObject you want to rotate with the phone's gyroscope.
/// </summary>
public class GyroController : MonoBehaviour
{
    private Gyroscope gyro;
    private bool gyroSupported;

    void Start()
    {
        gyroSupported = SystemInfo.supportsGyroscope;
        if (gyroSupported)
        {
            gyro = Input.gyro;
            gyro.enabled = true;
        }
        else
        {
            Debug.LogWarning("Gyroscope not supported on this device.");
        }
    }

    void Update()
    {
        if (gyroSupported)
        {
            // For most devices, this orientation works for landscape
            transform.localRotation = Quaternion.Euler(90f, 0f, 0f) * gyro.attitude;
        }
    }
}






