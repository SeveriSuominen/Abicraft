using AbicraftCore;
using AbicraftMonos;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AbicraftNodes.Editors
{
    [CustomEditor(typeof(Abicraft))]
    public class AbicraftEditor : AbicratInspectorEditor
    {
        Abicraft abicraft;

        public override void OnInspectorGUI()
        {
            if (!abicraft)
                abicraft = target as Abicraft;

            GUIStyle gstyle = new GUIStyle(GUI.skin.GetStyle("HelpBox"));
            GUIStyle bstyle = new GUIStyle(EditorStyles.boldLabel);
            GUIStyle poolbtnstyle = new GUIStyle(EditorStyles.miniButton);
            poolbtnstyle.normal.textColor = new Color(0.7f, 0, 0);

            GuiSpace(5);
            GUILayout.Label("Abicraft global data file", bstyle);
            GuiSpace(5);

            abicraft.dataFile = EditorGUILayout.ObjectField(abicraft.dataFile, typeof(AbicraftGlobalDataFile), false) as AbicraftGlobalDataFile;

            GuiSpace(5);
            GuiLine(1);
            GuiSpace(5);

            if(abicraft.InstantiateToPool.Count > 0)
                GUILayout.Label("Abicraft object pool", bstyle);

            for (int i = 0; i < abicraft.InstantiateToPool.Count; i++)
            {
                if(abicraft.InstantiateToPool[i] != null)
                {
                    if (abicraft.InstantiateToPool[i].abjRef != null)
                    {
                        try
                        {
                            GUILayout.BeginHorizontal(gstyle);

                            GUI.enabled = abicraft.InstantiateToPool[i].includeForScene;

                            if (GUILayout.Button("Detach", poolbtnstyle))
                            {
                                abicraft.InstantiateToPool.RemoveAt(i);
                            }

                            GUILayout.Label(abicraft.InstantiateToPool[i].abjRef.name, GUILayout.Width(120));

                            GuiSpace(5);
                            GUILayout.Label("Amount", GUILayout.Width(50));
                            abicraft.InstantiateToPool[i].abjRef.InstantiateToPoolAmount = abicraft.InstantiateToPool[i].amountForScene = EditorGUILayout.IntField(abicraft.InstantiateToPool[i].amountForScene, GUILayout.Width(40));
                            GUI.enabled = true;

                            abicraft.InstantiateToPool[i].includeForScene = GUILayout.Toggle(abicraft.InstantiateToPool[i].includeForScene, "Include");

                            GUILayout.EndHorizontal();
                        }
                        catch (ArgumentException e)
                        {

                        }
                    }
                    else
                    {
                        abicraft.InstantiateToPool.RemoveAt(i);
                        EditorUtility.SetDirty(target);
                    }
                }
            }

            if (GUI.changed)
            {
                EditorUtility.SetDirty(target);
            }
        }
    }
}

