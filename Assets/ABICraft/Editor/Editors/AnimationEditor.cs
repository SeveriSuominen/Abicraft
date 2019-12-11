using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static XNodeEditor.NodeEditor;
using XNodeEditor;
using AbicraftNodes.Action;

[CustomNodeEditor(typeof(AnimationNode))]
public class AnimationEditor : NodeEditor
{
    public Texture2D icon;

    AnimationNode node;
    public override void OnHeaderGUI()
    {   
        // Initialization
        if (node == null)
        {
            node = target as AnimationNode;
        }

        base.OnHeaderGUI();
        Rect dotRect = GUILayoutUtility.GetLastRect();
        dotRect.size = new Vector2(16, 16);
        dotRect.y += 6;
        
        GUI.color =  node.data.statusColor;
        GUI.DrawTexture(dotRect, NodeEditorResources.dot);
        GUI.color = Color.white;
    }
}
