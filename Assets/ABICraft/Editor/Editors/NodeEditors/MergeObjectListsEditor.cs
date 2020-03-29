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
using AbicraftNodes.Math;
using AbicraftNodes.Object;

namespace AbicraftNodes.Editors
{
    [CustomNodeEditor(typeof(MergeObjectListsNode))]
    public class MergeObjectListsEditor : NodeEditor
    {
        MergeObjectListsNode node;

        int lastSelectedIndex;
   

        public override void OnHeaderGUI(GUIStyle style)
        {
            // Initialization
            if (node == null)
            {
                node = target as MergeObjectListsNode;
            }

            base.OnHeaderGUI(style);

            Rect dotRect = GUILayoutUtility.GetLastRect();
            dotRect.size = new Vector2(16, 16);
            dotRect.y += 6;

            /*
            GUI.color = Color.red;
            GUI.DrawTexture(dotRect, NodeEditorResources.dot);
            GUI.color = Color.white;*/
        }

        public override void OnBodyGUI()
        {
            GuiSpace(10);
            node = target as MergeObjectListsNode;

            GUILayout.BeginHorizontal();
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("ObjectList"),    new GUIContent("Lists"));
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("ObjectListOut"), new GUIContent("Merged"));
            GUILayout.EndHorizontal();
        }
    }
}
