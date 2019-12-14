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
        node = target as AnimationNode;

        obj = node.GetInputValue<AbicraftObject>("Obj");

        base.OnBodyGUI();

        if(obj != null)
        {
            node.clips = obj.GetComponent<Animator>().runtimeAnimatorController.animationClips;

            string[] strings;

            if (node.clips == null)
                strings = new string[0];
            else
                strings = new string[node.clips.Length];

            for (int i = 0; i < node.clips.Length; i++)
            {
                strings[i] = node.clips[i].name;
            }

            GUILayout.Label("Available Animations", NodeEditorGUILayout.GetFieldStyle("In"));
            node.selectedIndex = EditorGUILayout.Popup(node.selectedIndex, strings); //selectedIndex = EditorGUILayout.Popup(selectedIndex, strings);
        }
    }
}
