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
            GUIStyle gstyle = new GUIStyle(GUI.skin.GetStyle("HelpBox"));
            gstyle.normal.textColor = Color.white;

            GuiSpace(10);
            node = target as GetVariableNode;

            if (node.GetGlobalVariable != node.lastGetGlobalVariableSetting)
            {
                node.selectedIndex = 0;
                node.lastGetGlobalVariableSetting = node.GetGlobalVariable;
            }

            bool hasDataFile = true;

            List<string> variableNames = new List<string>();
            List<Type>   variableTypes = new List<Type>();

            variableNames.Add("None");
            variableTypes.Add(typeof(object));

            List<AbicraftCore.Variables.AbicraftAbilityVariableDefinition> variableDefinitions;

            if (AbicraftGlobalContext.HasValidAbicraftInstance())
            {
                variableDefinitions = node.GetGlobalVariable ? AbicraftGlobalContext.abicraft.dataFile.GlobalVariableDefinitions : node.graph.variableDefinitions;
            }
            else if(!node.GetGlobalVariable)
            {
                variableDefinitions = node.graph.variableDefinitions;
                hasDataFile = false;
            }
            else
            {
                variableDefinitions = new List<AbicraftCore.Variables.AbicraftAbilityVariableDefinition>();
                hasDataFile = false;
            }

            for (int i = 0; i < variableDefinitions.Count; i++)
            {
                if (node.GetGlobalVariable)
                {
                    if(variableDefinitions[i].global_owner_ability == node.getFromAbilityGlobal)
                    {
                        variableNames.Add(variableDefinitions[i].VARIABLE_NAME);
                        variableTypes.Add(variableDefinitions[i].VARIABLE_TYPE);
                    }
                }
                else
                {
                    variableNames.Add(variableDefinitions[i].VARIABLE_NAME);
                    variableTypes.Add(variableDefinitions[i].VARIABLE_TYPE);
                }
            }

            GUIStyle styleW = new GUIStyle(EditorStyles.popup);
            styleW.normal.textColor = node.selectedIndex == 0 ? Color.red : Color.black;

            if (!BuildPipeline.isBuildingPlayer && node.lastVariableCount != variableNames.Count)
            {
                if (node.selectedIndex > variableNames.Count - 1)
                    node.selectedIndex = 0;
                else
                {
                    bool resetted = false;

                    for (int i = 0; i < variableNames.Count; i++)
                    {
                        if (node.selectedVariable != null && node.selectedVariable.Equals(variableNames[i]))
                        {
                            node.selectedIndex = i;
                            resetted = true;
                            break;
                        }
                    }
                    if (!resetted)
                        node.selectedIndex = 0;
                }
                node.lastVariableCount = variableNames.Count;
            }

            if(node.GetGlobalVariable && AbicraftGlobalContext.HasValidAbicraftInstance())
            {
                List<string> ability_names = new List<string>();
                List<AbicraftAbility> abilities = new List<AbicraftAbility>();

                ability_names.Add("None");
                abilities.Add(null);

                for (int i = 0; i < AbicraftGlobalContext.abicraft.dataFile.AllAbilityGraphs.Count; i++)
                {
                    ability_names.Add(AbicraftGlobalContext.abicraft.dataFile.AllAbilityGraphs[i].AbilityName);
                    abilities.Add(AbicraftGlobalContext.abicraft.dataFile.AllAbilityGraphs[i]);
                }

                int indexofability = 0;

                if (node.getFromAbilityGlobal)
                {
                    indexofability = ability_names.IndexOf(node.getFromAbilityGlobal.AbilityName);

                    if (indexofability < 0 || indexofability > ability_names.Count - 1)
                        indexofability = 0;
                }
                node.getFromAbilityGlobal = abilities[EditorGUILayout.Popup(indexofability, ability_names.ToArray())];
            }

            if(node.getFromAbilityGlobal != node.lastSelectedAbilityGlobal)
            {
                node.selectedIndex = 0;
                node.lastSelectedAbilityGlobal = node.getFromAbilityGlobal;
            }

            GuiSpace(5);

            Color col = GUI.color;
            if (node.selectedIndex == 0)
            {
                GUI.color = ERRORCOLOR;
            }

            node.selectedIndex = EditorGUILayout.Popup(node.selectedIndex, variableNames.ToArray(), styleW);
            GUI.color = col;

            if (node.GetGlobalVariable && !hasDataFile)
            {
                Helpbox("Could not fetch global variables, abicraft data file reference missing", MessageType.Error);
            }

            if(lastSelectedIndex != node.selectedIndex)
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
            }

            GuiSpace(5);
            GuiLine(1);
            GuiSpace(5);

            base.OnBodyGUI();

            if (GUI.changed)
            {
                EditorUtility.SetDirty(target);
            }
        }
    }
}
