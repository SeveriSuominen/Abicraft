using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static AbicraftNodeEditor.NodeEditor;
using AbicraftNodeEditor;
using AbicraftNodes.Action;
using UnityEditor;
using AbicraftCore;
using AbicraftMonos;

namespace AbicraftNodes.Editors
{
    [CustomNodeEditor(typeof(CastStatesNode))]
    public class CastStateEditor : NodeEditor
    {
        public Texture2D icon;

        CastStatesNode node;
        AbicraftObject obj;
        Animator animator;

        int selectedIndex;

        public override void OnHeaderGUI(GUIStyle style)
        {
            // Initialization
            if (node == null)
            {
                node = target as CastStatesNode;
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

            node = target as CastStatesNode;

            if (!node.abicraft)
                node.abicraft = AbicraftGlobalContext.abicraft;

            var abicraft = node.abicraft;

            List<GUIContent> avaibleStateContents = new List<GUIContent>();
            List<AbicraftState.StateType> types = new List<AbicraftState.StateType>();

            Texture2D icon = null;

            if (abicraft)
            {
                avaibleStateContents.Add(new GUIContent("None"));
                types.Add(AbicraftState.StateType.Neutral);

                for (int i = 0; i < abicraft.dataFile.GlobalStates.Count; i++)
                {
                    AbicraftState state = abicraft.dataFile.GlobalStates[i];
                    avaibleStateContents.Add(new GUIContent(state.StateName, state.icon));
                    types.Add(state.type);
                }

                //icon = abicraft.dataFile.GlobalStates[node.selectedIndex].icon;
            }

            if (icon != null)
            {
                Color guiDefColorBg = GUI.backgroundColor;
                GUI.backgroundColor = new Color(0, 0, 0, 0);

                //GUILayout.Box(icon, GUILayout.Width(NodeEditor.width - icon.width * 0.5f), GUILayout.Height(NodeEditor.width * 0.33f));

                GUI.backgroundColor = guiDefColorBg;
            }

            base.OnBodyGUI();

            GuiSpace(5);
            GuiLine(1);
            GuiSpace(5);

            EditorGUIUtility.SetIconSize(new Vector2(16, 16));
            GUIStyle gstyle = new GUIStyle(GUI.skin.GetStyle("HelpBox"));
            if (abicraft)
            {
                for (int i = 0; i < node.allSelectedIndices.Count; i++)
                {
                    GUILayout.BeginHorizontal(gstyle);
                  

                    GUIStyle styleState = new GUIStyle();
                    styleState.fontSize = 13;

                    if (types[node.allSelectedIndices[i]].Equals(AbicraftState.StateType.Negative))
                        styleState.normal.textColor = new Color32(255, 38, 0, 255);
                    if (types[node.allSelectedIndices[i]].Equals(AbicraftState.StateType.Positive))
                        styleState.normal.textColor = Color.green;
                    if (types[node.allSelectedIndices[i]].Equals(AbicraftState.StateType.Neutral))
                        styleState.normal.textColor = Color.yellow;

                    GUILayout.Label(avaibleStateContents[node.allSelectedIndices[i]], styleState);

                    if(GUILayout.Button("X", GUILayout.MaxWidth(15), GUILayout.MaxHeight(15)))
                        node.allSelectedIndices.RemoveAt(i);

                    GUILayout.EndHorizontal();

                    GUIStyle styleLabel = new GUIStyle();
                    styleLabel.normal.textColor = Color.white;

                    GuiSpace(5);

                    GUILayout.Label("Cast for seconds", styleLabel);
                    EditorGUILayout.FloatField(2);

                    if(i != node.allSelectedIndices.Count - 1)
                    {
                        GuiSpace(5);
                        GuiLine(1);
                        GuiSpace(5);
                    }
                }
            }

            GuiSpace(5);
            GuiLine(1);
            GuiSpace(5);

            EditorGUIUtility.SetIconSize(Vector2.zero);

            GUILayout.Label("Available States", NodeEditorGUILayout.GetFieldStyle("In"));
            node.selectedIndex = EditorGUILayout.Popup(node.selectedIndex, avaibleStateContents.ToArray()); //selectedIndex = EditorGUILayout.Popup(selectedIndex, strings);

            if (node.selectedIndex != node.lastIndex)
            {
                if (node.selectedIndex != 0)
                {
                    if (!node.allSelectedIndices.Contains(node.selectedIndex))
                        node.allSelectedIndices.Add(node.selectedIndex);

                    node.lastIndex = node.selectedIndex;
                }
            }
        }
    }
}
