﻿using System;
using System.Collections;
using UnityEngine;

[Serializable]
public struct Point3
{
    public float x;
    public float y;
    public float z;

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

    public override string ToString()
    {
        return "[" + x + ", " + y + ", " + z + "]";
    }

    public override bool Equals(object o)
    {
        //return false if type mismatch
        if (o == null || GetType() != o.GetType())
            return false;

        //cast to point3
        Point3 p = (Point3)o;

        return p.x == x && p.y == y && p.z == z;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}
