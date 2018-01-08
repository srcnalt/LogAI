using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[ExecuteInEditMode]
public class UnitDebug : MonoBehaviour
{
    public DrawMode drawMode = DrawMode.on;
    public RecorderState recorderState = RecorderState.idle;
    public Transform player;
    public Transform playerCamera;

    [Space(20)]
    [Range(1, 5)]
    public float unitSize;
    public Color drawColor;
    public Color activeCubeColor;

    [Space(20)]
    public Vector3 boundingBoxSize;
    public Vector3 boundingBoxPivot;
    public Color boundingBoxColor;

    public enum DrawMode { on, off, bounding_box_only, unit_cubes_only }
    public enum RecorderState { idle, recording, replaying };

    private Vector3 currentActiveCube = new Vector3(0, 0, 0);
    private Vector3 previousActiveCube = new Vector3(0, 0, 0);

    private SessionLog sessionLog;
    private float loggerTick = 1;

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.R))
        {
            Record();
            GuiManager.instance.recordButton.image.color = new Color(1, 0, 0); 
        }

        if (Input.GetKeyUp(KeyCode.T))
        {
            Stop();
            GuiManager.instance.recordButton.image.color = new Color(1, 1, 1);
        }
    }

    public void Record()
    {
        recorderState = RecorderState.recording;

        sessionLog = new SessionLog(DateTime.Now.ToString("dd-MM-yy-HH-mm-ss"), "TestMap");

        InvokeRepeating("Logger", 0, loggerTick);
    }

    public void Stop()
    {
        recorderState = RecorderState.idle;
        sessionLog.sessionEnd = DateTime.Now.ToString("dd-MM-yy-HH-mm-ss");

        string json = JsonUtility.ToJson(sessionLog);

        File.WriteAllText(Application.dataPath + "/Logs/session_log_" + sessionLog.sessionStart + ".json", json);
    }

    public void Replay()
    {
        recorderState = RecorderState.replaying;

        //TODO: replay
    }

    void OnDrawGizmos()
    {
        if (drawMode == DrawMode.off) return;

        if(drawMode == DrawMode.bounding_box_only)
        {
            DrawBoundingBox();
        }
        else if (drawMode == DrawMode.unit_cubes_only)
        {
            DrawUnitCubes();
        }
        else
        {
            DrawBoundingBox();
            DrawUnitCubes();
        }
    }

    void DrawBoundingBox()
    {
        Gizmos.color = new Color(1, 0, 0);
        Gizmos.DrawSphere(boundingBoxPivot, 0.1f);

        Vector3 boundingBoxCenter = new Vector3(boundingBoxPivot.x + boundingBoxSize.x / 2, boundingBoxPivot.y + boundingBoxSize.y / 2, boundingBoxPivot.z + boundingBoxSize.z / 2);

        Gizmos.color = boundingBoxColor;
        Gizmos.DrawWireCube(boundingBoxCenter, boundingBoxSize);
    }

    void DrawUnitCubes()
    {
        for (float x = 0; x < boundingBoxSize.x; x += unitSize)
        {
            for (float y = 0; y < boundingBoxSize.y; y += unitSize)
            {
                for (float z = 0; z < boundingBoxSize.z; z += unitSize)
                {
                    Vector3 cubeCenter = new Vector3(boundingBoxPivot.x + unitSize / 2 + x, 
                                                     boundingBoxPivot.y + unitSize / 2 + y,
                                                     boundingBoxPivot.z + unitSize / 2 + z);

                    bool isActiveCube = IsPlayerInTheBox(new Vector3(x, y, z));

                    if (isActiveCube)
                    {
                        currentActiveCube = new Vector3(x, y, z);

                        Gizmos.color = activeCubeColor;
                        Gizmos.DrawCube(cubeCenter, new Vector3(unitSize, unitSize, unitSize));
                    }
                    else
                    {
                        Gizmos.color = drawColor;
                        Gizmos.DrawWireCube(cubeCenter, new Vector3(unitSize, unitSize, unitSize));
                    }
                }
            }
        }

        if(currentActiveCube != previousActiveCube)
        {
            Logger();

            previousActiveCube = currentActiveCube;
        }
    }

    bool IsPlayerInTheBox(Vector3 cubeIndex)
    {
        Vector3 pos = player.position;

        if(((pos.x > boundingBoxPivot.x + cubeIndex.x) && (pos.x < boundingBoxPivot.x + cubeIndex.x + unitSize)) &&
        ((pos.y > boundingBoxPivot.y + cubeIndex.y) && (pos.y < boundingBoxPivot.y + cubeIndex.y + unitSize)) &&
        ((pos.z > boundingBoxPivot.z + cubeIndex.z) && (pos.z < boundingBoxPivot.z + cubeIndex.z + unitSize)))
        {
            return true;
        }

        return false;
    }

    void Logger()
    {
        if(recorderState == RecorderState.recording)
        {
            LogLine logLine = new LogLine();

            logLine.miliseconds =    DateTime.Now.Millisecond;
            logLine.stateName =      "TestState";
            logLine.actionName =     "TestAction";
            logLine.playerPosition = player.position;
            logLine.playerRotation = player.rotation;
            logLine.cameraRotation = playerCamera.rotation;

            sessionLog.logs.Add(logLine);
        }
    }
}
