using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Abicraft))]
public class AbicraftEditor : Editor
{
    //[InitializeOnLoadMethod]
    //public static void InitUpdate() { EditorApplication.update += UpdateAbicraftReferences; }

    Abicraft abicraft;

    /*static void UpdateAbicraftReferences()
    {
        if (abicraft)
        {
            if (!AbicraftGlobalContext.abicraft)
                AbicraftGlobalContext.AddAbicraftInstance(abicraft);
        }
    }*/

    public override void OnInspectorGUI()
    {
        if(!abicraft)
            abicraft = target as Abicraft;

        GUILayout.Label("Abicraft global data file");
        abicraft.dataFile = EditorGUILayout.ObjectField(abicraft.dataFile, typeof(AbicraftGlobalDataFile)) as AbicraftGlobalDataFile;

        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
    }
}
