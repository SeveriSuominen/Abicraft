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

namespace AbicraftNodes.Editors
{
    [CustomNodeEditor(typeof(VectorNode))]
    public class VectorEditor : NodeEditor
    {
        VectorNode node;

        int lastSelectedIndex;
   

        public override void OnHeaderGUI(GUIStyle style)
        {
            // Initialization
            if (node == null)
            {
                node = target as VectorNode;
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
            node = target as VectorNode;

            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("VectorIn"),  new GUIContent("Vector"));
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("VectorOut"), new GUIContent("Vector"));

            //base.OnBodyGUI();
            /*if(lastSelectedIndex != node.selectedIndex)
            {
                node.selectedVariable = variableNames[node.selectedIndex];

                // SHOULD BE MAX ONE
                List <NodePort> portConnections = new List<NodePort>();  
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
            }*/

            GuiSpace(5);
            GuiLine(1);
            GuiSpace(5);

            var connected = node.GetInputPort("VectorIn").IsConnected;

            if (connected)
            {
                NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("XOut"), new GUIContent("X"));
                NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("YOut"), new GUIContent("X"));
                NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("ZOut"), new GUIContent("Z"));
            }
            else
            {
                NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("XIn"), new GUIContent("X"));
                NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("YIn"), new GUIContent("X"));
                NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("ZIn"), new GUIContent("Z"));
            }
        }
    }
}
