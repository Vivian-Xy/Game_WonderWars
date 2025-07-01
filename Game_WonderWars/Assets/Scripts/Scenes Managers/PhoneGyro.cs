using UnityEngine;

/// <summary>
/// Attach this script to the GameObject you want to rotate with the phone's gyroscope.
/// </summary>
public class PhoneGyro : MonoBehaviour
{
    private Gyroscope gyro;
    private bool gyroEnabled;
    // This rotation fix is commonly used for landscape orientation
    private readonly Quaternion rotFix = new Quaternion(0, 0, 1, 0);

    void Start()
    {
        // Enable the gyroscope if supported
        if (SystemInfo.supportsGyroscope)
        {
            gyro = Input.gyro;
            gyro.enabled = true;
            gyroEnabled = true;
        }
        else
        {
            gyroEnabled = false;
            Debug.LogWarning("Gyroscope not supported on this device.");
        }
    }

    void Update()
    {
        if (gyroEnabled)
        {
            // Apply the gyroscope attitude with the rotation fix
            transform.localRotation = gyro.attitude * rotFix;
        }
    }
}






