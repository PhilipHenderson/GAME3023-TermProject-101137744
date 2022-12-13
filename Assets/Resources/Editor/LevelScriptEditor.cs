using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PlayerEditor))]
public class LevelScriptEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector(); 
        //MyCustomWindow();
    }

    public void MyCustomWindow()
    {
        PlayerEditor myPlayerEditor = (PlayerEditor)target;

        // Player Name
        myPlayerEditor.playerName = EditorGUILayout.TextField("Player Name", myPlayerEditor.playerName);

        // Exerience and Level
        myPlayerEditor.playerExperience = EditorGUILayout.IntField("Player Experience", myPlayerEditor.playerExperience);
        EditorGUILayout.LabelField("Level", myPlayerEditor.Experience.ToString());

        // Player Speed
        myPlayerEditor.playerSpeed = EditorGUILayout.IntField("Player Speed", myPlayerEditor.playerSpeed);
    }
}
