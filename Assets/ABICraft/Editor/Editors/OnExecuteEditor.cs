using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static AbicraftNodeEditor.NodeEditor;
using AbicraftNodeEditor;
using AbicraftNodes;

[CustomNodeEditor(typeof(OnExecuteNode))]
public class OnExecuteEditor : NodeEditor
{
    public Texture2D icon;

    OnExecuteNode node;
    public override void OnHeaderGUI(GUIStyle style)
    {
        // Initialization
        if (node == null)
        {
            node = target as OnExecuteNode;
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
        if (icon == null)
            icon = Resources.Load("Icons/baseline_play_arrow_white_18dp") as Texture2D;

        if (node == null) node = target as OnExecuteNode;

        // Update serialized object's representation
        serializedObject.Update();


        // if(node.icon != null)
        // GUI.DrawTexture(new Rect(10, 10, 60, 60), node.icon, ScaleMode.ScaleToFit, true, 10.0F);
        
        GUILayout.Space(15);
        NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("Out"), GUIContent.none);
        GUILayout.Space(15);
        Rect dotRect = new Rect(80,35,0,0);
        dotRect.size = new Vector2(50, 50);

        GUI.color = Color.white;
        GUI.DrawTexture(dotRect, icon);
        GUI.color = Color.white;
   
        // Apply property modifications
        serializedObject.ApplyModifiedProperties();
    }
}
