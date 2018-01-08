using System;
using System.Collections.Generic;

[Serializable]
public class SessionLog
{
    public string sessionStart;
    public string sessionEnd;
    public string mapName;
    public List<LogLine> logs;

    public SessionLog(string sessionStart, string mapName)
    {
        this.sessionStart = sessionStart;
        this.sessionEnd = "";
        this.mapName = mapName;
        this.logs = new List<LogLine>();
    }
}