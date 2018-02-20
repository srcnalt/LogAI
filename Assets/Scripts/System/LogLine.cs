using System;
using UnityEngine;

[Serializable]
public class LogLine
{
    public float  time;
    public State state;
    public Action action;

    public Vector3 playerPosition;
    public Quaternion cameraRotation;
    public Vector3 lookAtPoint;
}