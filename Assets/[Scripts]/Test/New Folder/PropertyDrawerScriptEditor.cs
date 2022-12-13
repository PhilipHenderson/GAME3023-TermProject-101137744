using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PropertyDrawerEditor))]
public class PropertyDrawerScriptEditor : Editor
{
    public override void OnInspectorGUI()
    {
        //DrawDefaultInspector();
        DisplayLocation1();
        DisplayLocation2();
    }

    public void DisplayLocation1()
    {
        PropertyDrawerEditor myPropertyDrawerEditor = (PropertyDrawerEditor)target;
        // Editable
        myPropertyDrawerEditor.displayLocation1 = EditorGUILayout.Vector2Field("Display Location 1", myPropertyDrawerEditor.displayLocation1);
        // Not Editable
        EditorGUILayout.LabelField("Display Location 1", myPropertyDrawerEditor.DisplayLocation1.ToString());
    }
    public void DisplayLocation2()
    {
        PropertyDrawerEditor myPropertyDrawerEditor = (PropertyDrawerEditor)target;

        myPropertyDrawerEditor.displayLocation2 = EditorGUILayout.Vector2Field("Display Location 2", myPropertyDrawerEditor.displayLocation2);
    }
}
