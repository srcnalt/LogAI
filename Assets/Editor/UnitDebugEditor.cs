﻿using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;

[CustomEditor(typeof(LogManager))]
public class LogManagerEditor : Editor
{
    private List<string> options = new List<string>();
    private int index = 0;
    private FileInfo[] paths;
    private LogManager logManager;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        logManager = (LogManager)target;

        if (!logManager.activeLog)
        {
            EditorGUILayout.HelpBox("There is no active log...", MessageType.Error);
        }
        
        EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Active batch");
            BatchDropDown();
        EditorGUILayout.EndHorizontal();

        CreateBatch();
    }

    private void BatchDropDown()
    {
        paths = new DirectoryInfo(Application.dataPath + "/Resources/Batches/").GetFiles();

        foreach (FileInfo path in paths)
        {
            if (path.Extension == ".dat")
            {
                options.Add(path.Name);
            }
        }
        
        index = EditorGUILayout.Popup(index, options.ToArray());

        //if gui changed or activeBatch is empty try loading
        if (GUI.changed || logManager.activeBatch.logSectionDictionary.Count == 0)
        {
            try
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Open(paths[index].FullName, FileMode.Open);

                logManager.activeBatch = (LogBatch)bf.Deserialize(file);

                file.Dispose();
            }
            catch
            {
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.HelpBox("No log batch found...", MessageType.Error);
            }
        }
    }

    private void CreateBatch()
    {
        TextAsset[] textAssets = Resources.LoadAll<TextAsset>("Logs/");

        if (GUILayout.Button("Build Log Batch"))
        {
            LogBatch logBatch = new LogBatch();

            foreach (TextAsset textAsset in textAssets)
            {
                SessionLog sessionLog = JsonUtility.FromJson<SessionLog>(textAsset.text);

                //new maps session logs
                if (logBatch.mapName != sessionLog.mapName)
                {
                    logBatch.mapName = sessionLog.mapName;
                }

                foreach (LogSection section in sessionLog.logSections)
                {
                    if (!logBatch.logSectionDictionary.ContainsKey(section.sector))
                    {
                        logBatch.logSectionDictionary.Add(section.sector, new List<LogSection>());
                    }

                    logBatch.logSectionDictionary[section.sector].Add(section);
                }
            }

            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Create(Application.dataPath + "/Resources/Batches/log_batch_" + logBatch.mapName + ".dat");

            bf.Serialize(file, logBatch);
            file.Close();
        }
    }
}
