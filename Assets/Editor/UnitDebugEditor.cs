using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;

[CustomEditor(typeof(LogManager))]
public class LogManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        LogManager lm = (LogManager)target;

        TextAsset[] test = Resources.LoadAll<TextAsset>("Logs/");

        lm.logs = new List<TextAsset>(test);

        if (GUILayout.Button("Build Object"))
        {
            
        }
    }
}
