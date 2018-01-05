using UnityEngine;

public class Movement : MonoBehaviour
{
    #region private variables
    private float verticalAxis;
    private float horizontalAxis;
    private float rotationX;
    #endregion

    #region public variables
    [Range(0,1)]
    public float movementSpeed;
    public float sensitivity;
    #endregion

    void Update ()
    {
        verticalAxis = Input.GetAxis("Vertical");
        horizontalAxis = Input.GetAxis("Horizontal");
        rotationX += Input.GetAxis("Mouse X") * sensitivity;

        Vector3 movementDirection = new Vector3(horizontalAxis, 0, verticalAxis);
        transform.Translate(movementDirection * movementSpeed);

        Quaternion targetRotation = Quaternion.Euler(new Vector3(0, rotationX, 0));
        transform.rotation = targetRotation;
	}
}
