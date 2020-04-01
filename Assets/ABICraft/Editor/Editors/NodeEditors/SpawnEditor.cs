using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using AbicraftNodeEditor;
using AbicraftNodes.Action;
using UnityEditor;
using AbicraftCore;
using AbicraftMonos;
using System;
using AbicraftNodes.Meta;

using AbicraftNodeEditor;
using AbicraftNodes.Meta;

namespace AbicraftNodes.Editors
{
    [CustomNodeEditor(typeof(SpawnNode))]
    public class SpawnEditor : NodeEditor
    {
        SpawnNode node;

        int lastSelectedIndex;

        public override void OnHeaderGUI(GUIStyle style)
        {
            // Initialization
            if (node == null)
            {
                node = target as SpawnNode;
            }

            base.OnHeaderGUI(style);

            Rect dotRect = GUILayoutUtility.GetLastRect();
            dotRect.size = new Vector2(16, 16);
            dotRect.y += 6;

            if(false)
            {
                GUI.color = Color.red;
                GUI.DrawTexture(dotRect, NodeEditorResources.dot);
                GUI.color = Color.white;
            }
        }

        public override void OnBodyGUI()
        {
            GUILayout.BeginHorizontal();
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("In"),  new GUIContent("In" ));
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("Out"), new GUIContent("Out"));
            GUILayout.EndHorizontal();
            
            node = target as SpawnNode;

            GuiSpace(5);
            GuiLine(1);
            GuiSpace(5);

            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("SpawnedObject"), new GUIContent("Spawned Object"));

            GuiSpace(5);
            GuiLine(1);
            GuiSpace(5);

            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("ObjectToSpawn"), new GUIContent("Object To Spawn"));

            GUIStyle style = new GUIStyle();
            style.normal.textColor = Color.white;
            style.fontSize = 12;

            GUIStyle poolbtnstyle = new GUIStyle(EditorStyles.miniButton);
            
            if (AbicraftGlobalContext.HasValidAbicraftInstance() && node.ObjectToSpawn)
            {
                bool isPooled = false;
                int  inIndex = -1;

                for (int i = 0; i < AbicraftGlobalContext.abicraft.InstantiateToPool.Count; i++)
                {
                    if (AbicraftGlobalContext.abicraft.InstantiateToPool[i].abjRef == node.ObjectToSpawn)
                    {
                        isPooled = true;
                        inIndex  = i;
                        break;
                    }
                }

                poolbtnstyle.normal.textColor = isPooled ? new Color(0.9f, 0.6f, 0) : new Color(0, 0.8f, 0);
                Color bgcolor = GUI.backgroundColor;
                GuiSpace(5);
                if (!isPooled)
                {
                    GUI.backgroundColor = new Color(0.2f, 0.2f, 0.2f);

                    if (GUILayout.Button("Attach to pool", poolbtnstyle))
                    {
                        AbicraftGlobalContext.abicraft.InstantiateToPool.Add(
                            AbicraftObjectPoolInstantiate.Create(node.ObjectToSpawn)
                        );
                        EditorUtility.SetDirty(AbicraftGlobalContext.abicraft);
                    }
                    GUI.backgroundColor = bgcolor;

                    Helpbox("Object not pooled, its recommended to pool object for better performance", MessageType.Warning);
                }
                else
                {
                    GUI.backgroundColor = new Color(0.2f, 0.2f, 0.2f);
                    if (GUILayout.Button("Detach from pool", poolbtnstyle))
                    {
                        AbicraftGlobalContext.abicraft.InstantiateToPool.RemoveAt(inIndex);
                    }
                    GUI.backgroundColor = bgcolor;

                    GuiSpace(5);
                }
            }
            GuiLine(1);
            GuiSpace(5);
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("spawnPosition"), new GUIContent("Spawn Position"));
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("spawnRotation"), new GUIContent("Spawn Rotation"));
        }
    }
}
