using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    private float rotationY;
    private float rotationX;

    public float sensitivity;
    public Vector2 maxAngles;
    public Transform cameraPinPoint;

	void Update ()
    {
        rotationX += Input.GetAxis("Mouse X") * sensitivity;
        rotationY -= Input.GetAxis("Mouse Y") * sensitivity;
        rotationY = Mathf.Clamp(rotationY, maxAngles.x, maxAngles.y);

        Quaternion targetRotation = Quaternion.Euler(new Vector3(rotationY, rotationX, 0.0f));
        transform.rotation = targetRotation;

        transform.position = cameraPinPoint.position;
    }
}
