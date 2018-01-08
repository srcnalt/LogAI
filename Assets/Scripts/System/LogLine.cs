using System;
using UnityEngine;

[Serializable]
public class LogLine
{
    public int miliseconds;
    public string stateName;
    public string actionName;

    public Vector3 playerPosition;
    public Quaternion playerRotation;
    public Quaternion cameraRotation;
}