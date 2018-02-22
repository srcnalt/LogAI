using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LogBatch
{
    public string mapName;
    public Dictionary<Point3, List<LogSection>> logSectionDictionary = new Dictionary<Point3, List<LogSection>>();
}
