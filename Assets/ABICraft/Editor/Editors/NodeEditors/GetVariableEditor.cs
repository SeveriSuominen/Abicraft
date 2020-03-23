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
    [CustomNodeEditor(typeof(GetVariableNode))]
    public class GetVariableEditor : NodeEditor
    {
        GetVariableNode node;

        int lastSelectedIndex;

        public override void OnHeaderGUI(GUIStyle style)
        {
            // Initialization
            if (node == null)
            {
                node = target as GetVariableNode;
            }

            base.OnHeaderGUI(style);

            Rect dotRect = GUILayoutUtility.GetLastRect();
            dotRect.size = new Vector2(16, 16);
            dotRect.y += 6;

            /*GUI.color =  node.data.statusColor;
            GUI.DrawTexture(dotRect, NodeEditorResources.dot);
            GUI.color = Color.white;*/
        }

        public override void OnBodyGUI()
        {

            node = target as GetVariableNode;

            List<string> variableNames = new List<string>();
            List<Type>   variableTypes = new List<Type>();

            for (int i = 0; i < node.graph.variableDefinitions.Count; i++)
            {
                variableNames.Add(node.graph.variableDefinitions[i].VARIABLE_NAME);
                variableTypes.Add(node.graph.variableDefinitions[i].VARIABLE_TYPE);
            }

            node.selectedIndex = EditorGUILayout.Popup(node.selectedIndex, variableNames.ToArray());

            if(lastSelectedIndex != node.selectedIndex)
            {
                node.ClearDynamicPorts();
                node.AddDynamicOutput(variableTypes[node.selectedIndex], AbicraftNode.ConnectionType.Multiple, AbicraftNode.TypeConstraint.None, "Value");

                lastSelectedIndex = node.selectedIndex;
            }

            base.OnBodyGUI();
        }
    }
}
