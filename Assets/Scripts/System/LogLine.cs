using System;
using UnityEngine;

[Serializable]
public class LogLine
{
    public float  time;
    public StateEnum state;
    public ActionEnum action;

    public Vector3 playerPosition;
    public Quaternion cameraRotation;
    public Vector3 lookAtPoint;
}