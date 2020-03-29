using AbicraftCore;
using AbicraftMonos;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AbicraftNodes.Editors
{
    [CustomEditor(typeof(AbicraftObject))]
    public class AbicraftObjectEditor : AbicratInspectorEditor
    {
        int instantiatePoolAmount;

        public override void OnInspectorGUI()
        {
            GUIStyle gstyle = new GUIStyle(GUI.skin.GetStyle("HelpBox"));
            GUIStyle bstyle = new GUIStyle(EditorStyles.boldLabel);
            GUIStyle poolbtnstyle = new GUIStyle(EditorStyles.miniButton);
   
            var abicraftObject = target as AbicraftObject;

            GUILayout.BeginVertical();
            GuiSpace(5);
            GUILayout.Label("Profile", bstyle);
            GuiSpace(5);

            abicraftObject.Profile = EditorGUILayout.ObjectField(abicraftObject.Profile, typeof(AbicraftObjectProfile), false) as AbicraftObjectProfile;

            if (AbicraftGlobalContext.abicraft)
            {
                GuiSpace(5);
                GuiLine (1);
                GuiSpace(5);

                GUILayout.BeginHorizontal();
                GUILayout.Label("Pooling options", bstyle);

                bool isPooled = false;
                int  indexAt  = -1;

                for (int i = 0; i < AbicraftGlobalContext.abicraft.InstantiateToPool.Count; i++)
                {
                    if (AbicraftGlobalContext.abicraft.InstantiateToPool[i].abjRef == abicraftObject)
                    {
                        isPooled = true;
                        indexAt  = i;
                        break;
                    } 
                }

                poolbtnstyle.normal.textColor = isPooled ? new Color(0.75f, 0, 0) : new Color(0, 0.6f, 0);

                if (isPooled)
                {
                    if (GUILayout.Button("Detach from pool", poolbtnstyle))
                    {
                        AbicraftGlobalContext.abicraft.InstantiateToPool.RemoveAt(indexAt);
                    }
                }
                else
                {
                    if (GUILayout.Button("Attach to pool",  poolbtnstyle))
                    {                      
                        AbicraftGlobalContext.abicraft.InstantiateToPool.Add (
                            AbicraftObjectPoolInstantiate.Create(abicraftObject)
                        );
                        EditorUtility.SetDirty(AbicraftGlobalContext.abicraft);
                    }
                }
                GUILayout.EndHorizontal();


                GuiSpace(5);
                GUILayout.Label("You can pool objects by setuping them to your Abicraft monobehaviour", gstyle);


                if (!isPooled)
                {
                    GUI.enabled = false;
                    abicraftObject.InstantiateObjectToPool = GUILayout.Toggle(isPooled, "Instantiate Object To Pool");
                    GUI.enabled = true;

                    GUILayout.Label("Amount");
                    abicraftObject.InstantiateToPoolAmount = EditorGUILayout.IntField(abicraftObject.InstantiateToPoolAmount);
                }
                else
                {
                    GUI.enabled = false;
                    abicraftObject.InstantiateObjectToPool = GUILayout.Toggle(isPooled, "Instantiate Object To Pool");

                    GUILayout.Label("Amount");
                    abicraftObject.InstantiateToPoolAmount = EditorGUILayout.IntField(abicraftObject.InstantiateToPoolAmount);
                    GUI.enabled = true; 
                }
                GuiSpace(5);
            }

            GUILayout.EndVertical();

            if (GUI.changed)
            {
                EditorUtility.SetDirty(target);
            }
        }
    }
}
