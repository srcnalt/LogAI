using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[CustomEditor(typeof(LogManager))]
public class LogManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        TextAsset[] textAssets = Resources.LoadAll<TextAsset>("Logs/");
        
        if (GUILayout.Button("Build Log Batch"))
        {
            LogBatch logBatch = new LogBatch();

            foreach (TextAsset textAsset in textAssets)
            {
                SessionLog sessionLog = JsonUtility.FromJson<SessionLog>(textAsset.text);

                //new maps session logs
                if(logBatch.mapName != sessionLog.mapName)
                {
                    logBatch.mapName = sessionLog.mapName;
                }

                foreach (LogSection section in sessionLog.logSections)
                {
                    if(!logBatch.logSectionDictionary.ContainsKey(section.sector))
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
