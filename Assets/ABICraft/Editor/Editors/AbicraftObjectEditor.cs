using AbicraftMonos;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AbicraftNodes.Editors
{
    [CustomEditor(typeof(AbicraftObject))]
    public class AbicraftObjectEditor : AbicratEditor
    {
        int instantiatePoolAmount;

        public override void OnInspectorGUI()
        {
            var abicraftObject = target as AbicraftObject;

            abicraftObject.Profile = EditorGUILayout.ObjectField(abicraftObject.Profile, typeof(AbicraftObjectProfile), false) as AbicraftObjectProfile;

            abicraftObject.InstantiateObjectToPool = GUILayout.Toggle(abicraftObject.InstantiateObjectToPool, "Instantiate Object To Pool");

            if (abicraftObject.InstantiateObjectToPool)
                abicraftObject.InstantiateToPoolAmount = EditorGUILayout.IntField(abicraftObject.InstantiateToPoolAmount);
        
            if (GUI.changed)
            {
                EditorUtility.SetDirty(target);
            }
        }
    }
}
