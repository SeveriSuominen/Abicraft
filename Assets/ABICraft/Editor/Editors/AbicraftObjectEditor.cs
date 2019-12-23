using AbicraftMonos;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AbicraftObject))]
public class AbicraftObjectEditor : Editor
{
    int instantiatePoolAmount;

    public override void OnInspectorGUI()
    {
        var abicraftObject = target as AbicraftObject;

        abicraftObject.InstantiateObjectToPool = GUILayout.Toggle(abicraftObject.InstantiateObjectToPool, "Instantiate Object To Pool");

        if (abicraftObject.InstantiateObjectToPool)
            abicraftObject.InstantiateToPoolAmount = EditorGUILayout.IntField(abicraftObject.InstantiateToPoolAmount);

        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
    }
}
