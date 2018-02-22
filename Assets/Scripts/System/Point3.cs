using System;
using UnityEngine;

[Serializable]
public class Point3
{
    public float x;
    public float y;
    public float z;

    public Point3()
    {
        x = 0;
        y = 0;
        z = 0;
    }

    public Point3(float x, float y, float z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public Vector3 Vector3
    {
        get {
            return new Vector3(x, y, z);
        }

        set {
            x = value.x;
            y = value.y;
            z = value.z;
        }
    }

    public static Point3 ToPoint(Vector3 v)
    {
        return new Point3(v.x, v.y, v.z);
    }

    public static Vector3 ToVector(Point3 s)
    {
        return new Vector3(s.x, s.y, s.z);
    }
}
