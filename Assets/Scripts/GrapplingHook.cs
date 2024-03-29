﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingHook : MonoBehaviour
{
    public GameObject player;
    public float speed;
    public float minDistance;
    public Transform hitBall;

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

    void Update()
    {
        LogManager.instance.action = ActionEnum.Idle;

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

        //Player state check
        RaycastHit hit = new RaycastHit();

        if (Physics.Raycast(player.transform.position, Vector3.down, out hit, 2))
        {
            LogManager.instance.state = StateEnum.OnGround;
        }
        else
        {
            LogManager.instance.state = StateEnum.InAir;
        }

        if (!hooked && Physics.Raycast(cam.position, cam.forward, out hit, 50))
        {
            hitBall.position = hit.point;
        }
        else
        {
            hitBall.position = player.transform.position;
        }
    }

    public void Press()
    {
        RaycastHit hit;

        if (Physics.Raycast(cam.position, cam.forward, out hit, 50))
        {
            hooked = true;
            targetPoint = hit.point;
        }

        LogManager.instance.action = ActionEnum.Press;

        LogManager.instance.Logger(targetPoint);
    }

    public void Release()
    {
        hooked = false;
        line.enabled = false;

        LogManager.instance.action = ActionEnum.Release;

        LogManager.instance.Logger();
    }

    public void ConnectHook()
    {
        line.enabled = true;

        line.SetPosition(0, transform.position);
        line.SetPosition(1, targetPoint);

        if (Vector3.Distance(transform.position, targetPoint) > minDistance)
        {
            player.GetComponent<Rigidbody>().velocity = (targetPoint - transform.position).normalized * speed;
        }
        else
        {
            player.GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
    }
}