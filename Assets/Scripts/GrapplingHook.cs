using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingHook : MonoBehaviour
{
    public GameObject player;
    public float speed;

    private Transform cam;
    private LineRenderer line;

    private Vector3 targetPoint;
    public bool hooked;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        cam = Camera.main.transform;
        line = GetComponent<LineRenderer>();
    }
    
    void Update ()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Press();
        }

        if (Input.GetMouseButtonUp(0))
        {
            Release();
        }

        if (hooked)
        {
            ConnectHook();
        }
	}

    public void Press()
    {
        RaycastHit hit;

        if (Physics.Raycast(cam.position, cam.forward, out hit, 100))
        {
            hooked = true;
            line.enabled = true;

            targetPoint = hit.point;
        }

        if (LogManager.instance.recorderState == LogManager.RecorderState.recording)
            LogManager.instance.Logger("Press");

        Debug.Log("Press");
    }

    public void Release()
    {
        hooked = false;
        line.enabled = false;

        if(LogManager.instance.recorderState == LogManager.RecorderState.recording)
            LogManager.instance.Logger("Release");

        Debug.Log("Release");
    }

    public void ConnectHook()
    {
        line.SetPosition(0, transform.position);
        line.SetPosition(1, targetPoint);

        if (Vector3.Distance(transform.position, targetPoint) > 1)
        {
            player.GetComponent<Rigidbody>().velocity = (targetPoint - transform.position).normalized * speed;
        }
    }
}
