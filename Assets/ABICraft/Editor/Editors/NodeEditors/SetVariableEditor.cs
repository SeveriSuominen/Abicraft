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
    [CustomNodeEditor(typeof(SetVariableNode))]
    public class SetVariableEditor : NodeEditor
    {
        SetVariableNode node;

        int lastSelectedIndex;

        public override void OnHeaderGUI(GUIStyle style)
        {
            // Initialization
            if (node == null)
            {
                node = target as SetVariableNode;
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
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("In"),  new GUIContent("In" ));
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("Out"), new GUIContent("Out"));

            node = target as SetVariableNode;

            GuiSpace(5);
            GuiLine(1);
            GuiSpace(5);

            GUIStyle style = new GUIStyle();
            style.normal.textColor = Color.white;
            style.fontSize = 12;

            Color col = GUI.color; 

            if (node.SetGlobalVariable && node.definitionsNeedUpdating)
            {
                GUI.color = ERRORCOLOR;
            }

            GUILayout.Label("Variable name", style);
            GuiSpace(5);
            node.VariableName = EditorGUILayout.TextField(node.VariableName);

            GuiSpace(5);
            GuiLine (1);
            GuiSpace(5);

            GUILayout.Label("Set Global Variable", style);
            GuiSpace(5);
            node.SetGlobalVariable = EditorGUILayout.Toggle(node.SetGlobalVariable, GUILayout.Width(20));

            if(node.VariableName != node.lastVariableName)
            {
                node.definitionsNeedUpdating = true;
                node.lastVariableName = node.VariableName;
            }

            if (node.SetGlobalVariable != node.lastGlobalVariableSetting)
            {
                node.definitionsNeedUpdating = true;
                node.lastGlobalVariableSetting = node.SetGlobalVariable;
            }

            GUI.color = col;

            GuiSpace(5);
            GuiLine(1);
            GuiSpace(5);

            if (node.SetGlobalVariable && node.definitionsNeedUpdating)
            {
                GUIStyle gstyle = new GUIStyle(GUI.skin.GetStyle("HelpBox"));
                gstyle.normal.textColor = Color.white;
                gstyle.fontSize = 11;

                Helpbox("Global variable definitions needs to be updated after changes", MessageType.Warning);

                if (GUILayout.Button("Update"))
                {
                    AbicraftGlobalContext.UpdateGlobalVariableDefinitions();
                    node.definitionsNeedUpdating = false;
                }
                GuiSpace(5);
                GuiLine(1);
                GuiSpace(5);
            }

            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("Value"), new GUIContent("Value"));
            // base.OnBodyGUI();
        }
    }
}
