using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;

using static AbicraftNodeEditor.NodeEditor;

using AbicraftNodeEditor;
using AbicraftNodes.Action;
using AbicraftMonos;

[CustomNodeEditor(typeof(AnimationOverrideNode))]
public class AnimationOverrideEditor : NodeEditor
{
    public Texture2D icon;

    AnimationOverrideNode node;
    AbicraftObject obj;
    Animator animator;

    int selectedIndex;

    public override void OnHeaderGUI(GUIStyle style)
    {   
        // Initialization
        if (node == null)
        {
            node = target as AnimationOverrideNode;
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
        node = target as AnimationOverrideNode;

        obj = node.GetInputValue<AbicraftObject>(null, "Obj");

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
