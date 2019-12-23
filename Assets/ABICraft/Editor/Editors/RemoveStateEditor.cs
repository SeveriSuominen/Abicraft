using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static AbicraftNodeEditor.NodeEditor;
using AbicraftNodeEditor;
using AbicraftNodes.Action;
using UnityEditor;
using AbicraftCore;

[CustomNodeEditor(typeof( RemoveStatesNode))]
public class RemoveStateEditor : NodeEditor
{
    public Texture2D icon;

    RemoveStatesNode node;
    AbicraftObject obj;
    Animator animator;

    int selectedIndex;

    public override void OnHeaderGUI(GUIStyle style)
    {   
        // Initialization
        if (node == null)
        {
            node = target as  RemoveStatesNode;
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
        node = target as  RemoveStatesNode;

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
                avaibleStateContents.Add(new GUIContent(state.name, state.icon));
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

        if(node.RemoveMode == RemoveStatesNode.RemoveStatesMode.Selected)
        {
            GuiSpace(5);
            GuiLine(1);
            GuiSpace(5);
            EditorGUIUtility.SetIconSize(new Vector2(16, 16));

            if (abicraft)
            {
                for (int i = 0; i < node.allSelectedIndices.Count; i++)
                {
                    GUIStyle style = new GUIStyle();
                    if (types[node.allSelectedIndices[i]].Equals(AbicraftState.StateType.Negative))
                        style.normal.textColor = new Color32(255, 38, 0, 255);
                    if (types[node.allSelectedIndices[i]].Equals(AbicraftState.StateType.Positive))
                        style.normal.textColor = Color.green;
                    if (types[node.allSelectedIndices[i]].Equals(AbicraftState.StateType.Neutral))
                        style.normal.textColor = Color.yellow;

                    GUILayout.Label(avaibleStateContents[node.allSelectedIndices[i]], style);
                }
            }

            EditorGUIUtility.SetIconSize(Vector2.zero);
            GuiSpace(5);
            GuiLine(1);
            GuiSpace(5);
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


    void GuiLine(int i_height = 1)
    {
        Rect rect = EditorGUILayout.GetControlRect(false, i_height);

        rect.height = i_height;

        EditorGUI.DrawRect(rect, new Color(0.3f, 0.3f, 0.3f, 1));
    }

    void GuiSpace(int i_height = 1)
    {
        Rect rect = EditorGUILayout.GetControlRect(false, i_height);

        rect.height = i_height;

        EditorGUI.DrawRect(rect, new Color(0,0,0,0) );
    }
}
