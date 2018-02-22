using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LogSection
{
    public Point3 sector;
    public List<LogLine> logLines = new List<LogLine>();
}
