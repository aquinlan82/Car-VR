using UnityEngine;
using System.Collections;

/*
Uses gyroscope in mobile device to move camera, mimicing VR
*/

public class MoveCamera : MonoBehaviour
{
    private float initialYAngle = 0f;
    private float appliedGyroYAngle = 0f;
    private float calibrationYAngle = 0f;
    private Transform rawGyroRotation;
    private float tempSmoothing;

    [SerializeField] private float smoothing = 0.1f;

	/* Initialize variables and get starting position */
    private IEnumerator Start() {
        Input.gyro.enabled = true;
        Application.targetFrameRate = 60;
        initialYAngle = transform.eulerAngles.y;

        rawGyroRotation = new GameObject("GyroRaw").transform;
        rawGyroRotation.position = transform.position;
        rawGyroRotation.rotation = transform.rotation;

        // Wait until gyro is active, then calibrate to reset starting rotation.
        yield return new WaitForSeconds(1);

        StartCoroutine(CalibrateYAngle());
    }

	/* On each update, calculate new rotation and move to it */
    private void Update() {
        ApplyGyroRotation();
        ApplyCalibration();

        transform.rotation = Quaternion.Slerp(transform.rotation, rawGyroRotation.rotation, smoothing);
    }

	/* Update calibration angle based on gyro */
    private IEnumerator CalibrateYAngle() {
        tempSmoothing = smoothing;
        smoothing = 1;
        calibrationYAngle = appliedGyroYAngle - initialYAngle; // Offsets the y angle in case it wasn't 0 at edit time.
        yield return null;
        smoothing = tempSmoothing;
    }

	/* Get angle to rotate */
    private void ApplyGyroRotation() {
        rawGyroRotation.rotation = Input.gyro.attitude;
        rawGyroRotation.Rotate(0f, 0f, 180f, Space.Self); // Swap "handedness" of quaternion from gyro.
        rawGyroRotation.Rotate(90f, 180f, 0f, Space.World); // Rotate to make sense as a camera pointing out the back of your device.
        appliedGyroYAngle = rawGyroRotation.eulerAngles.y; // Save the angle around y axis for use in calibration.
    }

	/* Rotates y angle back however much it deviated when calibrationYAngle was saved */
    private void ApplyCalibration() {
        rawGyroRotation.Rotate(0f, - calibrationYAngle, 0f, Space.World); 
    }

}