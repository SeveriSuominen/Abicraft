using AbicraftCore;
using AbicraftMonos;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AbicraftNodes.Editors
{
    [CustomEditor(typeof(Abicraft))]
    public class AbicraftEditor : Editor
    {
        Abicraft abicraft;

        public override void OnInspectorGUI()
        {
            if (!abicraft)
                abicraft = target as Abicraft;

            GUILayout.Label("Abicraft global data file");
            abicraft.dataFile = EditorGUILayout.ObjectField(abicraft.dataFile, typeof(AbicraftGlobalDataFile)) as AbicraftGlobalDataFile;

            if (GUI.changed)
            {
                EditorUtility.SetDirty(target);
            }
        }
    }
}

