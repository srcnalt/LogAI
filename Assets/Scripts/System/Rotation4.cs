using System;
using UnityEngine;

[Serializable]
public class Rotation4
{
    public float x;
    public float y;
    public float z;
    public float w;

    public Rotation4()
    {
        x = 0;
        y = 0;
        z = 0;
        w = 0;
    }

    public Rotation4(float x, float y, float z, float w)
    {
        this.x = x;
        this.y = y;
        this.z = z;
        this.w = w;
    }

    public Quaternion Quaternion
    {
        get {
            return new Quaternion(x, y, z, w);
        }

        set {
            x = value.x;
            y = value.y;
            z = value.z;
            w = value.w;
        }
    }

    public static Rotation4 ToRotation(Quaternion q)
    {
        return new Rotation4(q.x, q.y, q.z, q.w);
    }

    public static Quaternion ToQuaternion(Rotation4 r)
    {
        return new Quaternion(r.x, r.y, r.z, r.w);
    }
}
