using AbicraftCore;
using AbicraftNodeEditor;
using AbicraftNodes.Math;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomNodeEditor(typeof(TowardsNode))]
public class TowardsEditor : NodeEditor
{
    TowardsNode node;


    public override void OnHeaderGUI(GUIStyle style)
    {
        // Initialization
        if (node == null)
        {
            node = target as TowardsNode;
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
        node = target as TowardsNode;

        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.white;
        style.margin = new RectOffset(0, 0, 0, 0);

        GUIStyle autostyle = new GUIStyle(EditorStyles.boldLabel);
        autostyle.normal.textColor = Color.yellow;
        autostyle.margin = new RectOffset(0, 0, 0, 0);
        autostyle.fontSize = 11;


        GUIStyle popstyle = new GUIStyle(EditorStyles.popup);
        popstyle.margin = new RectOffset(0, 0, 0, 0);
        GuiSpace(5);
        GUILayout.BeginHorizontal();
        GUILayout.Label("From Origin", style, GUILayout.Width(80));
        node.originMode = (TowardsNode.Mode)EditorGUILayout.EnumPopup(node.originMode, popstyle);
        GUILayout.EndHorizontal();

        GuiSpace(5);
        GUILayout.BeginHorizontal();
        GUILayout.Label("Towards", style, GUILayout.Width(80));
        node.alt_name = "Towards " + (node.towardsMode = (TowardsNode.Mode)EditorGUILayout.EnumPopup(node.towardsMode, popstyle)).ToString();
        node.use_alt_name = true;
        GUILayout.EndHorizontal();

        GuiSpace(5);
        GuiLine(1);
        GuiSpace(5);

        GUILayout.BeginHorizontal();
        GUILayout.BeginVertical();

        switch (node.originMode)
        {
            case TowardsNode.Mode.Cursor:
                GUILayout.Label("From Cursor", autostyle);
                GuiSpace(5);
                break;

            case TowardsNode.Mode.Object:
                NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("Origin"), new GUIContent("From Obj"));
                break;

            case TowardsNode.Mode.Position:
                NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("OriginPosition"), new GUIContent("From Position"));
                break;
        }
     
        switch (node.towardsMode)
        {
            case TowardsNode.Mode.Cursor:
                GUILayout.Label("Towards Cursor", autostyle);
                GuiSpace(5);
                break;
            case TowardsNode.Mode.Object:
                NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("Towards"), new GUIContent("Towards Obj"));
                break;
            case TowardsNode.Mode.Position:
                NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("TowardsPosition"), new GUIContent("Towards Position"));
                break;
        }

        GUILayout.EndVertical();

        GUILayout.BeginVertical();

        NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("Direction"), new GUIContent("Direction"));
        NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("Distance"),  new GUIContent("Distance"));

        GUILayout.EndVertical();
        GUILayout.EndHorizontal();

        if(node.towardsMode == TowardsNode.Mode.Cursor)
            Helpbox("Cursor world position is automatically calculated from camera", MessageType.Info);
    }
}
