using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using static AbicraftNodeEditor.NodeEditor;
using AbicraftNodeEditor;
using AbicraftNodes.Action;

[CustomNodeEditor(typeof(RotateToDirectionNode))]
public class RotateToCursorEditor : NodeEditor
{
    public Texture2D icon;

    RotateToDirectionNode node;
    public override void OnHeaderGUI()
    {   
        // Initialization
        if (node == null)
        {
            node = target as RotateToDirectionNode;
        }

        base.OnHeaderGUI();

        Rect dotRect = GUILayoutUtility.GetLastRect();
        dotRect.size = new Vector2(16, 16);
        dotRect.y += 6;
    }
}
