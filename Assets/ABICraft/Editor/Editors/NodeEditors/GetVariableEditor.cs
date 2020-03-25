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

            if(node.selectedIndex == 0)
            {
                GUI.color = Color.red;
                GUI.DrawTexture(dotRect, NodeEditorResources.dot);
                GUI.color = Color.white;
            }
        }

        public override void OnBodyGUI()
        {
       
            GuiSpace(10);
            node = target as GetVariableNode;

            if (node.GetGlobalVariable != node.lastGetGlobalVariableSetting)
            {
                node.selectedIndex = 0;
                node.lastGetGlobalVariableSetting = node.GetGlobalVariable;
            }

            List<string> variableNames = new List<string>();
            List<Type>   variableTypes = new List<Type>();

            variableNames.Add("None");
            variableTypes.Add(typeof(object));

            var variableDefinitions = node.GetGlobalVariable ? AbicraftGlobalContext.abicraft.dataFile.GlobalVariableDefinitions : node.graph.variableDefinitions;

            for (int i = 0; i < variableDefinitions.Count; i++)
            {
                variableNames.Add(variableDefinitions[i].VARIABLE_NAME);
                variableTypes.Add(variableDefinitions[i].VARIABLE_TYPE);
            }

            GUIStyle styleW = new GUIStyle(EditorStyles.popup);
            styleW.normal.textColor = node.selectedIndex == 0 ? Color.red : Color.black;

            Color col = GUI.color;
            if (node.selectedIndex == 0)
            {
                GUI.color = ERRORCOLOR;
            }

            node.selectedIndex = EditorGUILayout.Popup(node.selectedIndex, variableNames.ToArray(), styleW);
            GUI.color = col;

            if(lastSelectedIndex != node.selectedIndex)
            {
                // SHOULD BE MAX ONE
                List<NodePort> portConnections = new List<NodePort>();  
                foreach(var dport in node.DynamicOutputs)
                {
                    portConnections = dport.GetConnections();
                }

                node.ClearDynamicPorts();

                node.AddDynamicOutput(variableTypes[node.selectedIndex], AbicraftNode.ConnectionType.Multiple, AbicraftNode.TypeConstraint.None, "Value");
                NodePort port = node.GetOutputPort("Value");

                for (int i = 0; i < portConnections.Count; i++)
                {
                    port.Connect(portConnections[i]);
                }
               
                lastSelectedIndex = node.selectedIndex;
            }

            GuiSpace(5);
            GuiLine(1);
            GuiSpace(5);

            base.OnBodyGUI();
        }
    }
}
