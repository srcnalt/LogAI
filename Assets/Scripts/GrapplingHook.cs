﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingHook : MonoBehaviour
{
    public GameObject player;

    private Transform cam;
    private LineRenderer line;

    private Vector3 targetPoint;
    private bool hooked;

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
            RaycastHit hit;

            if (Physics.Raycast(cam.position, cam.forward, out hit, 100))
            {
                hooked = true;
                line.enabled = true;

                targetPoint = hit.point;
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            hooked = false;
            line.enabled = false;
        }

        if (hooked)
        {
            ConnectHook();
        }
	}

    private void ConnectHook()
    {
        line.SetPosition(0, transform.position);
        line.SetPosition(1, targetPoint);

        if (Vector3.Distance(transform.position, targetPoint) > 1)
        {
            player.GetComponent<Rigidbody>().velocity = (targetPoint - transform.position).normalized * 10;
        }
    }
}
