using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static AbicraftNodeEditor.NodeEditor;

using AbicraftNodeEditor;
using AbicraftNodes.Action;

namespace AbicraftNodes.Editors
{
    [CustomNodeEditor(typeof(RotateToDirectionNode))]
    public class RotateToCursorEditor : NodeEditor
    {
        public Texture2D icon;

        RotateToDirectionNode node;
        public override void OnHeaderGUI(GUIStyle style)
        {
            // Initialization
            if (node == null)
            {
                node = target as RotateToDirectionNode;
            }

            base.OnHeaderGUI(style);

            Rect dotRect = GUILayoutUtility.GetLastRect();
            dotRect.size = new Vector2(16, 16);
            dotRect.y += 6;
        }
    }
}
