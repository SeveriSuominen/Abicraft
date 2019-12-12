using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static XNodeEditor.NodeEditor;
using XNodeEditor;
using AbicraftNodes.Action;
using UnityEditor;

[CustomNodeEditor(typeof(AnimationNode))]
public class AnimationEditor : NodeEditor
{
    public Texture2D icon;

    AnimationNode node;
    AbicraftObject obj;
    Animator animator;

    int selectedIndex;

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
        
        /*GUI.color =  node.data.statusColor;
        GUI.DrawTexture(dotRect, NodeEditorResources.dot);
        GUI.color = Color.white;*/
    }

    public override void OnBodyGUI()
    {
        // Initialization
        if (node == null)
        {
            node = target as AnimationNode;
        }

        if (node.data == null)
        {
            node.data = new AnimationNode.NodeData();
        }

        if (obj == null)
        {
            obj = node.GetInputValue<AbicraftObject>("Obj");
        }

        base.OnBodyGUI();

        if (obj != null)
        {
            if (animator == null)
            {
                animator = obj.GetComponent<Animator>();
            }

            if (animator != null)
            {
                node.data.clips = animator.runtimeAnimatorController.animationClips;

                string[] strings = new string[node.data.clips.Length];

                for (int i = 0; i < node.data.clips.Length; i++)
                {
                    strings[i] = node.data.clips[i].name;
                }
               
                GUILayout.Label("Available Animations", NodeEditorGUILayout.GetFieldStyle("In"));
                node.data.selectedIndex = selectedIndex = EditorGUILayout.Popup(selectedIndex, strings);
            }
        }
    }
}
