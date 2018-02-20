using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class LogManager : MonoBehaviour
{
    #region Public variables
    public static LogManager instance;

    public enum DrawMode { off, on, bounding_box_only, unit_cubes_only }
    public enum RecorderState { idle, recording, replaying };

    [Header("System variables")]
    public DrawMode drawMode = DrawMode.on;
    public RecorderState recorderState = RecorderState.idle;
    public Transform player;
    public Transform playerCamera;
    public GrapplingHook grapplingHook;
    public Transform spawnPoint;
    public float loggerTick;

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
    public bool drawPath;

    #endregion

    #region Private variables
    private Vector3 currentActiveCube = new Vector3(0, 0, 0);
    private Vector3 previousActiveCube = new Vector3(0, 0, 0);
    private SessionLog sessionLog;
    private Dictionary<string, Action> actionList;
    #endregion

    private void Awake()
    {
        if (instance != this)
            Destroy(instance);

        instance = this;
    }

    private void Start()
    {
        actionList = new Dictionary<string, Action>();

        actionList.Add("Press", grapplingHook.Press);
        actionList.Add("Release", grapplingHook.Release);

        player.position = spawnPoint.position;
    }

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

        if(recorderState == RecorderState.recording)
        {
            //InsertLogByUnitChange();
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
        CancelInvoke();
        recorderState = RecorderState.idle;

        sessionLog.sessionEnd = DateTime.Now.ToString("dd-MM-yy-HH-mm-ss");

        string json = JsonUtility.ToJson(sessionLog);

        File.WriteAllText(Application.dataPath + "/Logs/session_log_" + sessionLog.sessionStart + ".json", json);
    }

    public void Replay()
    {
        Debug.Log("Replaying...");

        player.position = spawnPoint.position;

        recorderState = RecorderState.replaying;

        //TODO: replay

        SessionLog replaySession = JsonUtility.FromJson<SessionLog>(activeLog.text);

        StartCoroutine(PlayRecordingSteps(replaySession.logs));
    }
    
    IEnumerator PlayRecordingSteps(List<LogLine> logLines)
    {
        float oldTime = 0;
        LogLine previousLog = logLines[0];
        logLines.RemoveAt(0);

        foreach (LogLine line in logLines)
        {
            float time = 0;
            float step = line.time - oldTime;

            while (time <= step)
            {
                time += Time.deltaTime;

                if (line.lookAtPoint != Vector3.zero)
                {
                    Debug.DrawLine(Camera.main.transform.position, line.lookAtPoint);
                    Camera.main.transform.LookAt(line.lookAtPoint);
                }
                else
                {
                    Camera.main.transform.rotation = Quaternion.Lerp(previousLog.cameraRotation, line.cameraRotation, time / step);
                }

                //player rotation may not be needed
                //player.rotation = Quaternion.Lerp(firstLog.playerRotation, line.playerRotation, time / step);

                //Call the registered method
                if(line.actionName != string.Empty)
                {
                    Debug.Log("Invoked method...");
                    actionList[line.actionName].Invoke();
                }

                yield return null;
            }

            oldTime = line.time;
            previousLog = line;
        }

        Debug.Log("Replay ended...");
    }

    public void Logger()
    {
        Logger(null, Vector3.zero);
    }

    public void Logger(string actionName)
    {
        Logger(actionName, Vector3.zero);
    }

    public void Logger(string actionName, Vector3 lookAtPoint)
    {
        if (recorderState == RecorderState.recording)
        {
            LogLine logLine = new LogLine();

            logLine.time = Time.time;
            logLine.stateName = null;
            logLine.actionName = actionName;
            logLine.playerPosition = player.position;
            logLine.cameraRotation = playerCamera.rotation;
            logLine.lookAtPoint = lookAtPoint;

            sessionLog.logs.Add(logLine);
        }
    }

    private void InsertLogByUnitChange()
    {
        Vector3 currentActiveCube = new Vector3(
            Mathf.Floor(player.position.x / unitSize),
            Mathf.Floor(player.position.y / unitSize),
            Mathf.Floor(player.position.z / unitSize)
        );

        if (previousActiveCube != currentActiveCube)
        {
            previousActiveCube = currentActiveCube;

            Debug.Log("logged");

            Logger(null);
        }
    }

    #region Visual Debug Methods

    //TODO: Add only active box

    void OnDrawGizmos()
    {
        DrawPathFromActiveRecording();

        if (drawMode == DrawMode.off) return;

        Vector3 currentActiveCube = new Vector3(
            Mathf.Floor(player.position.x / unitSize),
            Mathf.Floor(player.position.y / unitSize),
            Mathf.Floor(player.position.z / unitSize)
        );

        Vector3 cubeCenter = new Vector3(
            boundingBoxPivot.x + unitSize / 2 + currentActiveCube.x,
            boundingBoxPivot.y + unitSize / 2 + currentActiveCube.y,
            boundingBoxPivot.z + unitSize / 2 + currentActiveCube.z
        );

        if (previousActiveCube != currentActiveCube)
        {
            previousActiveCube = currentActiveCube;            
        }

        DrawBoundingBox();
    }

    private void DrawUnitCubes(bool isActiveCube, Vector3 cubeCenter)
    {
        if (drawMode == DrawMode.unit_cubes_only || drawMode != DrawMode.bounding_box_only)
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

    private void DrawBoundingBox()
    {
        if (drawMode == DrawMode.bounding_box_only || drawMode != DrawMode.unit_cubes_only)
        {
            Gizmos.color = new Color(1, 0, 0);
            Gizmos.DrawSphere(boundingBoxPivot, 0.1f);

            Vector3 boundingBoxCenter = new Vector3(boundingBoxPivot.x + boundingBoxSize.x / 2, boundingBoxPivot.y + boundingBoxSize.y / 2, boundingBoxPivot.z + boundingBoxSize.z / 2);

            Gizmos.color = boundingBoxColor;
            Gizmos.DrawWireCube(boundingBoxCenter, boundingBoxSize);
        }
    }

    private void DrawPathFromActiveRecording()
    {
        if (drawPath && activeLog)
        {
            List<LogLine> logs = JsonUtility.FromJson<SessionLog>(activeLog.text).logs;

            for (int i = 0; i < logs.Count - 1; i++)
            {
                Gizmos.color = Color.white;

                if (logs[i + 1].actionName != string.Empty)
                    Handles.Label(logs[i + 1].playerPosition, logs[i + 1].actionName);
                else
                    Gizmos.DrawSphere(logs[i + 1].playerPosition, 0.1f);

                if (logs[i + 1].lookAtPoint != Vector3.zero)
                {
                    Gizmos.color = Color.black;
                    Gizmos.DrawSphere(logs[i + 1].lookAtPoint, 0.5f);
                }

                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(logs[i].playerPosition, logs[i + 1].playerPosition);
            }
        }
    }

    bool IsPlayerInTheBox(Vector3 cubeIndex)
    {
        Vector3 pos = player.position;

        if (((pos.x > boundingBoxPivot.x + cubeIndex.x) && (pos.x < boundingBoxPivot.x + cubeIndex.x + unitSize)) &&
        ((pos.y > boundingBoxPivot.y + cubeIndex.y) && (pos.y < boundingBoxPivot.y + cubeIndex.y + unitSize)) &&
        ((pos.z > boundingBoxPivot.z + cubeIndex.z) && (pos.z < boundingBoxPivot.z + cubeIndex.z + unitSize)))
        {
            return true;
        }

        return false;
    }

    #endregion
}
