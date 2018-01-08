using System;
using UnityEngine;

[Serializable]
public class LogLine
{
    public float  time;
    public string stateName;
    public string actionName;

    public Vector3 playerPosition;
    public Quaternion playerRotation;
    public Quaternion cameraRotation;
}