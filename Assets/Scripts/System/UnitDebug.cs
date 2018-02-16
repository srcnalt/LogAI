using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[ExecuteInEditMode]
public class UnitDebug : MonoBehaviour
{
    #region Public variables

    public enum DrawMode { on, off, bounding_box_only, unit_cubes_only }
    public enum RecorderState { idle, recording, replaying };

    [Header("System variables")]
    public DrawMode drawMode = DrawMode.on;
    public RecorderState recorderState = RecorderState.idle;
    public Transform player;
    public Transform playerCamera;

    [Header("Unit Cube Variables")]
    [Range(1, 5)]
    public float unitSize;
    public Color drawColor;
    public Color activeCubeColor;

    [Header("Bounding Box Variables")]
    public Vector3 boundingBoxSize;
    public Vector3 boundingBoxPivot;
    public Color boundingBoxColor;

    [Header("Active Log")]
    public TextAsset activeLog;

    #endregion

    private Vector3 currentActiveCube = new Vector3(0, 0, 0);
    private Vector3 previousActiveCube = new Vector3(0, 0, 0);

    private SessionLog sessionLog;
    private float loggerTick = 1;
    private float loggerCount = 0;
    private bool loggerOn = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            Record();
            GuiManager.instance.recordButton.image.color = new Color(1, 0, 0); 
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            Stop();
            GuiManager.instance.recordButton.image.color = new Color(1, 1, 1);
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            Replay();
        }
    }

    public void Record()
    {
        loggerOn = true;

        recorderState = RecorderState.recording;

        sessionLog = new SessionLog(DateTime.Now.ToString("dd-MM-yy-HH-mm-ss"), "TestMap");

        InvokeRepeating("Logger", 0, loggerTick);
    }

    public void Stop()
    {
        loggerOn = false;

        CancelInvoke();
        recorderState = RecorderState.idle;

        sessionLog.sessionEnd = DateTime.Now.ToString("dd-MM-yy-HH-mm-ss");

        string json = JsonUtility.ToJson(sessionLog);

        File.WriteAllText(Application.dataPath + "/Logs/session_log_" + sessionLog.sessionStart + ".json", json);
    }

    public void Replay()
    {
        recorderState = RecorderState.replaying;

        //TODO: replay

        SessionLog replaySession = JsonUtility.FromJson<SessionLog>(activeLog.text);

        StartCoroutine(PlayRecordingSteps(replaySession.logs));
    }
    
    IEnumerator PlayRecordingSteps(List<LogLine> logLines)
    {
        float oldTime = 0;

        foreach (LogLine line in logLines)
        {
            //current payer state, add all things here when needed in logLine type
            Vector3 currentPos = player.position;
            float time = 0;
            float step = line.time - oldTime;

            while (time <= step)
            {
                time += Time.deltaTime;

                player.position = Vector3.Lerp(currentPos, line.playerPosition, time / step);

                yield return null;
            }

            oldTime = line.time;
        }
    }

    void OnDrawGizmos()
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
                    }

                    if (currentActiveCube != previousActiveCube && loggerOn)
                    {
                        Logger();

                        previousActiveCube = currentActiveCube;
                    }

                    if (drawMode != DrawMode.off && (drawMode == DrawMode.unit_cubes_only || drawMode != DrawMode.bounding_box_only))
                    {
                        if (isActiveCube)
                        {
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
        }

        if (drawMode != DrawMode.off && (drawMode == DrawMode.bounding_box_only || drawMode != DrawMode.unit_cubes_only))
        {
            Gizmos.color = new Color(1, 0, 0);
            Gizmos.DrawSphere(boundingBoxPivot, 0.1f);

            Vector3 boundingBoxCenter = new Vector3(boundingBoxPivot.x + boundingBoxSize.x / 2, boundingBoxPivot.y + boundingBoxSize.y / 2, boundingBoxPivot.z + boundingBoxSize.z / 2);

            Gizmos.color = boundingBoxColor;
            Gizmos.DrawWireCube(boundingBoxCenter, boundingBoxSize);
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
        Debug.Log("logged " + Time.time + " : " + loggerCount++);

        if(recorderState == RecorderState.recording)
        {
            LogLine logLine = new LogLine();

            logLine.time =           Time.time;
            logLine.stateName =      "TestState";
            logLine.actionName =     "TestAction";
            logLine.playerPosition = player.position;
            logLine.playerRotation = player.rotation;
            logLine.cameraRotation = playerCamera.rotation;

            sessionLog.logs.Add(logLine);
        }
    }
}
