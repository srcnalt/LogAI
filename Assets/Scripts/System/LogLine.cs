using System;
using UnityEngine;

[Serializable]
public class LogLine
{
    public float  time;
    public string stateName;
    public string actionName;

    public Vector3 playerPosition;
    public Quaternion cameraRotation;
    public Vector3 lookAtPoint;
}