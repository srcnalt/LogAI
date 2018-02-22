using System;
using UnityEngine;

[Serializable]
public class LogLine
{
    public float  time;
    public StateEnum state;
    public ActionEnum action;

    public Point3 playerPosition;
    public Rotation4 cameraRotation;
    public Point3 lookAtPoint;
}